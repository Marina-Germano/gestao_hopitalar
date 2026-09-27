using System;
using System.Collections.Generic;

namespace GestaoHospitalarApi.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public int? Ativo { get; set; }
        public string Nome { get; set; } = null!;
        public string? Cpf { get; set; }
        public DateTime? Nascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Rua { get; set; }
        public int? NumeroCasa { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }
        public string Login { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string Perfil { get; set; } = null!;
        public DateTime? DataCriacao { get; set; }

        // Propriedades de Navegação (Relacionamentos)
        // Um usuário PODE ser um Médico (1 para 1)
        public virtual Medico? Medico { get; set; }

        // Um usuário PODE ser um Paciente (1 para 1)
        public virtual Paciente? Paciente { get; set; }
    }
}