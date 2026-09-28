using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Medicamento
{
    public int IdMedicamento { get; set; }

    public int IdAlmoxarifado { get; set; }

    public string? PrincipioAtivo { get; set; }

    public string? Contraindicacoes { get; set; }

    public virtual Almoxarifado IdAlmoxarifadoNavigation { get; set; } = null!;

    public virtual ICollection<InteracaoMedicamentosa> InteracaoMedicamentosaIdMedicamento1Navigations { get; set; } = new List<InteracaoMedicamentosa>();

    public virtual ICollection<InteracaoMedicamentosa> InteracaoMedicamentosaIdMedicamento2Navigations { get; set; } = new List<InteracaoMedicamentosa>();

    public virtual ICollection<Prescricao> Prescricaos { get; set; } = new List<Prescricao>();
}
