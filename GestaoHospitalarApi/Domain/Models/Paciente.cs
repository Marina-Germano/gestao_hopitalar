using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models;

public partial class Paciente
{
    public int IdPaciente { get; set; }

    public int? Ativo { get; set; }

    public string Nome { get; set; } = null!;

    public string? Cpf { get; set; }

    public string? Sexo { get; set; }

    public DateOnly? Nascimento { get; set; }

    public string? Alergias { get; set; }

    public string? TipoSanguineo { get; set; }

    public string? HistoricoClinico { get; set; }

    public string? Telefone { get; set; }

    public string? Rua { get; set; }

    public int? NumeroCasa { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public string? Cep { get; set; }

    public string? NomeResponsavel { get; set; }

    public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    public virtual ICollection<PacienteConvenio> PacienteConvenios { get; set; } = new List<PacienteConvenio>();

    public virtual ICollection<Prontuario> Prontuarios { get; set; } = new List<Prontuario>();

    public virtual ICollection<Triagem> Triagems { get; set; } = new List<Triagem>();
}
