using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoHospitalarApi.Models;

public partial class Leito
{
    public int IdLeito { get; set; }

    public int IdAla { get; set; }

    public string Numero { get; set; } = null!;

    public string? TipoLeito { get; set; }

    public DateTime DataHigienizacao { get; set; }

    public string? Situacao { get; set; }

    [ForeignKey("IdAla")]
    public virtual Ala IdAlaNavigation { get; set; } = null!;
    public virtual ICollection<Internacao> Internacaos { get; set; } = new List<Internacao>();
}
