using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Triagem
{
    public int IdTriagem { get; set; }

    public int IdPaciente { get; set; }

    public string? ResponsavelTriagem { get; set; }

    public string? Pressao { get; set; }

    public decimal? Temperatura { get; set; }

    public int? FrequenciaCardiaca { get; set; }

    public int? Saturacao { get; set; }

    public int? EscalaDor { get; set; }

    public string? Risco { get; set; }

    public string? Queixa { get; set; }

    public string? Observacoes { get; set; }

    public string? Internacao { get; set; }

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;

    public virtual ICollection<Prontuario> Prontuarios { get; set; } = new List<Prontuario>();
}
