using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Exame
{
    public int IdExame { get; set; }

    public string Nome { get; set; } = null!;

    public decimal? Valor { get; set; }

    public string? Descricao { get; set; }

    public virtual ICollection<SolicitacaoExame> SolicitacaoExames { get; set; } = new List<SolicitacaoExame>();
}
