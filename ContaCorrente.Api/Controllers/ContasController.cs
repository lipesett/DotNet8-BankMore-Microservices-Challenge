using ContaCorrente.Api.Application.Commands.CreateContaCorrente;
using ContaCorrente.Api.Application.DTOs;
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
    }
}