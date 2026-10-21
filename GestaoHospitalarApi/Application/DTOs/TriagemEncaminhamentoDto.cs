namespace GestaoHospitalarApi.Application.DTOs
{
    public class TriagemEncaminhamentoDto
    {
        // --- Avaliação de Destino ---
        public string RiscoEvasao { get; set; } = string.Empty; // "Baixo", "Médio", "Alto"
        public bool PrecisaIsolamento { get; set; } // true (Sim) ou false (Não)
        public int IdMedicoResponsavel { get; set; }

        // --- Escolha do Radio Button ---
        public string TipoDestino { get; set; } = string.Empty; // Enviar "SALA" ou "LEITO"

        // Envia o ID correspondente ao Radio Button escolhido
        public int? IdSala { get; set; }
        public int? IdLeito { get; set; }
    }
}