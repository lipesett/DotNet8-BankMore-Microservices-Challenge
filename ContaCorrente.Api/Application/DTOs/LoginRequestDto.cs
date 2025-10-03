namespace ContaCorrente.Api.Application.DTOs
{
    public class LoginRequestDto
    {
        // Como o desafio diz que o login é feito por CPF ou número da conta, ambos são opcionais
        public int? NumeroConta { get; set; }
        public string? Cpf { get; set; }
        public string Senha { get; set; }
    }
}