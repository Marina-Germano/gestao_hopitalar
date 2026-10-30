namespace GestaoHospitalarApi.Application.DTOs
{
    // --- INTERAÇÃO MEDICAMENTOSA ---
    public class InteracaoCadastroDto
    {
        public int IdMedicamento1 { get; set; }
        public int IdMedicamento2 { get; set; }
        public string Gravidade { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class InteracaoListagemDto : InteracaoCadastroDto
    {
        public int IdInteracao { get; set; }
        public string NomeMedicamento1 { get; set; } = string.Empty;
        public string NomeMedicamento2 { get; set; } = string.Empty;
    }
}