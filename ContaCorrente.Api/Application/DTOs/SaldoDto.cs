namespace ContaCorrente.Api.Application.DTOs
{
    public class SaldoDto
    {
        public int NumeroConta { get; set; }
        public string? NomeTitular { get; set; }
        public DateTime DataHoraConsulta { get; set; }
        public decimal ValorSaldo { get; set; }
    }
}