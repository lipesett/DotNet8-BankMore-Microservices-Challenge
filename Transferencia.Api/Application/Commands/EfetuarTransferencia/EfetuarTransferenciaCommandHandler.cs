using MediatR;
using Transferencia.Api.Application.Abstractions;
using Dapper;
using System.Text.Json;
using System.Net.Http.Headers;
using Transferencia.Api.Application.DTOs;
using TransferenciaEntity = Transferencia.Api.Domain.Transferencia;

namespace Transferencia.Api.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaCommandHandler : IRequestHandler<EfetuarTransferenciaCommand>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EfetuarTransferenciaCommandHandler(IDbConnectionFactory dbConnectionFactory, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var idempotencyKey = await connection.QueryFirstOrDefaultAsync<string>("SELECT Requisicao FROM ChavesIdempotencia WHERE IdChaveIdempotencia = @IdRequisicao", new { request.IdRequisicao }, transaction);
            if (idempotencyKey != null) { transaction.Commit(); return; }

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            var contaCorrenteClient = _httpClientFactory.CreateClient("ContaCorrenteClient");
            contaCorrenteClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);

            var responseContaDestino = await contaCorrenteClient.GetAsync($"/api/contas/por-numero/{request.NumeroContaDestino}");
            if (!responseContaDestino.IsSuccessStatusCode)
            {
                throw new Exception("Conta de destino não encontrada.");
            }
            var contaDestino = await responseContaDestino.Content.ReadFromJsonAsync<ContaCorrenteDto>();
            var idContaDestino = contaDestino.IdContaCorrente;

            var debitoDto = new MovimentacaoContaDto { IdRequisicao = Guid.NewGuid().ToString(), TipoMovimento = 'D', Valor = request.Valor };
            var responseDebito = await contaCorrenteClient.PostAsJsonAsync("/api/contas/movimentacao", debitoDto);
            if (!responseDebito.IsSuccessStatusCode) { transaction.Rollback(); throw new Exception("Falha ao debitar da conta de origem (ex: saldo insuficiente)."); }

            try
            {
                var creditoDto = new MovimentacaoContaDto { IdRequisicao = Guid.NewGuid().ToString(), NumeroConta = request.NumeroContaDestino, TipoMovimento = 'C', Valor = request.Valor };
                var responseCredito = await contaCorrenteClient.PostAsJsonAsync("/api/contas/movimentacao", creditoDto);
                if (!responseCredito.IsSuccessStatusCode) { throw new Exception("Falha ao creditar na conta de destino."); }
            }
            catch (Exception)
            {
                // Compensação (Estorno)
                var estornoDto = new MovimentacaoContaDto { IdRequisicao = Guid.NewGuid().ToString(), TipoMovimento = 'C', Valor = request.Valor };
                await contaCorrenteClient.PostAsJsonAsync("/api/contas/movimentacao", estornoDto);
                transaction.Rollback();
                throw;
            }

            var transferencia = new TransferenciaEntity
            {
                IdContaCorrente_Origem = request.IdContaCorrenteOrigem,
                IdContaCorrente_Destino = idContaDestino,
                DataMovimento = DateTime.UtcNow,
                Valor = request.Valor
            };
            var sqlTransferencia = @"INSERT INTO Transferencias (IdTransferencia, IdContaCorrente_Origem, IdContaCorrente_Destino, DataMovimento, Valor) 
                                     VALUES (@IdTransferencia, @IdContaCorrente_Origem, @IdContaCorrente_Destino, @DataMovimento, @Valor)";
            await connection.ExecuteAsync(sqlTransferencia, transferencia, transaction);

            var requestJson = JsonSerializer.Serialize(request);
            var sqlIdempotency = @"INSERT INTO ChavesIdempotencia (IdChaveIdempotencia, Requisicao) VALUES (@IdRequisicao, @RequestJson);";
            await connection.ExecuteAsync(sqlIdempotency, new { request.IdRequisicao, RequestJson = requestJson }, transaction);

            transaction.Commit();
        }
    }
}