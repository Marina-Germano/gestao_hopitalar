using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoHospitalarApi.Models;

public partial class InteracaoMedicamentosa
{
    public int IdInteracao { get; set; }

    public int IdMedicamento1 { get; set; }

    public int IdMedicamento2 { get; set; }

    public string? Gravidade { get; set; }

    public string? Descricao { get; set; }

    [ForeignKey("IdMedicamento1")]
    public virtual Medicamento IdMedicamento1Navigation { get; set; } = null!;

    [ForeignKey("IdMedicamento2")]
    public virtual Medicamento IdMedicamento2Navigation { get; set; } = null!;
}
