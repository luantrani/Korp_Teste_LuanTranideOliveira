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

            if (produto.Saldo < 0)
            {
                return BadRequest("Saldo inicial não pode ser negativo.");
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(int id, Produto produto)
        {
            if (id != produto.Id && produto.Id != 0)
            {
                return BadRequest("O id da rota deve corresponder ao id do produto.");
            }

            var produtoExistente = await _context.Produtos.FindAsync(id);
            if (produtoExistente == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(produto.Codigo) || string.IsNullOrWhiteSpace(produto.Descricao))
            {
                return BadRequest("Código e descrição são obrigatórios.");
            }

            if (produto.Saldo < 0)
            {
                return BadRequest("Saldo não pode ser negativo.");
            }

            produtoExistente.Codigo = produto.Codigo;
            produtoExistente.Descricao = produto.Descricao;
            produtoExistente.Saldo = produto.Saldo;

            await _context.SaveChangesAsync();
            return Ok(produtoExistente);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
