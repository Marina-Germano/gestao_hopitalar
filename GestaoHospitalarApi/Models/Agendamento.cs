using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoHospitalarApi.Models;

public partial class Agendamento
{
    public int IdAgendamento { get; set; }

    public int IdPaciente { get; set; }

    public int IdMedico { get; set; }

    public int IdSala { get; set; }

    public DateTime DataHora { get; set; }

    public string? Status { get; set; }

    [ForeignKey("IdMedico")]
    public virtual Medico IdMedicoNavigation { get; set; } = null!;

    [ForeignKey("IdPaciente")]
    public virtual Paciente IdPacienteNavigation { get; set; } = null!;

    [ForeignKey("IdSala")]
    public virtual Sala IdSalaNavigation { get; set; } = null!;
}
