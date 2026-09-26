using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class PacienteConvenio
{
    public int IdPacienteConvenio { get; set; }

    public int IdPaciente { get; set; }

    public int IdConvenio { get; set; }

    public string? NumeroCarteira { get; set; }

    public DateOnly Validade { get; set; }

    public int? Ativo { get; set; }

    public virtual Convenio IdConvenioNavigation { get; set; } = null!;

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
