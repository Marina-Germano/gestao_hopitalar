namespace GestaoHospitalarApi.Application.DTOs
{
    public class ConsumoItemCadastroDto
    {
        public int IdInternacao { get; set; } // Vinculado diretamente à Internação
        public int IdAlmoxarifado { get; set; }
        public int Quantidade { get; set; }
        public string? Observacao { get; set; } // Campo opcional para observações adicionais

    }

    public class ConsumoItemListagemDto : ConsumoItemCadastroDto
    {
        public int IdConsumo { get; set; }
        public string NomeItem { get; set; } = string.Empty;
        public string NomePaciente { get; set; } = string.Empty;
        public DateTime? DataConsumo { get; set; }
    }
}