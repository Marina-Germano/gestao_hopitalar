using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? Ativo { get; set; }

    public string Nome { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public string? Email { get; set; }

    public string Perfil { get; set; } = null!;

    public DateTime? DataCriacao { get; set; }

    public virtual Medico? Medico { get; set; }
}
