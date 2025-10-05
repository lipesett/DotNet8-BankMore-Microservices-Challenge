namespace Transferencia.Api.Application.DTOs
{
    public class MovimentacaoContaDto
    {
        public string IdRequisicao { get; set; }
        public int? NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
    }
}