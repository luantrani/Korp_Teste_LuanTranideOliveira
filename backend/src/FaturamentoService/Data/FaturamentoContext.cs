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
    }
}
