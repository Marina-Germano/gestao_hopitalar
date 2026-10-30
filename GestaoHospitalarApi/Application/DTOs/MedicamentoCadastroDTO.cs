using System;

namespace GestaoHospitalarApi.Application.DTOs
{
    public class MedicamentoCadastroDto
    {
        // --- Campos da Tela de Estoque (Almoxarifado) ---
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int Quantidade { get; set; }
        public string? Unidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public int EstoqueMinimo { get; set; }
        public string? Lote { get; set; }
        public DateTime? Validade { get; set; }

        // --- Campos Clínicos (Medicamento) ---
        public string? PrincipioAtivo { get; set; }
        public string? Contraindicacoes { get; set; }
    }

    public class MedicamentoListagemDto : MedicamentoCadastroDto
    {
        public int IdMedicamento { get; set; }
        public int IdAlmoxarifado { get; set; }
        public string Categoria { get; set; } = "MEDICAMENTO";
    }
}