using MediatR;

namespace ContaCorrente.Api.Application.Commands.InativarConta
{
    public class InativarContaCommand : IRequest
    {
        public string IdContaCorrente { get; set; }
        public string Senha { get; set; }
    }
}