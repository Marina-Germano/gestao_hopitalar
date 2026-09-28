using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Sala
{
    public int IdSala { get; set; }

    public string Nome { get; set; } = null!;

    public string? Tipo { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    public virtual ICollection<Prontuario> Prontuarios { get; set; } = new List<Prontuario>();
}
