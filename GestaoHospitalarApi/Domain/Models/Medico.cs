using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Medico
{
    public int IdMedico { get; set; }

    public int? Ativo { get; set; }

    public int IdEspecialidade { get; set; }

    public int? IdUsuario { get; set; }

    public string Nome { get; set; } = null!;

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string Crm { get; set; } = null!;

    public decimal? Honorario { get; set; }

    public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    public virtual ICollection<EscalaMedica> EscalaMedicas { get; set; } = new List<EscalaMedica>();

    public virtual Especialidade IdEspecialidadeNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<Prescricao> Prescricaos { get; set; } = new List<Prescricao>();

    public virtual ICollection<Prontuario> Prontuarios { get; set; } = new List<Prontuario>();

    public virtual ICollection<SolicitacaoExame> SolicitacaoExames { get; set; } = new List<SolicitacaoExame>();
}
