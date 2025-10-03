using ContaCorrente.Api.Application.Abstractions;
using ContaCorrente.Api.Application.Exceptions;
using ContaCorrente.Api.Domain;
using Dapper;
using MediatR;
using System.Text.Json;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Commands.Movimentacao
{
    public class MovimentacaoCommandHandler : IRequestHandler<MovimentacaoCommand>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovimentacaoCommandHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task Handle(MovimentacaoCommand request, CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Verificação de Idempotência Robusta
                var idempotencyCheckSql = "SELECT Requisicao FROM ChavesIdempotencia WHERE IdChaveIdempotencia = @IdRequisicao";
                var savedRequestJson = await connection.QueryFirstOrDefaultAsync<string>(idempotencyCheckSql, new { request.IdRequisicao }, transaction);

                if (savedRequestJson != null)
                {
                    // Define um tipo anônimo para representar a estrutura da requisição
                    var requestShape = new { request.IdContaCorrente, request.Valor, request.TipoMovimento };
                    var currentRequestJson = JsonSerializer.Serialize(requestShape);

                    // Compara os objetos em vez das strings brutas
                    if (savedRequestJson == currentRequestJson)
                    {
                        // É uma repetição válida e idêntica. Retorna sucesso.
                        transaction.Commit();
                        return;
                    }
                    else
                    {
                        // A chave está sendo reutilizada com um corpo diferente. Isso é um erro.
                        throw new BusinessRuleViolationException("Chave de idempotência está sendo reutilizada com uma requisição diferente.", "IDEMPOTENCY_ERROR");
                    }
                }

                // 2. Validações de Negócio (AGORA EXECUTADAS APENAS PARA NOVAS REQUISIÇÕES)
                var conta = await connection.QueryFirstOrDefaultAsync<ContaCorrenteEntity>(
                    "SELECT * FROM ContasCorrentes WHERE IdContaCorrente = @IdContaCorrente",
                    new { request.IdContaCorrente },
                    transaction);

                if (conta == null)
                    throw new BusinessRuleViolationException("Apenas contas correntes cadastradas podem receber movimentação.", "INVALID_ACCOUNT");

                if (!conta.Ativo)
                    throw new BusinessRuleViolationException("Apenas contas correntes ativas podem receber movimentação.", "INACTIVE_ACCOUNT");

                if (request.Valor <= 0)
                    throw new BusinessRuleViolationException("Apenas valores positivos podem ser recebidos.", "INVALID_VALUE");

                var tipo = char.ToUpper(request.TipoMovimento);
                if (tipo != 'C' && tipo != 'D')
                    throw new BusinessRuleViolationException("Apenas os tipos 'C' (crédito) ou 'D' (débito) podem ser aceitos.", "INVALID_TYPE");

                var movimento = new Movimento
                {
                    IdContaCorrente = request.IdContaCorrente,
                    DataMovimento = DateTime.UtcNow,
                    TipoMovimento = tipo,
                    Valor = request.Valor
                };
                var sqlMovimento = @"INSERT INTO Movimentos (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor)
                                     VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor);";
                await connection.ExecuteAsync(sqlMovimento, movimento, transaction);

                var newRequestJson = JsonSerializer.Serialize(new { request.IdContaCorrente, request.Valor, request.TipoMovimento });
                var sqlIdempotency = @"INSERT INTO ChavesIdempotencia (IdChaveIdempotencia, Requisicao)
                                       VALUES (@IdRequisicao, @RequestJson);";
                await connection.ExecuteAsync(sqlIdempotency, new { request.IdRequisicao, RequestJson = newRequestJson }, transaction);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}