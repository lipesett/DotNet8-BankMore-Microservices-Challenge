using ContaCorrente.Api.Application.Abstractions;
using Dapper;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Commands.CreateContaCorrente
{
    public class CreateContaCorrenteCommandHandler : IRequestHandler<CreateContaCorrenteCommand, int>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public CreateContaCorrenteCommandHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> Handle(CreateContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar o CPF
            if (!IsValidCpf(request.Cpf))
            {
                // No futuro, trocar por uma exceção customizada de BadRequest
                throw new ArgumentException("CPF inválido");
            }

            // 2. Gerar Salt e Hash da Senha
            var salt = Guid.NewGuid().ToString();
            var senhaHash = HashPassword(request.Senha, salt);

            // 3. Criar a nova entidade de Conta Corrente
            var novaConta = new ContaCorrenteEntity
            {
                Numero = new Random().Next(10000, 99999),
                Nome = "Titular Padrão", // nome padrão por enquanto
                Ativo = true,
                Senha = senhaHash,
                Salt = salt
            };

            // 4. Persistir na tabela CONTACORRENTE
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();

            var sql = @"
                INSERT INTO ContasCorrentes (IdContaCorrente, Numero, Nome, Ativo, Senha, Salt)
                VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo, @Senha, @Salt);";

            await connection.ExecuteAsync(sql, novaConta);

            // 5. Retornar o número da conta
            return novaConta.Numero;
        }

        private bool IsValidCpf(string cpf)
        {
            // Apenas para fins de teste.
            return !string.IsNullOrWhiteSpace(cpf) && cpf.Length == 11 && cpf.All(char.IsDigit);
        }

        private string HashPassword(string password, string salt)
        {
            // Apenas para fins de teste. Uma solução real usaria uma biblioteca como BCrypt.Net
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}