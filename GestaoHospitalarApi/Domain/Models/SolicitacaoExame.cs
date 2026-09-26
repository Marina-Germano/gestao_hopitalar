using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class SolicitacaoExame
{
    public int IdSolicitacao { get; set; }

    public int IdProntuario { get; set; }

    public int IdExame { get; set; }

    public int IdMedico { get; set; }

    public DateTime? DataSolicitacao { get; set; }

    public string? StatusExame { get; set; }

    public string? Resultado { get; set; }

    public virtual Exame IdExameNavigation { get; set; } = null!;

    public virtual Medico IdMedicoNavigation { get; set; } = null!;

    public virtual Prontuario IdProntuarioNavigation { get; set; } = null!;
}
