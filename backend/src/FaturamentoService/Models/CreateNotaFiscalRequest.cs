namespace FaturamentoService.Models
{
    public class CreateNotaFiscalRequest
    {
        public List<ItemNotaFiscalRequest> Itens { get; set; } = new();
    }
}
