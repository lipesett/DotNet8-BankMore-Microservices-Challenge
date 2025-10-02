using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Api.Application.DTOs
{
    public class CreateContaCorrenteRequest
    {
        [Required]
        public string Cpf { get; set; }
        [Required]
        public string Senha { get; set; }
    }
}