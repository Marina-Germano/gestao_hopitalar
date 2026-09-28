namespace GestaoHospitalarApi.Application.DTOs
{
    public class ConsumoItemCadastroDto
    {
        public int IdInternacao { get; set; } // Vinculado diretamente à Internação
        public int IdAlmoxarifado { get; set; }
        public int Quantidade { get; set; }

    }
}