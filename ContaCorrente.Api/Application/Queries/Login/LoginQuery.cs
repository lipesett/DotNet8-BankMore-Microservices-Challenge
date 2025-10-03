using ContaCorrente.Api.Application.DTOs;
using MediatR;

namespace ContaCorrente.Api.Application.Queries.Login
{
    public class LoginQuery : IRequest<LoginResponseDto>
    {
        public int? NumeroConta { get; set; }
        public string? Cpf { get; set; }
        public string Senha { get; set; }
    }
}