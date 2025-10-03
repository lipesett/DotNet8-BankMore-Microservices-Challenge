using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Api.Application.DTOs
{
    public class InativarContaRequestDto
    {
        [Required]
        public string Senha { get; set; }
    }
}