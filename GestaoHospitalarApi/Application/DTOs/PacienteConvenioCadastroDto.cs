using System;

namespace GestaoHospitalarApi.Application.DTOs
{
    public class PacienteConvenioCadastroDto
    {
        public int IdPaciente { get; set; }
        public int IdConvenio { get; set; }
        public string? NomeConvenio { get; set; } 
        public string? Numero { get; set; }
        public string? NumeroCarteira { get; set; }
        public DateTime Validade { get; set; }
        public int Ativo { get; set; } = 1;
    }

    public class PacienteConvenioListagemDto : PacienteConvenioCadastroDto
    {
        public int IdPacienteConvenio { get; set; }
        public string NomePaciente { get; set; } = string.Empty;
        public string NomeConvenio { get; set; } = string.Empty;
    }
}