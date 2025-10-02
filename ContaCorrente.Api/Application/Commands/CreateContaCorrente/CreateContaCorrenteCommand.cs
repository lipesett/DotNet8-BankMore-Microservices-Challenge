using MediatR;

namespace ContaCorrente.Api.Application.Commands.CreateContaCorrente
{
    public class CreateContaCorrenteCommand : IRequest<int>
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }
}