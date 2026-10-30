using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class PrescricaoService : IPrescricaoService
    {
        private readonly IPrescricaoRepository _prescricaoRepository;

        public PrescricaoService(IPrescricaoRepository prescricaoRepository)
        {
            _prescricaoRepository = prescricaoRepository;
        }

        public async Task CadastrarAsync(PrescricaoCadastroDto dto)
        {
            var prescricao = new Prescricao
            {
                IdProntuario = dto.IdProntuario,
                IdMedico = dto.IdMedico,
                IdMedicamento = dto.IdMedicamento,
                Dosagem = dto.Dosagem,
                Aplicacao = dto.Aplicacao,
                Horario = dto.Horario,
                Observacao = dto.Observacao
            };

            await _prescricaoRepository.AddAsync(prescricao);
            await _prescricaoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<PrescricaoListagemDto>> ObterPorProntuarioAsync(int idProntuario)
        {
            var prescricoes = await _prescricaoRepository.ObterPorProntuarioAsync(idProntuario);
            return prescricoes.Select(p => new PrescricaoListagemDto
            {
                IdPrescricao = p.IdPrescricao,
                IdProntuario = p.IdProntuario,
                IdMedico = p.IdMedico,
                NomeMedico = p.IdMedicoNavigation?.IdUsuarioNavigation?.IdPessoaNavigation?.Nome ?? "Médico Desconhecido",
                IdMedicamento = p.IdMedicamento,
                NomeMedicamento = p.IdMedicamentoNavigation?.IdAlmoxarifadoNavigation?.Nome ?? "Medicamento Desconhecido",
                Dosagem = p.Dosagem,
                Aplicacao = p.Aplicacao,
                Horario = p.Horario,
                Observacao = p.Observacao
            });
        }

        public async Task<PrescricaoListagemDto> ObterPorIdAsync(int id)
        {
            var p = await _prescricaoRepository.GetByIdAsync(id);
            if (p == null) throw new Exception("Prescrição não encontrada.");
            
            return new PrescricaoListagemDto
            {
                IdPrescricao = p.IdPrescricao, IdProntuario = p.IdProntuario, IdMedico = p.IdMedico,
                IdMedicamento = p.IdMedicamento, Dosagem = p.Dosagem, Aplicacao = p.Aplicacao,
                Horario = p.Horario, Observacao = p.Observacao
            };
        }

        public async Task AtualizarAsync(int id, PrescricaoCadastroDto dto)
        {
            var p = await _prescricaoRepository.GetByIdAsync(id);
            if (p == null) throw new Exception("Prescrição não encontrada.");

            p.IdMedico = dto.IdMedico; p.IdMedicamento = dto.IdMedicamento;
            p.Dosagem = dto.Dosagem; p.Aplicacao = dto.Aplicacao;
            p.Horario = dto.Horario; p.Observacao = dto.Observacao;

            _prescricaoRepository.Update(p);
            await _prescricaoRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var p = await _prescricaoRepository.GetByIdAsync(id);
            if (p != null)
            {
                _prescricaoRepository.Delete(p);
                await _prescricaoRepository.SaveChangesAsync();
            }
        }
    }
}