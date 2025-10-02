using ContaCorrente.Api.Domain;
using Microsoft.EntityFrameworkCore;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Infrastructure
{
    public class ContaCorrenteDbContext : DbContext
    {
        public ContaCorrenteDbContext(DbContextOptions<ContaCorrenteDbContext> options) : base(options)
        {
        }

        public DbSet<ContaCorrenteEntity> ContasCorrentes { get; set; }
        public DbSet<Movimento> Movimentos { get; set; }
        public DbSet<ChaveIdempotencia> ChavesIdempotencia { get; set; }
    }
}