using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Api.Domain
{
    public class Movimento
    {
        [Key]
        public string IdMovimento { get; set; } = Guid.NewGuid().ToString();
        public string IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public char TipoMovimento { get; set; } // 'C' para Crédito, 'D' para Débito
        public decimal Valor { get; set; }
    }
}