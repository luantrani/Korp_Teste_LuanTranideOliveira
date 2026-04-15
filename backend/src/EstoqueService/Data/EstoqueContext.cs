using Microsoft.EntityFrameworkCore;
using EstoqueService.Models;

namespace EstoqueService.Data
{
    public class EstoqueContext : DbContext
    {
        public EstoqueContext(DbContextOptions<EstoqueContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; } = null!;
    }
}
