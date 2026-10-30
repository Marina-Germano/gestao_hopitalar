using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoHospitalarApi.Models;

public partial class PacienteConvenio
{
    public int IdPacienteConvenio { get; set; }

    public int IdPaciente { get; set; }

    public int IdConvenio { get; set; }

    public string? NumeroCarteira { get; set; }

    public DateOnly Validade { get; set; }

    public int? Ativo { get; set; }

    [ForeignKey("IdConvenio")]
    public virtual Convenio IdConvenioNavigation { get; set; } = null!;

    [ForeignKey("IdPaciente")]
    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
