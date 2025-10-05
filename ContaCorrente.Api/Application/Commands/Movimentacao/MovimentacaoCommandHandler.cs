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
                var idempotencyCheckSql = "SELECT Requisicao FROM ChavesIdempotencia WHERE IdChaveIdempotencia = @IdRequisicao";
                var savedRequestJson = await connection.QueryFirstOrDefaultAsync<string>(idempotencyCheckSql, new { request.IdRequisicao }, transaction);

                if (savedRequestJson != null)
                {
                    var requestDataToCompare = new { IdContaCorrente = request.IdContaCorrente, Valor = request.Valor, TipoMovimento = request.TipoMovimento, NumeroConta = request.NumeroConta };
                    var currentRequestJson = JsonSerializer.Serialize(requestDataToCompare);

                    if (savedRequestJson == currentRequestJson)
                    {
                        transaction.Commit();
                        return;
                    }
                    else
                    {
                        throw new BusinessRuleViolationException("Chave de idempotência está sendo reutilizada com uma requisição diferente.", "IDEMPOTENCY_ERROR");
                    }
                }

                string idContaAlvo;
                ContaCorrenteEntity conta;

                if (request.NumeroConta.HasValue)
                {
                    conta = await connection.QueryFirstOrDefaultAsync<ContaCorrenteEntity>(
                        "SELECT * FROM ContasCorrentes WHERE Numero = @NumeroConta",
                        new { request.NumeroConta },
                        transaction);

                    if (conta == null)
                        throw new BusinessRuleViolationException("A conta de destino informada não existe.", "INVALID_ACCOUNT");
                }
                else
                {
                    conta = await connection.QueryFirstOrDefaultAsync<ContaCorrenteEntity>(
                        "SELECT * FROM ContasCorrentes WHERE IdContaCorrente = @IdContaCorrente",
                        new { request.IdContaCorrente },
                        transaction);
                }

                idContaAlvo = conta.IdContaCorrente;

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
                    IdContaCorrente = idContaAlvo,
                    DataMovimento = DateTime.UtcNow,
                    TipoMovimento = tipo,
                    Valor = request.Valor
                };

                var sqlMovimento = @"INSERT INTO Movimentos (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor)
                                     VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";
                await connection.ExecuteAsync(sqlMovimento, movimento, transaction);

                var requestJsonToSave = JsonSerializer.Serialize(new { IdContaCorrente = request.IdContaCorrente, Valor = request.Valor, TipoMovimento = request.TipoMovimento, NumeroConta = request.NumeroConta });
                var sqlIdempotency = @"INSERT INTO ChavesIdempotencia (IdChaveIdempotencia, Requisicao)
                                       VALUES (@IdRequisicao, @RequestJson)";
                await connection.ExecuteAsync(sqlIdempotency, new { request.IdRequisicao, RequestJson = requestJsonToSave }, transaction);

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