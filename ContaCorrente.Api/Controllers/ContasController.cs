using ContaCorrente.Api.Application.Commands.CreateContaCorrente;
using ContaCorrente.Api.Application.DTOs;
using ContaCorrente.Api.Application.Exceptions;
using ContaCorrente.Api.Application.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
                // Captura a exceção no Handler para CPF inválido.
                if (ex.Message.Contains("CPF inválido"))
                {
                    var errorResponse = new ErrorResponse
                    {
                        Mensagem = "O CPF fornecido é inválido.",
                        TipoFalha = "INVALID_DOCUMENT"
                    };
                    return BadRequest(errorResponse);
                }
                // Se for outra ArgumentException, retorna um erro genérico.
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
    }
}