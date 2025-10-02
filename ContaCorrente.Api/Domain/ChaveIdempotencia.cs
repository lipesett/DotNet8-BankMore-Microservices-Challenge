using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Api.Domain
{
    public class ChaveIdempotencia
    {
        [Key]
        public string IdChaveIdempotencia { get; set; }
        public string? Requisicao { get; set; }
        public string? Resultado { get; set; }
    }
}