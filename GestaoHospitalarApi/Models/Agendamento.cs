using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Agendamento
{
    public int IdAgendamento { get; set; }

    public int IdPaciente { get; set; }

    public int IdMedico { get; set; }

    public int IdSala { get; set; }

    public DateTime DataHora { get; set; }

    public string? Status { get; set; }

    public virtual Medico IdMedicoNavigation { get; set; } = null!;

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;

    public virtual Sala IdSalaNavigation { get; set; } = null!;
}
