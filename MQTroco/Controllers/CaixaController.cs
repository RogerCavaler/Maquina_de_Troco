using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using MQTroco.Models;
using MQTroco.Repositories.Interface;
using MQTroco.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Controllers
{
    public class CaixaController : Controller
    {
        private readonly ICaixaRepository _caixaRepository;
        private readonly IMoedaRepository _moedaRepository;
        private readonly ICaixaMoedaRepository _caixaMoedaRepository;

        public CaixaController(ICaixaRepository caixaRepository, IMoedaRepository moedaRepository, ICaixaMoedaRepository caixaMoedaRepository)
        {
            _caixaRepository = caixaRepository;
            _moedaRepository = moedaRepository;
            _caixaMoedaRepository = caixaMoedaRepository;
        }

        public IActionResult Index()
        {

            var caixas = _caixaRepository.Caixas;

            caixas.ForEach(_obj =>
            {
                _obj.Saldo = _caixaMoedaRepository.CaixaMoedas(_obj.Id).Sum(s => (s.QtdMoeda * s.MoedaModel.Valor));
                _obj.Status = _obj.DataAbertura != new DateTime() && _obj.DataFechamento == new DateTime();
            });

            caixas = caixas.OrderByDescending(o => o.Status).ThenBy(t => t.DataAbertura).ToList();

            return View(caixas);
        }

        public IActionResult Create()
        {
            CaixaModel caixa = new CaixaModel
            {
                Id = Common.NewId()
            };
            
            return View(caixa);
        }

        [HttpPost]
        public IActionResult Create(CaixaModel caixa)
        {
            try
            {
                caixa.DataAbertura = DateTime.Now;
                _caixaRepository.AddCaixa(caixa);
                return RedirectToAction("Abastecimento", new RouteValueDictionary(new { idcaixa = caixa.Id } ));
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", $"Falha na abertura do caixa - {e.Message}");
                return View(caixa);
            }
        }

        [HttpGet]
        public IActionResult Encerrar(string idcaixa)
        {
            CaixaModel caixa = _caixaRepository.GetCaixaById(idcaixa);
            return View(caixa);
        }

        [HttpPost]
        public IActionResult Encerrar(CaixaModel caixa)
        {
            try
            {
                caixa.DataFechamento = DateTime.Now;
                if (_caixaRepository.UpdateCaixa(caixa))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception("Erro não identificado!");
                }                
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", $"Falha ao realizar o fechamento de caixa - {e.Message}");
                return View(caixa);
            }
        }

        public IActionResult Abastecimento(string idcaixa)
        {
            CaixaMoedaViewModel caixaMoeda = new CaixaMoedaViewModel();
            caixaMoeda.Caixa = _caixaRepository.GetCaixaById(idcaixa);
            caixaMoeda.CaixaMoedas = _caixaMoedaRepository.CaixaMoedas(idcaixa);
            caixaMoeda.Moedas = new List<MoedaModel>();

            foreach (var item in _moedaRepository.Moedas)
            {
                caixaMoeda.Moedas.Add(item);
                if (caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == item.Id).Count() == 0)
                {
                    caixaMoeda.CaixaMoedas.Add(new CaixaMoedaModel
                    {
                        Id = "aa",
                        CaixaModelId = caixaMoeda.Caixa.Id,
                        CaixaModel = caixaMoeda.Caixa,
                        MoedaModelId = item.Id,
                        MoedaModel = item,
                        QtdMoeda = 0
                    });
                }
            }

            caixaMoeda.CaixaMoedas = caixaMoeda.CaixaMoedas.OrderBy(o => o.MoedaModel.Valor).Select(s => s).ToList();

            return View(caixaMoeda);
        }

        [HttpPost]
        public IActionResult Abastecimento(CaixaMoedaViewModel caixaMoeda, IFormCollection formCollection)
        {
            try
            {
                caixaMoeda.Caixa = _caixaRepository.GetCaixaById(caixaMoeda.Caixa.Id);
                caixaMoeda.Moedas = _moedaRepository.Moedas;
                caixaMoeda.CaixaMoedas = _caixaMoedaRepository.CaixaMoedas(caixaMoeda.Caixa.Id);

                #region validacao
                foreach (var i in formCollection.Where(w => w.Value != caixaMoeda.Caixa.Id && w.Key != "__RequestVerificationToken"))
                {
                    if (caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).Count() >= 0)
                    {
                        var _cxm = caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).FirstOrDefault();
                        if (!object.Equals(_cxm, null) && Convert.ToInt32(i.Value) < _cxm.QtdMoeda)
                        {
                            throw new Exception($"Não é permitido deduzir moedas no abastecimento de estoque! Para essa operação utilize a opção de SANGRIA");
                        }
                    }
                }
                #endregion

                foreach (var i in formCollection.Where(w => w.Value != caixaMoeda.Caixa.Id && w.Key != "__RequestVerificationToken"))
                {
                    if (caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).Count() == 0)
                    {
                        var _m = _moedaRepository.GetMoedaById(i.Key);
                        _caixaMoedaRepository.AddMoeda(new CaixaMoedaModel
                        {
                            MoedaModel = _m,
                            MoedaModelId = _m.Id,
                            CaixaModel = caixaMoeda.Caixa,
                            CaixaModelId = caixaMoeda.Caixa.Id,
                            QtdMoeda = Convert.ToInt32(i.Value)
                        });
                    }
                    else
                    {
                        var _cxm = caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).FirstOrDefault();
                        _cxm.QtdMoeda = Convert.ToInt32(i.Value);
                        _caixaMoedaRepository.UpdateMoeda(_cxm);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                string idcaixa = caixaMoeda.Caixa.Id;
                caixaMoeda = new CaixaMoedaViewModel();
                caixaMoeda.Caixa = _caixaRepository.GetCaixaById(idcaixa);
                caixaMoeda.CaixaMoedas = _caixaMoedaRepository.CaixaMoedas(idcaixa);
                caixaMoeda.Moedas = new List<MoedaModel>();

                foreach (var item in _moedaRepository.Moedas)
                {
                    caixaMoeda.Moedas.Add(item);
                    if (caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == item.Id).Count() == 0)
                    {
                        caixaMoeda.CaixaMoedas.Add(new CaixaMoedaModel
                        {
                            Id = "aa",
                            CaixaModelId = caixaMoeda.Caixa.Id,
                            CaixaModel = caixaMoeda.Caixa,
                            MoedaModelId = item.Id,
                            MoedaModel = item,
                            QtdMoeda = formCollection.Where(w => w.Key == item.Id).Count() > 0 ? Convert.ToInt32(formCollection.Where(w => w.Key == item.Id).FirstOrDefault().Value) : 0
                        });
                    }
                }

                caixaMoeda.CaixaMoedas = caixaMoeda.CaixaMoedas.OrderBy(o => o.MoedaModel.Valor).Select(s => s).ToList();

                ModelState.AddModelError("", $"Falha ao realizar o abastecimento do caixa - {e.Message}");

                return View(caixaMoeda);
            }
        }

        [HttpGet]
        public IActionResult Sangria(string idcaixa)
        {
            CaixaMoedaViewModel caixaMoeda = new CaixaMoedaViewModel();
            caixaMoeda.Caixa = _caixaRepository.GetCaixaById(idcaixa);
            caixaMoeda.CaixaMoedas = _caixaMoedaRepository.CaixaMoedas(idcaixa);
            caixaMoeda.Moedas = new List<MoedaModel>();

            foreach (var item in _moedaRepository.Moedas)
            {
                caixaMoeda.Moedas.Add(item);
                if (caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == item.Id).Count() == 0)
                {
                    caixaMoeda.CaixaMoedas.Add(new CaixaMoedaModel
                    {
                        Id = "aa",
                        CaixaModelId = caixaMoeda.Caixa.Id,
                        CaixaModel = caixaMoeda.Caixa,
                        MoedaModelId = item.Id,
                        MoedaModel = item,
                        QtdMoeda = 0
                    });
                }
            }

            caixaMoeda.CaixaMoedas = caixaMoeda.CaixaMoedas.OrderBy(o => o.MoedaModel.Valor).Select(s => s).ToList();

            return View(caixaMoeda);
        }

        [HttpPost]
        public IActionResult Sangria(CaixaMoedaViewModel caixaMoeda, IFormCollection formCollection)
        {
            try
            {
                caixaMoeda.Caixa = _caixaRepository.GetCaixaById(caixaMoeda.Caixa.Id);
                caixaMoeda.CaixaMoedas = _caixaMoedaRepository.CaixaMoedas(caixaMoeda.Caixa.Id);

                #region validacao
                foreach (var i in formCollection.Where(w => w.Value != caixaMoeda.Caixa.Id && w.Key != "__RequestVerificationToken" && w.Key != "__qtdDisponivel"))
                {
                    if (string.IsNullOrEmpty(i.Value))
                        continue;

                    if (caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).Count() >= 0)
                    {
                        var _cxm = caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).FirstOrDefault();
                        if (Convert.ToInt32(i.Value) > _cxm.QtdMoeda)
                        {
                            throw new Exception($"Não é permitido incrementar moedas na sangria de estoque! Para essa operação utilize a opção de ABASTECIMENTO");
                        }

                        if((_cxm.QtdMoeda- Convert.ToInt32(i.Value)) < 0)
                        {
                            throw new Exception($"Não é possível remover mais moedas do que há em caixa! ({_cxm.MoedaModel.Descricao} | {_cxm.QtdMoeda})");
                        }
                    }
                }
                #endregion

                foreach (var i in formCollection.Where(w => w.Value != caixaMoeda.Caixa.Id && w.Key != "__RequestVerificationToken" && w.Key != "__qtdDisponivel"))
                {
                    if (string.IsNullOrEmpty(i.Value))
                        continue;

                    if (caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).Count() >= 0)
                    {
                        var _cxm = caixaMoeda.CaixaMoedas.Where(w => w.MoedaModel.Id == i.Key).FirstOrDefault();
                        _cxm.QtdMoeda -= Convert.ToInt32(i.Value);
                        _caixaMoedaRepository.UpdateMoeda(_cxm);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                string idcaixa = caixaMoeda.Caixa.Id;
                caixaMoeda = new CaixaMoedaViewModel();
                caixaMoeda.Caixa = _caixaRepository.GetCaixaById(idcaixa);
                caixaMoeda.Moedas = _moedaRepository.Moedas;
                caixaMoeda.CaixaMoedas = _caixaMoedaRepository.CaixaMoedas(idcaixa);
                caixaMoeda.CaixaMoedas = caixaMoeda.CaixaMoedas.OrderBy(o => o.MoedaModel.Valor).Select(s => s).ToList();

                ModelState.AddModelError("", $"Falha ao realizar a sangria de caixa - {e.Message}");

                return View(caixaMoeda);
            }
        }
    }
}
