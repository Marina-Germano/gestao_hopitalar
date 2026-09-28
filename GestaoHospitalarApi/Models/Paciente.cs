using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Paciente
{
    public int IdPaciente { get; set; }

    public int IdPessoa { get; set; }

    public int? Ativo { get; set; }

    public string? Alergias { get; set; }

    public string? TipoSanguineo { get; set; }

    public string? HistoricoClinico { get; set; }

    public string? NomeResponsavel { get; set; }

    public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;

    public virtual ICollection<PacienteConvenio> PacienteConvenios { get; set; } = new List<PacienteConvenio>();

    public virtual ICollection<Prontuario> Prontuarios { get; set; } = new List<Prontuario>();

    public virtual ICollection<Triagem> Triagems { get; set; } = new List<Triagem>();
}
