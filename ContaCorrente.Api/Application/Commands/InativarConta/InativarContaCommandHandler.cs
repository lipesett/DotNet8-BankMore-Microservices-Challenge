using ContaCorrente.Api.Application.Abstractions;
using ContaCorrente.Api.Application.Exceptions;
using Dapper;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Commands.InativarConta
{
    public class InativarContaCommandHandler : IRequestHandler<InativarContaCommand>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public InativarContaCommandHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();

            var sql = "SELECT * FROM ContasCorrentes WHERE IdContaCorrente = @IdContaCorrente";
            var user = await connection.QueryFirstOrDefaultAsync<ContaCorrenteEntity>(sql, new { request.IdContaCorrente });

            if (user == null)
            {
                throw new NotFoundException("Conta corrente não encontrada.");
            }

            var senhaHash = HashPassword(request.Senha, user.Salt);

            if (user.Senha != senhaHash)
            {
                throw new ArgumentException("Senha inválida.");
            }

            var updateSql = "UPDATE ContasCorrentes SET Ativo = 0 WHERE IdContaCorrente = @IdContaCorrente";
            await connection.ExecuteAsync(updateSql, new { request.IdContaCorrente });
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}