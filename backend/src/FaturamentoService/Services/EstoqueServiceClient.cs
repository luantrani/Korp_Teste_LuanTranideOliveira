using System.Net.Http.Json;
using FaturamentoService.Models;

namespace FaturamentoService.Services
{
    public class EstoqueServiceClient
    {
        private readonly HttpClient _httpClient;

        public EstoqueServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProdutoDto?> GetProdutoAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProdutoDto>($"api/produtos/{id}");
        }

        public async Task<bool> AjustarSaldoAsync(int id, int quantidade)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/produtos/{id}/saldo", quantidade);
            return response.IsSuccessStatusCode;
        }
    }
}
