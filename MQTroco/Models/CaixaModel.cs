using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Models
{
    public class CaixaModel
    {
        [Required(ErrorMessage = "É obrigatório  definir a Identificação do caixa")]
        [Display(Name = "Identificação", Prompt = "Identificação do caixa")]
        public string Id { get; set; }

        [Required(ErrorMessage = "É obrigatório  definir a Data de Abertura do caixa")]
        [Display(Name = "Data de abertura", Prompt = "Data de abertura do caixa")]
        public DateTime DataAbertura { get; set; }
                
        [Display(Name = "Data de fechamento", Prompt = "Data de fechamento do caixa")]
        public DateTime DataFechamento { get; set; }

        [NotMapped]
        public bool Status { get; set; }

        [NotMapped]
        public decimal Saldo { get; set; }
    }
}
