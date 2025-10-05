using MediatR;

namespace ContaCorrente.Api.Application.Commands.Movimentacao
{
    public class MovimentacaoCommand : IRequest
    {
        public string IdRequisicao { get; set; }
        public string IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
        public int? NumeroConta { get; set; }
    }
}