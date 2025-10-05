using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Api.Application.DTOs
{
    public class MovimentacaoRequestDto
    {
        [Required]
        public string IdRequisicao { get; set; } // Para a Idempotência

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public char TipoMovimento { get; set; } // 'C' para Crédito, 'D' para Débito

        public int? NumeroConta { get; set; }
    }
}