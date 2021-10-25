using MQTroco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Repositories.Interface
{
    public interface ICaixaMoedaRepository
    {
        List<CaixaMoedaModel> CaixaMoedas(string idcaixa);
        public CaixaMoedaModel CaixaMoedasById(string id);
        public void AddMoeda(CaixaMoedaModel moeda);
        public void UpdateMoeda(CaixaMoedaModel moeda);
    }
}
