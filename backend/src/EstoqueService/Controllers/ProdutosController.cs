using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstoqueService.Data;
using EstoqueService.Models;

namespace EstoqueService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly EstoqueContext _context;

        public ProdutosController(EstoqueContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Produto>> GetProdutos()
        {
            return await _context.Produtos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduto(Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Codigo) || string.IsNullOrWhiteSpace(produto.Descricao))
            {
                return BadRequest("Código e descrição são obrigatórios.");
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        [HttpPut("{id}/saldo")]
        public async Task<IActionResult> AjustarSaldo(int id, [FromBody] int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            if (produto.Saldo + quantidade < 0)
            {
                return BadRequest("Saldo insuficiente para essa operação.");
            }

            produto.Saldo += quantidade;
            await _context.SaveChangesAsync();
            return Ok(produto);
        }
    }
}
