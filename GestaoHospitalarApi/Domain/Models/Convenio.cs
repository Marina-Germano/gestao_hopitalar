using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Convenio
{
    public int IdConvenio { get; set; }

    public int? Ativo { get; set; }

    public string NomeConvenio { get; set; } = null!;

    public string? TipoLeito { get; set; }

    public int? CobreInternacao { get; set; }

    public int? CobreExames { get; set; }

    public int? CobreCirurgia { get; set; }

    public decimal? LimiteMedicamento { get; set; }

    public decimal? PercentualCobertura { get; set; }

    public virtual ICollection<PacienteConvenio> PacienteConvenios { get; set; } = new List<PacienteConvenio>();
}
