using ContaCorrente.Api.Application.Abstractions;
using Dapper;
using MediatR;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Queries.GetContaPorNumero
{
    public class GetContaPorNumeroQueryHandler : IRequestHandler<GetContaPorNumeroQuery, ContaCorrenteEntity>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public GetContaPorNumeroQueryHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<ContaCorrenteEntity> Handle(GetContaPorNumeroQuery request, CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = "SELECT * FROM ContasCorrentes WHERE Numero = @NumeroConta";
            return await connection.QueryFirstOrDefaultAsync<ContaCorrenteEntity>(sql, new { request.NumeroConta });
        }
    }
}