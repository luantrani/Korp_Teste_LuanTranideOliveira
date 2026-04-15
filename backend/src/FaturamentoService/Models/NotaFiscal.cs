namespace FaturamentoService.Models
{
    public class NotaFiscal
    {
        public int Id { get; set; }
        public int Numeracao { get; set; }
        public string Status { get; set; } = "Aberta";
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public List<ItemNotaFiscal> Itens { get; set; } = new();
    }
}
