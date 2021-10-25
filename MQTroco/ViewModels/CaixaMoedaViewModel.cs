using MQTroco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.ViewModels
{
    public class CaixaMoedaViewModel
    {
        public CaixaModel Caixa { get; set; }
        public List<MoedaModel> Moedas { get; set; }
        public List<CaixaMoedaModel> CaixaMoedas { get; set; }
    }
}
