using Microsoft.EntityFrameworkCore;
using FaturamentoService.Models;

namespace FaturamentoService.Data
{
    public class FaturamentoContext : DbContext
    {
        public FaturamentoContext(DbContextOptions<FaturamentoContext> options) : base(options)
        {
        }

        public DbSet<NotaFiscal> NotasFiscais { get; set; } = null!;
        public DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotaFiscal>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.HasMany(n => n.Itens)
                      .WithOne(i => i.NotaFiscal)
                      .HasForeignKey(i => i.NotaFiscalId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItemNotaFiscal>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Quantidade).IsRequired();
            });
        }
    }
}
