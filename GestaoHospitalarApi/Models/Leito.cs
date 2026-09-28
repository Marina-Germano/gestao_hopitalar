using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Leito
{
    public int IdLeito { get; set; }

    public string Numero { get; set; } = null!;

    public string? Ala { get; set; }

    public string? Andar { get; set; }

    public DateTime DataHigienizacao { get; set; }

    public string? Situacao { get; set; }

    public virtual ICollection<Internacao> Internacaos { get; set; } = new List<Internacao>();
}
