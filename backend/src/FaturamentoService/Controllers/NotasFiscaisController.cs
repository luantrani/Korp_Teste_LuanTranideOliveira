using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaturamentoService.Data;
using FaturamentoService.Models;
using FaturamentoService.Services;

namespace FaturamentoService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly FaturamentoContext _context;
        private readonly EstoqueServiceClient _estoqueClient;

        public NotasFiscaisController(FaturamentoContext context, EstoqueServiceClient estoqueClient)
        {
            _context = context;
            _estoqueClient = estoqueClient;
        }

        [HttpGet]
        public async Task<IEnumerable<NotaFiscal>> GetNotas()
        {
            return await _context.NotasFiscais.Include(n => n.Itens).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNota(int id)
        {
            var nota = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            return Ok(nota);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNota(NotaFiscal nota)
        {
            if (nota.Itens == null || !nota.Itens.Any())
            {
                return BadRequest("A nota deve conter pelo menos um item.");
            }

            var ultimaNumeracao = await _context.NotasFiscais.MaxAsync(n => (int?)n.Numeracao) ?? 0;
            nota.Numeracao = ultimaNumeracao + 1;
            nota.Status = "Aberta";
            nota.CriadoEm = DateTime.UtcNow;

            foreach (var item in nota.Itens)
            {
                var produto = await _estoqueClient.GetProdutoAsync(item.ProdutoId);
                if (produto == null)
                {
                    return BadRequest($"Produto {item.ProdutoId} não encontrado no estoque.");
                }

                if (produto.Saldo < item.Quantidade)
                {
                    return BadRequest($"Saldo insuficiente para o produto {produto.Codigo}.");
                }
            }

            foreach (var item in nota.Itens)
            {
                var ok = await _estoqueClient.AjustarSaldoAsync(item.ProdutoId, -item.Quantidade);
                if (!ok)
                {
                    return StatusCode(502, "Erro ao atualizar saldo no serviço de estoque.");
                }
            }

            _context.NotasFiscais.Add(nota);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNota), new { id = nota.Id }, nota);
        }

        [HttpPost("{id}/imprimir")]
        public async Task<IActionResult> ImprimirNota(int id)
        {
            var nota = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);
            if (nota == null)
            {
                return NotFound();
            }

            if (nota.Status != "Aberta")
            {
                return BadRequest("A nota fiscal só pode ser impressa quando estiver com status Aberta.");
            }

            nota.Status = "Fechada";
            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Nota fiscal impressa e status atualizado para Fechada." });
        }
    }
}
