using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class ConsumoItem
{
    public int IdConsumo { get; set; }

    public int IdInternacao { get; set; }

    public int IdAlmoxarifado { get; set; }

    public int Quantidade { get; set; }

    public DateTime? DataConsumo { get; set; }

    public string? Observacao { get; set; }

    public virtual Almoxarifado IdAlmoxarifadoNavigation { get; set; } = null!;

    public virtual Internacao IdInternacaoNavigation { get; set; } = null!;
}
