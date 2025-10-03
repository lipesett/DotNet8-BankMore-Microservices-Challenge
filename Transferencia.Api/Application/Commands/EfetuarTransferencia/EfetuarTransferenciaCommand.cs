using MediatR;

namespace Transferencia.Api.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaCommand : IRequest
    {
        public string IdRequisicao { get; set; }
        public string IdContaCorrenteOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}