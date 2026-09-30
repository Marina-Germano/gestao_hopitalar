using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Faturamento
{
    public int IdFaturamento { get; set; }

    public int IdInternacao { get; set; }

    public decimal? TotalMedicamentos { get; set; }

    public decimal? TotalExames { get; set; }

    public decimal? TotalInternacao { get; set; }

    public decimal? TotalHonorarios { get; set; }

    public decimal? TotalConsumo { get; set; }

    public decimal? ValorTotal { get; set; }

    public string? StatusPagamento { get; set; }

    public DateTime DataFechamento { get; set; }

    public string? Observacao { get; set; }

    public virtual ICollection<Auditorium> Auditoria { get; set; } = new List<Auditorium>();

    public virtual Internacao IdInternacaoNavigation { get; set; } = null!;
}
