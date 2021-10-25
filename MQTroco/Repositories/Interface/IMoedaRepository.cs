using MQTroco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Repositories.Interface
{
    public interface IMoedaRepository
    {
        List<MoedaModel> Moedas { get; }
        MoedaModel GetMoedaById(string moeda);
        public string AddMoeda(MoedaModel moeda);        
    }
}
