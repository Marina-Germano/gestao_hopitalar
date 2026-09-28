using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Faturamento
{
    public int IdFaturamento { get; set; }

    public int IdInternacao { get; set; }

    public decimal? ValorMedicamentos { get; set; }

    public decimal? ValorExames { get; set; }

    public decimal? ValorInternacao { get; set; }

    public decimal? ValorHonorarios { get; set; }

    public decimal? ValorConsumo { get; set; }

    public decimal? ValorTotal { get; set; }

    public string? StatusPagamento { get; set; }

    public DateTime DataFechamento { get; set; }

    public string? Observacao { get; set; }

    public virtual ICollection<Auditorium> Auditoria { get; set; } = new List<Auditorium>();

    public virtual Internacao IdInternacaoNavigation { get; set; } = null!;
}
