using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Especialidade
{
    public int IdEspecialidade { get; set; }

    public string DescricaoEspecialidade { get; set; } = null!;

    public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();
}
