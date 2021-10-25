using MQTroco.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.ViewModels
{
    public class TrocoViewModel
    {
        [Required(ErrorMessage = "É obrigatório informar um valor base para calculo do troco")]
        [Display(Name = "Valor primário", Prompt = "Valor base para calculo do troco")]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "É obrigatório informar o valor pago")]
        [Display(Name = "Valor pago", Prompt = "Valor pago")]
        [Column(TypeName = "decimal(9,2)")]
        public decimal ValorPago { get; set; }
                
        public List<CaixaMoedaModel> CaixaMoedas { get; set; }
        public CaixaModel Caixa { get; set; }
    }
}
