using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class InteracaoMedicamentosa
{
    public int IdInteracao { get; set; }

    public int IdMedicamento1 { get; set; }

    public int IdMedicamento2 { get; set; }

    public string? Gravidade { get; set; }

    public string? Descricao { get; set; }

    public virtual Medicamento IdMedicamento1Navigation { get; set; } = null!;

    public virtual Medicamento IdMedicamento2Navigation { get; set; } = null!;
}
