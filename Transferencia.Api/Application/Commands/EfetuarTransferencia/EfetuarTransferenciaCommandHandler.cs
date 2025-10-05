using MediatR;
using System.Net.Http;

namespace Transferencia.Api.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaCommandHandler : IRequestHandler<EfetuarTransferenciaCommand>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EfetuarTransferenciaCommandHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            var contaCorrenteClient = _httpClientFactory.CreateClient("ContaCorrenteClient");

            // TODO: Implementar a lógica de transferência completa aqui

            // Exemplo de como faríamos uma chamada:
            // var response = await contaCorrenteClient.PostAsJsonAsync("/api/contas/movimentacao", ...);

            await Task.CompletedTask;
        }
    }
}