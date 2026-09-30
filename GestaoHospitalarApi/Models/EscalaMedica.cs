using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class EscalaMedica
{
    public int IdEscala { get; set; }

    public int IdMedico { get; set; }

    public DateOnly DataEscala { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFim { get; set; }

    public int? IsPlantao { get; set; }

    public virtual Medico IdMedicoNavigation { get; set; } = null!;
}
