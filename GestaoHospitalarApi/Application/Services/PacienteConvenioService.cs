using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class PacienteConvenioService : IPacienteConvenioService
    {
        private readonly IPacienteConvenioRepository _repository;

        public PacienteConvenioService(IPacienteConvenioRepository repository)
        {
            _repository = repository;
        }

        public async Task CadastrarAsync(PacienteConvenioCadastroDto dto)
        {
            var pc = new PacienteConvenio
            {
                IdPaciente = dto.IdPaciente,
                IdConvenio = dto.IdConvenio,
                NumeroCarteira = dto.NumeroCarteira,
                Validade = DateOnly.FromDateTime(dto.Validade),
                Ativo = dto.Ativo
            };
            await _repository.AddAsync(pc);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<PacienteConvenioListagemDto>> ObterPorPacienteAsync(int idPaciente)
        {
            var lista = await _repository.ObterPorPacienteAsync(idPaciente);
            return lista.Select(pc => new PacienteConvenioListagemDto
            {
                IdPacienteConvenio = pc.IdPacienteConvenio,
                IdPaciente = pc.IdPaciente,
                IdConvenio = pc.IdConvenio,
                NomePaciente = pc.IdPacienteNavigation?.IdPessoaNavigation?.Nome ?? "",
                // Troque "Nome" pela propriedade equivalente se o seu Convenio usar outro campo, ex: RazaoSocial
                NomeConvenio = pc.IdConvenioNavigation?.NomeConvenio ?? "", 
                NumeroCarteira = pc.NumeroCarteira,
                Validade = pc.Validade.ToDateTime(TimeOnly.MinValue),
                Ativo = pc.Ativo ?? 0
            });
        }

        public async Task InativarAsync(int id)
        {
            var pc = await _repository.GetByIdAsync(id);
            if (pc != null)
            {
                pc.Ativo = 0; // Desativa em vez de deletar fisicamente
                _repository.Update(pc);
                await _repository.SaveChangesAsync();
            }
        }
    }
}