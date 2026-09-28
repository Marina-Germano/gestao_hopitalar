namespace GestaoHospitalarApi.Application.DTOs
{
    public class ConvenioCadastroDto
    {
        public string NomeConvenio { get; set; } = string.Empty;
        public string? TipoLeito { get; set; } // COMUM, PRIVADO, PREMIUM
        public int CobreInternacao { get; set; } = 1;
        public int CobreExames { get; set; } = 1;
        public int CobreCirurgia { get; set; } = 1;
        public decimal? LimiteMedicamento { get; set; }
        public decimal? PercentualCobertura { get; set; }
        public int Ativo { get; set; } = 1;
    }
}