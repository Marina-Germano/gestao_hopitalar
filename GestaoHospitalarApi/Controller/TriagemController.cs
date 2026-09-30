// using GestaoHospitalarApi.Application.DTOs;
// using GestaoHospitalarApi.Domain.Repositories;
// using GestaoHospitalarApi.Application.Wrappers;
// using GestaoHospitalarApi.Models;
// using Microsoft.AspNetCore.Mvc;

// namespace GestaoHospitalarApi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class TriagensController : ControllerBase
//     {
//         private readonly IGenericRepository<Triagem> _triagemRepository;
//         private readonly IGenericRepository<Paciente> _pacienteRepository;
//         private readonly IGenericRepository<PacienteConvenio> _pacienteConvenioRepository;

//         public TriagensController(
//             IGenericRepository<Triagem> triagemRepository, 
//             IGenericRepository<Paciente> pacienteRepository,
//             IGenericRepository<PacienteConvenio> pacienteConvenioRepository)
//         {
//             _triagemRepository = triagemRepository;
//             _pacienteRepository = pacienteRepository;
//             _pacienteConvenioRepository = pacienteConvenioRepository;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAll()
//         {
//             var triagens = await _triagemRepository.GetAllAsync();
//             return Ok(ResultWrapper<IEnumerable<Triagem>>.Ok(triagens, "Triagens recuperadas com sucesso."));
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetById(int id)
//         {
//             var triagem = await _triagemRepository.GetByIdAsync(id);
//             if (triagem == null)
//                 return NotFound(ResultWrapper<Triagem>.Erro("Triagem não encontrada."));

//             return Ok(ResultWrapper<Triagem>.Ok(triagem, "Triagem recuperada com sucesso."));
//         }

//         [HttpPost]
//         public async Task<IActionResult> Create([FromBody] TriagemCadastroDto dto)
//         {
//             var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
//             if (paciente == null)
//                 return NotFound(ResultWrapper<Paciente>.Erro("Paciente informado não foi encontrado."));

//             var triagem = new Triagem
//             {
//                 IdPaciente = dto.IdPaciente,
//                 ResponsavelTriagem = dto.ResponsavelTriagem,
//                 Pressao = dto.Pressao,
//                 Temperatura = dto.Temperatura,
//                 FrequenciaCardiaca = dto.FrequenciaCardiaca,
//                 Saturacao = dto.Saturacao,
//                 EscalaDor = dto.EscalaDor,
//                 Risco = dto.Risco,
//                 Queixa = dto.Queixa,
//                 Alergias = dto.Alergias,
//                 Observacoes = dto.Observacoes,
//                 Internacao = dto.Internacao
//             };

//             await _triagemRepository.AddAsync(triagem);

//             // Se as informações de convênio foram enviadas na triagem, vincula na tabela paciente_convenio
//             if (dto.IdConvenio.HasValue && dto.IdConvenio.Value > 0)
//             {
//                 var pacienteConvenio = new PacienteConvenio
//                 {
//                     IdPaciente = dto.IdPaciente,
//                     IdConvenio = dto.IdConvenio.Value,
//                     NumeroCarteira = dto.NumeroCarteira,
//                     Validade = dto.ValidadeConvenio.HasValue
//                         ? DateOnly.FromDateTime(dto.ValidadeConvenio.Value)
//                         : DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
//                     Ativo = 1
//                 };

//                 await _pacienteConvenioRepository.AddAsync(pacienteConvenio);
//             }

//             await _triagemRepository.SaveChangesAsync();

//             return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Triagem registrada com sucesso", idTriagem = triagem.IdTriagem }, "Triagem registrada com sucesso"));
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> Update(int id, [FromBody] TriagemCadastroDto dto)
//         {
//             var triagem = await _triagemRepository.GetByIdAsync(id);
//             if (triagem == null)
//                 return NotFound(ResultWrapper<Triagem>.Erro("Triagem não encontrada."));

//             triagem.ResponsavelTriagem = dto.ResponsavelTriagem;
//             triagem.Pressao = dto.Pressao;
//             triagem.Temperatura = dto.Temperatura;
//             triagem.FrequenciaCardiaca = dto.FrequenciaCardiaca;
//             triagem.Saturacao = dto.Saturacao;
//             triagem.EscalaDor = dto.EscalaDor;
//             triagem.Risco = dto.Risco;
//             triagem.Queixa = dto.Queixa;
//             triagem.Alergias = dto.Alergias;
//             triagem.Observacoes = dto.Observacoes;
//             triagem.Internacao = dto.Internacao;

//             _triagemRepository.Update(triagem);
//             await _triagemRepository.SaveChangesAsync();

//             return Ok(ResultWrapper<object>.Ok(new { mensagem = "Triagem atualizada com sucesso." }, "Triagem atualizada com sucesso."));
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var triagem = await _triagemRepository.GetByIdAsync(id);
//             if (triagem == null)
//                 return NotFound(ResultWrapper<Triagem>.Erro("Triagem não encontrada."));

//             _triagemRepository.Delete(triagem);
//             await _triagemRepository.SaveChangesAsync();

//             return Ok(ResultWrapper<string>.Ok(string.Empty, "Triagem removida com sucesso."));
//         }
//     }
// }