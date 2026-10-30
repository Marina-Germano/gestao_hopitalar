using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoHospitalarApi.Models;

public partial class Prescricao
{
    public int IdPrescricao { get; set; }

    public int IdProntuario { get; set; }

    public int IdMedico { get; set; }

    public int IdMedicamento { get; set; }

    public string? Dosagem { get; set; }

    public string? Aplicacao { get; set; }

    public string Horario { get; set; } = null!;

    public string? Observacao { get; set; }

    [ForeignKey("IdMedicamento")]
    public virtual Medicamento IdMedicamentoNavigation { get; set; } = null!;

    [ForeignKey("IdMedico")]
    public virtual Medico IdMedicoNavigation { get; set; } = null!;

    [ForeignKey("IdProntuario")]
    public virtual Prontuario IdProntuarioNavigation { get; set; } = null!;
}
