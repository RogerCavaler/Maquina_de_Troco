using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Models
{
    public class CaixaMoedaModel
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "É obrigatório  definir o caixa vinculado")]
        [Display(Name = "Caixa", Prompt = "Identificação do caixa")]
        [ForeignKey("CaixaModel")]
        public string CaixaModelId { get; set; }

        
        public virtual CaixaModel CaixaModel { get; set; }

        [Required(ErrorMessage = "É obrigatório  definir a moeda vinculada")]
        [Display(Name = "Moeda", Prompt = "Identificação da moeda")]
        [ForeignKey("MoedaModel")]
        public string MoedaModelId { get; set; }

        
        public virtual MoedaModel MoedaModel { get; set; }

        [Required(ErrorMessage = "É obrigatório  definir a quantidade de moedas")]
        [Display(Name = "Quantidade", Prompt = "Quantidade de moedas")]
        [Range(0, 99999999999, ErrorMessage = "Valor fora do intervalo permitido (0 - 99999999999)")]
        public int QtdMoeda { get; set; }
    }
}
