namespace GestaoHospitalarApi.Application.DTOs
{
    public class AlmoxarifadoCadastroDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty; // MEDICAMENTO, DESCARTAVEL, LIMPEZA, EPI, INSUMO
        public string? Descricao { get; set; }
        public int Quantidade { get; set; }
        public string? Unidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public int? EstoqueMinimo { get; set; }
        public string? Lote { get; set; }
        public DateTime? Validade { get; set; }
    }
}