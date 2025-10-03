using System.ComponentModel.DataAnnotations;

namespace Transferencia.Api.Domain
{
    public class Transferencia
    {
        [Key]
        public string IdTransferencia { get; set; } = Guid.NewGuid().ToString();
        public string IdContaCorrente_Origem { get; set; }
        public string IdContaCorrente_Destino { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}