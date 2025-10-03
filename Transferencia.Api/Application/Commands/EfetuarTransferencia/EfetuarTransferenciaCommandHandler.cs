using MediatR;

namespace Transferencia.Api.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaCommandHandler : IRequestHandler<EfetuarTransferenciaCommand>
    {
        // Futuramente, injetaremos o HttpClient e a fábrica de conexão aqui

        public async Task Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implementar a lógica de transferência:
            // 1. Verificação de Idempotência
            // 2. Validações (conta ativa, saldo, etc.)
            // 3. Chamada HTTP para a API ContaCorrente para debitar
            // 4. Chamada HTTP para a API ContaCorrente para creditar
            // 5. Lógica de compensação (estorno)
            // 6. Persistir a transferência e a chave de idempotência

            await Task.CompletedTask;
        }
    }
}