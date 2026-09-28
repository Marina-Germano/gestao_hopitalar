using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdPessoa { get; set; }

    public string Login { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public string Perfil { get; set; } = null!;

    public int? Ativo { get; set; }

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;

    public virtual Medico? Medico { get; set; }
}
