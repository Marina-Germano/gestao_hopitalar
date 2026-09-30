using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Ala
{
    public int IdAla { get; set; }

    public string NomeAla { get; set; } = null!;

    public string Andar { get; set; } = null!;
}
