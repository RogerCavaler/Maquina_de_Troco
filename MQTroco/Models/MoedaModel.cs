using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Models
{
    public class MoedaModel
    {
        [Required(ErrorMessage = "É obrigatório  definir a Identificação da moeda")]
        [Display(Name = "Identificação", Prompt = "Identificação do caixa")]
        public string Id { get; set; }

        [Required(ErrorMessage = "É obrigatório definir a descrição da moeda")]
        [Display(Name = "Descrição", Prompt = "Descrição da moeda")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "É obrigatório  definir o valor da moeda")]
        [Display(Name = "Valor", Prompt = "Valor da moeda")]
        [Range(0.1, 1, ErrorMessage = "Valor fora do intervalo permitido (0,1 - 1)")]
        [Column(TypeName = "decimal(1,2)")]
        public decimal Valor { get; set; }
    }
}
