using ContaCorrente.Api.Application.Abstractions;
using ContaCorrente.Api.Application.DTOs;
using ContaCorrente.Api.Application.Exceptions;
using Dapper;
using MediatR;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Queries.GetSaldo
{
    public class GetSaldoQueryHandler : IRequestHandler<GetSaldoQuery, SaldoDto>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public GetSaldoQueryHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<SaldoDto> Handle(GetSaldoQuery request, CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();

            var conta = await connection.QueryFirstOrDefaultAsync<ContaCorrenteEntity>(
                "SELECT * FROM ContasCorrentes WHERE IdContaCorrente = @IdContaCorrente",
                new { request.IdContaCorrente });

            if (conta == null)
                throw new BusinessRuleViolationException("Apenas contas correntes cadastradas podem consultar o saldo.", "INVALID_ACCOUNT");

            if (!conta.Ativo)
                throw new BusinessRuleViolationException("Apenas contas correntes ativas podem consultar o saldo.", "INACTIVE_ACCOUNT");

            var sqlSaldo = @"
                SELECT COALESCE(SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE -Valor END), 0)
                FROM Movimentos
                WHERE IdContaCorrente = @IdContaCorrente;";

            var saldo = await connection.QuerySingleOrDefaultAsync<decimal>(sqlSaldo, new { request.IdContaCorrente });

            var saldoDto = new SaldoDto
            {
                NumeroConta = conta.Numero,
                NomeTitular = conta.Nome,
                DataHoraConsulta = DateTime.UtcNow,
                ValorSaldo = saldo
            };

            return saldoDto;
        }
    }
}