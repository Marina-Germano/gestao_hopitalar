using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Almoxarifado
{
    public int IdAlmoxarifado { get; set; }

    public string Nome { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Descricao { get; set; }

    public int Quantidade { get; set; }

    public string? Unidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public int? EstoqueMinimo { get; set; }

    public string? Lote { get; set; }

    public DateOnly? Validade { get; set; }

    public virtual ICollection<ConsumoItem> ConsumoItems { get; set; } = new List<ConsumoItem>();

    public virtual ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
}
