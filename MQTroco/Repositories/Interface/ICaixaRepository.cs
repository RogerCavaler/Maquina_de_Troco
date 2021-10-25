using MQTroco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Repositories.Interface
{
    public interface ICaixaRepository
    {
        List<CaixaModel> Caixas { get; }
        CaixaModel GetCaixaById(string id);
        public string AddCaixa(CaixaModel caixa);
        public bool UpdateCaixa(CaixaModel caixa);
    }
}
