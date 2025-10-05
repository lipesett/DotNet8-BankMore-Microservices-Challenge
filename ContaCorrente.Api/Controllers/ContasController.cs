using ContaCorrente.Api.Application.Commands.CreateContaCorrente;
using ContaCorrente.Api.Application.Commands.InativarConta;
using ContaCorrente.Api.Application.Commands.Movimentacao;
using ContaCorrente.Api.Application.DTOs;
using ContaCorrente.Api.Application.Exceptions;
using ContaCorrente.Api.Application.Queries.GetContaPorNumero;
using ContaCorrente.Api.Application.Queries.GetSaldo;
using ContaCorrente.Api.Application.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContaCorrente.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContaCorrenteRequest request)
        {
            try
            {
                var command = new CreateContaCorrenteCommand
                {
                    Cpf = request.Cpf,
                    Senha = request.Senha
                };

                var numeroConta = await _mediator.Send(command);

                var response = new CreateContaCorrenteResponse { NumeroConta = numeroConta };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("CPF inválido"))
                {
                    var errorResponse = new ErrorResponse
                    {
                        Mensagem = "O CPF fornecido é inválido.",
                        TipoFalha = "INVALID_DOCUMENT"
                    };
                    return BadRequest(errorResponse);
                }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var query = new LoginQuery
                {
                    NumeroConta = request.NumeroConta,
                    Cpf = request.Cpf,
                    Senha = request.Senha
                };

                var response = await _mediator.Send(query);

                return Ok(response);
            }
            catch (UnauthorizedUserException ex)
            {
                var errorResponse = new ErrorResponse
                {
                    Mensagem = ex.Message,
                    TipoFalha = "USER_UNAUTHORIZED"
                };
                return Unauthorized(errorResponse);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Inativar([FromBody] InativarContaRequestDto request)
        {
            var idContaCorrente = User.FindFirstValue("idcontacorrente");

            if (string.IsNullOrEmpty(idContaCorrente))
            {
                return Unauthorized();
            }

            var command = new InativarContaCommand
            {
                IdContaCorrente = idContaCorrente,
                Senha = request.Senha
            };

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost("movimentacao")]
        [Authorize]
        public async Task<IActionResult> Movimentacao([FromBody] MovimentacaoRequestDto request)
        {
            try
            {
                var idContaCorrente = User.FindFirstValue("idcontacorrente");
                if (string.IsNullOrEmpty(idContaCorrente))
                {
                    return Unauthorized();
                }

                var command = new MovimentacaoCommand
                {
                    IdRequisicao = request.IdRequisicao,
                    IdContaCorrente = idContaCorrente,
                    Valor = request.Valor,
                    TipoMovimento = request.TipoMovimento,
                    NumeroConta = request.NumeroConta,
                };

                await _mediator.Send(command);

                return NoContent();
            }
            catch (BusinessRuleViolationException ex)
            {
                var errorResponse = new ErrorResponse
                {
                    Mensagem = ex.Message,
                    TipoFalha = ex.FailureType
                };
                return BadRequest(errorResponse);
            }
        }

        [HttpGet("saldo")]
        [Authorize]
        public async Task<IActionResult> GetSaldo()
        {
            try
            {
                var idContaCorrente = User.FindFirstValue("idcontacorrente");
                if (string.IsNullOrEmpty(idContaCorrente))
                {
                    return Unauthorized();
                }

                var query = new GetSaldoQuery { IdContaCorrente = idContaCorrente };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (BusinessRuleViolationException ex)
            {
                var errorResponse = new ErrorResponse
                {
                    Mensagem = ex.Message,
                    TipoFalha = ex.FailureType
                };
                return BadRequest(errorResponse);
            }
        }

        [HttpGet("por-numero/{numeroConta}")]
        [Authorize]
        public async Task<IActionResult> GetByNumero(int numeroConta)
        {
            var query = new GetContaPorNumeroQuery { NumeroConta = numeroConta };
            var result = await _mediator.Send(query);
            return result != null ? Ok(result) : NotFound();
        }
    }
}