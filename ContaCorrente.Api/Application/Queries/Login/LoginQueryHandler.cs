using ContaCorrente.Api.Application.Abstractions;
using ContaCorrente.Api.Application.DTOs;
using ContaCorrente.Api.Application.Exceptions;
using Dapper;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponseDto>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ITokenService _tokenService;

        public LoginQueryHandler(IDbConnectionFactory dbConnectionFactory, ITokenService tokenService)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            // A requisição deve conter ou o número da conta ou o CPF
            if (!request.NumeroConta.HasValue && string.IsNullOrWhiteSpace(request.Cpf))
            {
                throw new UnauthorizedUserException("Número da conta ou CPF deve ser fornecido.");
            }

            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();

            // Busca o usuário pelo número da conta ou CPF
            var sql = "SELECT * FROM ContasCorrentes WHERE Numero = @NumeroConta OR Cpf = @Cpf";
            var user = await connection.QueryFirstOrDefaultAsync<ContaCorrenteEntity>(sql, new { request.NumeroConta, request.Cpf });

            if (user == null)
            {
                throw new UnauthorizedUserException("Usuário não encontrado.");
            }

            var senhaHash = HashPassword(request.Senha, user.Salt);

            // Valida a senha com o registro no banco de dados
            if (user.Senha != senhaHash)
            {
                throw new UnauthorizedUserException("Senha inválida.");
            }

            // Retorna um token JWT
            var token = _tokenService.CreateToken(user);

            return new LoginResponseDto { Token = token };
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