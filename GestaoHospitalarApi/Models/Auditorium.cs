using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Auditorium
{
    public int IdAuditoria { get; set; }

    public int IdFaturamento { get; set; }

    public string? Auditor { get; set; }

    public DateTime DataAuditoria { get; set; }

    public string? StatusAuditoria { get; set; }

    public string? Observacoes { get; set; }

    public int? Conformidade { get; set; }

    public virtual Faturamento IdFaturamentoNavigation { get; set; } = null!;
}
