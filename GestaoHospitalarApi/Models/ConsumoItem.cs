using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoHospitalarApi.Models;

public partial class ConsumoItem
{
    public int IdConsumo { get; set; }

    public int IdInternacao { get; set; }

    public int IdAlmoxarifado { get; set; }

    public int Quantidade { get; set; }

    public DateTime DataConsumo { get; set; }

    public string? Observacao { get; set; }

    [ForeignKey("IdAlmoxarifado")]
    public virtual Almoxarifado IdAlmoxarifadoNavigation { get; set; } = null!;

    [ForeignKey("IdInternacao")]
    public virtual Internacao IdInternacaoNavigation { get; set; } = null!;
}
