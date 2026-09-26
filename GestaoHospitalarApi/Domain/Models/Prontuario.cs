using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Prontuario
{
    public int IdProntuario { get; set; }

    public int IdPaciente { get; set; }

    public int IdTriagem { get; set; }

    public int IdMedico { get; set; }

    public int? IdSala { get; set; }

    public string RiscoEvasao { get; set; } = null!;

    public string Isolamento { get; set; } = null!;

    public string? Evolucao { get; set; }

    public DateTime? DataAbertura { get; set; }

    public string? StatusProntuario { get; set; }

    public virtual Medico IdMedicoNavigation { get; set; } = null!;

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;

    public virtual Sala? IdSalaNavigation { get; set; }

    public virtual Triagem IdTriagemNavigation { get; set; } = null!;

    public virtual ICollection<Internacao> Internacaos { get; set; } = new List<Internacao>();

    public virtual ICollection<LogProntuario> LogProntuarios { get; set; } = new List<LogProntuario>();

    public virtual ICollection<Prescricao> Prescricaos { get; set; } = new List<Prescricao>();

    public virtual ICollection<SolicitacaoExame> SolicitacaoExames { get; set; } = new List<SolicitacaoExame>();
}
