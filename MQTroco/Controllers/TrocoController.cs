using Microsoft.AspNetCore.Mvc;
using MQTroco.Models;
using MQTroco.Repositories.Interface;
using MQTroco.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Controllers
{
    public class TrocoController : Controller
    {
        private readonly ICaixaRepository _caixaRepository;
        private readonly IMoedaRepository _moedaRepository;
        private readonly ICaixaMoedaRepository _caixaMoedaRepository;

        public TrocoController(ICaixaRepository caixaRepository, IMoedaRepository moedaRepository, ICaixaMoedaRepository caixaMoedaRepository)
        {
            _caixaRepository = caixaRepository;
            _moedaRepository = moedaRepository;
            _caixaMoedaRepository = caixaMoedaRepository;
        }
        public IActionResult Index()
        {
            TrocoViewModel troco = new TrocoViewModel();
            troco.Caixa = _caixaRepository.Caixas.Where(w => w.DataAbertura != new DateTime() && w.DataFechamento == new DateTime()).FirstOrDefault();

            if(object.Equals(troco.Caixa, null))
            {
                return RedirectToAction("Index", "Caixa");
            }

            troco.CaixaMoedas = new List<CaixaMoedaModel>();
            return View(troco);
        }

        [HttpPost]
        public IActionResult Calcular(TrocoViewModel troco)
        {
            troco.Valor = troco.Valor/ 100;
            troco.ValorPago = troco.ValorPago / 100;
            troco.CaixaMoedas = Calcular((troco.ValorPago - troco.Valor), troco.Caixa.Id);

            var caixamoedas = _caixaMoedaRepository.CaixaMoedas(troco.Caixa.Id);

            if(caixamoedas.Sum(s => (s.QtdMoeda*s.MoedaModel.Valor)) < troco.CaixaMoedas.Sum(s => (s.QtdMoeda * s.MoedaModel.Valor)))
            {
                ModelState.AddModelError("", $"Operação cancelada! Valor de troco superior ao valor em caixa - R$ {troco.CaixaMoedas.Sum(s => (s.QtdMoeda * s.MoedaModel.Valor))}");
                troco.CaixaMoedas.Clear();
            }
            else if(caixamoedas.Sum(s => (s.QtdMoeda * s.MoedaModel.Valor)) < (troco.ValorPago-troco.Valor))
            {
                ModelState.AddModelError("", $"Operação cancelada! Valor de troco (R$ {troco.ValorPago - troco.Valor}) superior ao valor em caixa (R$ {troco.CaixaMoedas.Sum(s => (s.QtdMoeda * s.MoedaModel.Valor))})");
                troco.CaixaMoedas.Clear();
            }

            foreach (var m in troco.CaixaMoedas)
            {
                var _cxm = caixamoedas.Where(w => w.MoedaModel.Id == m.MoedaModel.Id).FirstOrDefault();
                _cxm.QtdMoeda -= m.QtdMoeda;
                _caixaMoedaRepository.UpdateMoeda(_cxm);
            }

            return View("~/Views/Troco/Index.cshtml", troco);
        }

        private List<CaixaMoedaModel> Calcular(decimal valor, string idcaixa)
        {
            List<CaixaMoedaModel> retorno = new List<CaixaMoedaModel>();
            var caixamoedas = _caixaMoedaRepository.CaixaMoedas(idcaixa);

            while (retorno.Sum(s => (s.MoedaModel.Valor*s.QtdMoeda)) < valor)
            {
                Dictionary<string, int> keys = new Dictionary<string, int>();
                foreach (var cm in caixamoedas.Where(w => w.QtdMoeda > 0 && retorno.Where(w1 => w1.MoedaModel.Id == w.MoedaModel.Id).Count() == 0))
                {
                    int qtd_moeda = (int)Math.Round((valor - retorno.Sum(s => (s.MoedaModel.Valor * s.QtdMoeda))) / cm.MoedaModel.Valor, 0, MidpointRounding.ToZero);
                    keys.Add(cm.MoedaModel.Id, qtd_moeda > cm.QtdMoeda ? cm.QtdMoeda : qtd_moeda);
                }

                if(keys.Where(w => w.Value == 0).Count() == keys.Count)
                    return retorno;

                var moedas = new CaixaMoedaModel
                {
                    MoedaModel = caixamoedas.Where(w => w.MoedaModelId == keys.Where(k => k.Value > 0).OrderBy(o => o.Value).FirstOrDefault().Key).FirstOrDefault().MoedaModel,
                    QtdMoeda = keys.Where(k => k.Value > 0).OrderBy(o => o.Value).FirstOrDefault().Value
                };
                retorno.Add(moedas);

                if(retorno.Sum(s => (s.MoedaModel.Valor * s.QtdMoeda)) > valor)
                {
                    retorno.Remove(moedas);
                    return retorno;
                }                
            }
            return retorno;
        }
    }
}
