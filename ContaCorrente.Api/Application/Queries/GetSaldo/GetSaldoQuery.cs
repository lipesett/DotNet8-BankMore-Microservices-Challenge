using ContaCorrente.Api.Application.DTOs;
using MediatR;

namespace ContaCorrente.Api.Application.Queries.GetSaldo
{
    public class GetSaldoQuery : IRequest<SaldoDto>
    {
        public string IdContaCorrente { get; set; }
    }
}