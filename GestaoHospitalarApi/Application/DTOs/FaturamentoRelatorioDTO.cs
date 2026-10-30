namespace GestaoHospitalarApi.Application.DTOs
{
    public class FaturamentoRelatorioDto
    {
        public int IdInternacao { get; set; }
        public int IdProntuario { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataAlta { get; set; }
        public int TotalDiarias { get; set; }
        
        public decimal ValorTotalDiarias { get; set; }
        public decimal ValorTotalConsumo { get; set; }
        public decimal ValorTotalExames { get; set; }
        
        public decimal ValorTotalGeral { get; set; }

        // Detalhamento opcional para o Front-end
        public List<ItemConsumoFaturamentoDto> DetalhesConsumo { get; set; } = new();
        public List<ItemExameFaturamentoDto> DetalhesExames { get; set; } = new();
    }

    public class ItemConsumoFaturamentoDto
    {
        public string NomeItem { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotalItem { get; set; }
    }

    public class ItemExameFaturamentoDto
    {
        public string NomeExame { get; set; } = string.Empty;
        public decimal ValorExame { get; set; }
    }

    public class FaturamentoListagemDto
    {
        public int IdFaturamento { get; set; }
        public int IdInternacao { get; set; }
        public string NomePaciente { get; set; } = string.Empty;
        public decimal TotalMedicamentos { get; set; }
        public decimal TotalExames { get; set; }
        public decimal TotalInternacao { get; set; }
        public decimal TotalHonorarios { get; set; }
        public decimal TotalConsumo { get; set; }
        public decimal ValorTotal { get; set; }
        public string StatusPagamento { get; set; } = string.Empty;
        public DateTime? DataFechamento { get; set; }
        public string? Observacao { get; set; }
    }
}