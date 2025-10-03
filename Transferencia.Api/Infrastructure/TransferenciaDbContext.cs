using Microsoft.EntityFrameworkCore;
using Transferencia.Api.Domain;
using TransferenciaEntity = Transferencia.Api.Domain.Transferencia;

namespace Transferencia.Api.Infrastructure
{
    public class TransferenciaDbContext : DbContext
    {
        public TransferenciaDbContext(DbContextOptions<TransferenciaDbContext> options) : base(options)
        {
        }

        public DbSet<TransferenciaEntity> Transferencias { get; set; }
        public DbSet<ChaveIdempotencia> ChavesIdempotencia { get; set; }
    }
}