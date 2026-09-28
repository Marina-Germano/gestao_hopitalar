using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Internacao
{
    public int IdInternacao { get; set; }

    public int IdProntuario { get; set; }

    public int IdLeito { get; set; }

    public DateTime DataEntrada { get; set; }

    public DateTime? DataAlta { get; set; }

    public string? Isolamento { get; set; }

    public string? StatusInternacao { get; set; }

    public virtual ICollection<ConsumoItem> ConsumoItems { get; set; } = new List<ConsumoItem>();

    public virtual ICollection<Faturamento> Faturamentos { get; set; } = new List<Faturamento>();

    public virtual Leito IdLeitoNavigation { get; set; } = null!;

    public virtual Prontuario IdProntuarioNavigation { get; set; } = null!;
}
