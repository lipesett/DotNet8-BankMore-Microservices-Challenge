using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Transferencia.Api.Application.Commands.EfetuarTransferencia;
using Transferencia.Api.Application.DTOs;

namespace Transferencia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferenciasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EfetuarTransferencia([FromBody] TransferenciaRequestDto request)
        {
            try
            {
                var idContaCorrenteOrigem = User.FindFirstValue("idcontacorrente");
                if (string.IsNullOrEmpty(idContaCorrenteOrigem))
                {
                    return Unauthorized();
                }

                var command = new EfetuarTransferenciaCommand
                {
                    IdRequisicao = request.IdRequisicao,
                    IdContaCorrenteOrigem = idContaCorrenteOrigem,
                    NumeroContaDestino = request.NumeroContaDestino,
                    Valor = request.Valor
                };

                await _mediator.Send(command);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = "Ocorreu uma falha durante a transferência.", Detalhe = ex.Message });
            }
        }
    }
}