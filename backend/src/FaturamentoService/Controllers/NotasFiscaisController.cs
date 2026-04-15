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
        public async Task<IActionResult> CreateNota(CreateNotaFiscalRequest request)
        {
            if (request.Itens == null || !request.Itens.Any())
            {
                return BadRequest("A nota deve conter pelo menos um item.");
            }

            if (request.Itens.Any(i => i.Quantidade <= 0))
            {
                return BadRequest("A quantidade de cada item deve ser maior que zero.");
            }

            var produtoIds = request.Itens.Select(i => i.ProdutoId).ToList();
            if (produtoIds.Distinct().Count() != produtoIds.Count)
            {
                return BadRequest("Não é permitido adicionar o mesmo produto mais de uma vez em uma nota.");
            }

            var produtos = new Dictionary<int, ProdutoDto>();
            foreach (var item in request.Itens)
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

                produtos[item.ProdutoId] = produto;
            }

            var ultimaNumeracao = await _context.NotasFiscais.MaxAsync(n => (int?)n.Numeracao) ?? 0;
            var nota = new NotaFiscal
            {
                Numeracao = ultimaNumeracao + 1,
                Status = "Aberta",
                CriadoEm = DateTime.UtcNow,
                Itens = request.Itens.Select(i => new ItemNotaFiscal
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            var atualizados = new List<ItemNotaFiscalRequest>();
            foreach (var item in request.Itens)
            {
                var ok = await _estoqueClient.AjustarSaldoAsync(item.ProdutoId, -item.Quantidade);
                if (!ok)
                {
                    await RollbackEstoqueAsync(atualizados);
                    return StatusCode(502, "Falha ao atualizar o estoque. A operação foi revertida quando possível.");
                }

                atualizados.Add(item);
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

        private async Task RollbackEstoqueAsync(List<ItemNotaFiscalRequest> itens)
        {
            foreach (var item in itens)
            {
                await _estoqueClient.AjustarSaldoAsync(item.ProdutoId, item.Quantidade);
            }
        }
    }
}
