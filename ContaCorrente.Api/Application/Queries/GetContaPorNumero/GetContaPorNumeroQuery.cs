using MediatR;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Queries.GetContaPorNumero
{
    public class GetContaPorNumeroQuery : IRequest<ContaCorrenteEntity>
    {
        public int NumeroConta { get; set; }
    }
}