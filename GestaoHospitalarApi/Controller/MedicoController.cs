// using GestaoHospitalarApi.Application.DTOs;
// using GestaoHospitalarApi.Domain.Repositories;
// using GestaoHospitalarApi.Application.Wrappers;
// using GestaoHospitalarApi.Models;
// using Microsoft.AspNetCore.Mvc;

// namespace GestaoHospitalarApi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class MedicosController : ControllerBase
//     {
//         private readonly IGenericRepository<Medico> _medicoRepository;

//         public MedicosController(IGenericRepository<Medico> medicoRepository)
//         {
//             _medicoRepository = medicoRepository;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAll()
//         {
//             var medicos = await _medicoRepository.GetAllAsync();
//             return Ok(ResultWrapper<IEnumerable<Medico>>.Ok(medicos, "Médicos recuperados com sucesso."));
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetById(int id)
//         {
//             var medico = await _medicoRepository.GetByIdAsync(id);
//             if (medico == null)
//                 return NotFound(ResultWrapper<Medico>.Erro("Médico não encontrado."));

//             return Ok(ResultWrapper<Medico>.Ok(medico, "Médico recuperado com sucesso."));
//         }

//         [HttpPost]
//         public async Task<IActionResult> Create([FromBody] MedicoCadastroDto dto)
//         {
//             var medico = new Medico
//             {
//                 IdEspecialidade = dto.IdEspecialidade,
//                 Crm = dto.Crm,
//                 Honorario = dto.Honorario,
//                 IdUsuarioNavigation = new Usuario
//                 {
//                     Login = dto.Login,
//                     Senha = dto.Senha,
//                     Perfil = string.IsNullOrEmpty(dto.Perfil) ? "MEDICO" : dto.Perfil.ToUpper(),
//                     Ativo = 1,
//                     IdPessoaNavigation = new Pessoa
//                     {
//                         Nome = dto.Nome,
//                         Cpf = dto.Cpf,
//                         Nascimento = dto.Nascimento.HasValue
//                         ? DateOnly.FromDateTime(dto.Nascimento.Value)
//                         : null,
//                         Sexo = dto.Sexo,
//                         Telefone = dto.Telefone,
//                         Email = dto.Email,
//                         Rua = dto.Rua,
//                         NumeroCasa = dto.NumeroCasa,
//                         Bairro = dto.Bairro,
//                         Cidade = dto.Cidade,
//                         Estado = dto.Estado,
//                         Cep = dto.Cep
//                     }
//                 }
//             };

//             await _medicoRepository.AddAsync(medico);
//             await _medicoRepository.SaveChangesAsync();

//             return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Médico cadastrado com sucesso", idMedico = medico.IdMedico }, "Médico cadastrado com sucesso"));
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> Update(int id, [FromBody] MedicoCadastroDto dto)
//         {
//             var medico = await _medicoRepository.GetByIdAsync(id);
//             if (medico == null)
//                 return NotFound(ResultWrapper<Medico>.Erro("Médico não encontrado."));

//             medico.IdEspecialidade = dto.IdEspecialidade;
//             medico.Crm = dto.Crm;
//             medico.Honorario = dto.Honorario;

//             if (medico.IdUsuarioNavigation != null)
//             {
//                 medico.IdUsuarioNavigation.Login = dto.Login;
//                 medico.IdUsuarioNavigation.Senha = dto.Senha;

//                 if (medico.IdUsuarioNavigation.IdPessoaNavigation != null)
//                 {
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Nome = dto.Nome;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Cpf = dto.Cpf;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Nascimento = dto.Nascimento.HasValue
//                         ? DateOnly.FromDateTime(dto.Nascimento.Value)
//                         : null;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Sexo = dto.Sexo;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Telefone = dto.Telefone;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Email = dto.Email;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Rua = dto.Rua;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.NumeroCasa = dto.NumeroCasa;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Bairro = dto.Bairro;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Cidade = dto.Cidade;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Estado = dto.Estado;
//                     medico.IdUsuarioNavigation.IdPessoaNavigation.Cep = dto.Cep;
//                 }
//             }

//             _medicoRepository.Update(medico);
//             await _medicoRepository.SaveChangesAsync();

//             return Ok(ResultWrapper<object>.Ok(new { mensagem = "Médico atualizado com sucesso." }, "Médico atualizado com sucesso."));
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var medico = await _medicoRepository.GetByIdAsync(id);
//             if (medico == null)
//                 return NotFound(ResultWrapper<Medico>.Erro("Médico não encontrado."));

//             _medicoRepository.Delete(medico);
//             await _medicoRepository.SaveChangesAsync();

//             return Ok(ResultWrapper<string>.Ok(string.Empty, "Médico removido com sucesso."));
//         }
//     }
// }