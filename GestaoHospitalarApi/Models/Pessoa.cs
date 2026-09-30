using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Pessoa
{
    public int IdPessoa { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateOnly? Nascimento { get; set; }

    public string? Sexo { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string? Rua { get; set; }

    public string? NumeroCasa { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public string? Cep { get; set; }

    public DateTime DataCriacao { get; set; }

    public virtual Paciente? Paciente { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
