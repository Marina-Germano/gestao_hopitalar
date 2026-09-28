using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class LogProntuario
{
    public int IdLog { get; set; }

    public int IdProntuario { get; set; }

    public string? Responsavel { get; set; }

    public DateTime DataAlteracao { get; set; }

    public string? Descricao { get; set; }

    public virtual Prontuario IdProntuarioNavigation { get; set; } = null!;
}
