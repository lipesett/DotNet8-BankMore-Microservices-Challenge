using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Api.Domain
{
    public class ContaCorrente
    {
        [Key]
        public string IdContaCorrente { get; set; } = Guid.NewGuid().ToString();
        public int Numero { get; set; }
        public string? Nome { get; set; }
        public bool Ativo { get; set; }
        public string? Senha { get; set; }
        public string? Salt { get; set; }
    }
}