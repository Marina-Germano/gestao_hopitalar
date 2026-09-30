// using GestaoHospitalarApi.Application.DTOs;
// using GestaoHospitalarApi.Domain.Repositories;
// using GestaoHospitalarApi.Application.Wrappers;
// using GestaoHospitalarApi.Models;
// using Microsoft.AspNetCore.Mvc;

// namespace GestaoHospitalarApi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class LeitosController : ControllerBase
//     {
//         private readonly IGenericRepository<Leito> _repository;

//         public LeitosController(IGenericRepository<Leito> repository)
//         {
//             _repository = repository;
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetAll()
//         {
//             var leitos = await _repository.GetAllAsync();
//             return Ok(ResultWrapper<IEnumerable<Leito>>.Ok(leitos, "Leitos recuperados com sucesso."));
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetById(int id)
//         {
//             var leito = await _repository.GetByIdAsync(id);
//             if (leito == null)
//                 return NotFound(ResultWrapper<Leito>.Erro("Leito não encontrado."));

//             return Ok(ResultWrapper<Leito>.Ok(leito, "Leito recuperado com sucesso."));
//         }

//         [HttpPost]
//         public async Task<IActionResult> Create([FromBody] LeitoCadastroDto dto)
//         {
//             var leito = new Leito
//             {
//                 Numero = dto.Numero,
//                 Ala = dto.Ala,
//                 Andar = dto.Andar,
//                 DataHigienizacao = dto.DataHigienizacao ?? DateTime.Now,
//                 Situacao = string.IsNullOrEmpty(dto.Situacao) ? "VAGO" : dto.Situacao.ToUpper()
//             };

//             await _repository.AddAsync(leito);
//             await _repository.SaveChangesAsync();

//             return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Leito cadastrado com sucesso", idLeito = leito.IdLeito }, "Leito cadastrado com sucesso"));
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> Update(int id, [FromBody] LeitoCadastroDto dto)
//         {
//             var leito = await _repository.GetByIdAsync(id);
//             if (leito == null)
//                 return NotFound(ResultWrapper<Leito>.Erro("Leito não encontrado."));

//             leito.Numero = dto.Numero;
//             leito.Ala = dto.Ala;
//             leito.Andar = dto.Andar;
//             leito.DataHigienizacao = dto.DataHigienizacao ?? leito.DataHigienizacao;
//             leito.Situacao = dto.Situacao.ToUpper();

//             _repository.Update(leito);
//             await _repository.SaveChangesAsync();

//             return Ok(ResultWrapper<object>.Ok(new { mensagem = "Leito atualizado com sucesso." }, "Leito atualizado com sucesso."));
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> Delete(int id)
//         {
//             var leito = await _repository.GetByIdAsync(id);
//             if (leito == null)
//                 return NotFound(ResultWrapper<Leito>.Erro("Leito não encontrado."));

//             _repository.Delete(leito);
//             await _repository.SaveChangesAsync();

//             return Ok(ResultWrapper<string>.Ok(string.Empty, "Leito removido com sucesso."));
//         }
//     }
// }