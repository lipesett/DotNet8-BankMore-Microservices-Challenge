using System.ComponentModel.DataAnnotations;

namespace Transferencia.Api.Application.DTOs
{
    public class TransferenciaRequestDto
    {
        [Required]
        public string IdRequisicao { get; set; }

        [Required]
        public int NumeroContaDestino { get; set; }

        [Required]
        public decimal Valor { get; set; }
    }
}