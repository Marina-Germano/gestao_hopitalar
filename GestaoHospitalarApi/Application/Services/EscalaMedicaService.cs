using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Application.DTOs.Escala;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Application.Services
{
    public class EscalaMedicaService : IEscalaMedicaService
    {
        private readonly IEscalaMedicaRepository _repository;
        private readonly AppDbContext _context;

        public EscalaMedicaService(IEscalaMedicaRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<EscalaMedicaListagemDto>> ObterPorMesAnoAsync(int ano, int mes)
        {
            var escalas = await _repository.ObterPorMesAnoAsync(ano, mes);
            return await MapearComNomeMedicoAsync(escalas);
        }

        public async Task<IEnumerable<EscalaMedicaListagemDto>> ObterTodasAsync()
        {
            var escalas = await _repository.ObterTodasComMedicoAsync();
            return await MapearComNomeMedicoAsync(escalas);
        }

        public async Task<EscalaMedicaListagemDto?> ObterPorIdAsync(int idEscala)
        {
            var escala = await _repository.ObterPorIdComMedicoAsync(idEscala);
            if (escala == null) return null;
            var lista = await MapearComNomeMedicoAsync(new[] { escala });
            return lista.FirstOrDefault();
        }

        public async Task<int> CadastrarAsync(EscalaMedicaCadastroDto dto)
        {
            var medico = await _context.Medicos
                .FirstOrDefaultAsync(m => m.Crm == dto.CrmMedico || m.Crm == dto.CrmMedico);

            if (medico == null)
                throw new ArgumentException($"Médico com CRM {dto.CrmMedico} não foi localizado.");

            var entidade = new EscalaMedica
            {
                IdMedico = medico.IdMedico,
                DataEscala = DateOnly.FromDateTime(dto.DataEscala),
                HoraInicio = TimeOnly.Parse(dto.HoraInicio),
                HoraFim = TimeOnly.Parse(dto.HoraFim),
                IsPlantao = dto.Plantao
            };

            await _repository.AddAsync(entidade);
            return entidade.IdEscala;
        }

        public async Task<bool> AtualizarAsync(int idEscala, EscalaMedicaCadastroDto dto)
        {
            var escala = await _context.EscalaMedicas.FindAsync(idEscala);
            if (escala == null) return false;

            var medico = await _context.Medicos
                .FirstOrDefaultAsync(m => m.Crm == dto.CrmMedico);

            if (medico != null)
            {
                escala.IdMedico = medico.IdMedico;
            }

            escala.DataEscala = DateOnly.FromDateTime(dto.DataEscala);
            escala.HoraInicio = TimeOnly.Parse(dto.HoraInicio);
            escala.HoraFim = TimeOnly.Parse(dto.HoraFim);
            escala.IsPlantao = dto.Plantao;

            return await _repository.AtualizarAsync(escala);
        }

        public async Task<bool> ExcluirAsync(int idEscala)
        {
            return await _repository.ExcluirAsync(idEscala);
        }

        private async Task<List<EscalaMedicaListagemDto>> MapearComNomeMedicoAsync(IEnumerable<EscalaMedica> escalas)
        {
            // Busca a junção de Medico -> Usuario -> Pessoa para obter o Nome real do médico
            var medicosInfo = await (from m in _context.Medicos
                                    join u in _context.Usuarios on m.IdUsuario equals u.IdUsuario
                                    join p in _context.Pessoas on u.IdPessoa equals p.IdPessoa
                                    select new
                                    {
                                        m.IdMedico,
                                        m.Crm,
                                        NomeMedico = p.Nome
                                    }).ToListAsync();

            return escalas.Select(e =>
            {
                var medico = medicosInfo.FirstOrDefault(m => m.IdMedico == e.IdMedico);

                return new EscalaMedicaListagemDto
                {
                    IdEscala = e.IdEscala,
                    IdMedico = e.IdMedico,
                    CrmMedico = medico?.Crm ?? string.Empty,
                    NomeMedico = medico?.NomeMedico ?? "Médico",
                    DataEscala = e.DataEscala.ToDateTime(TimeOnly.MinValue),
                    HoraInicio = e.HoraInicio.ToString("HH:mm"),
                    HoraFim = e.HoraFim.ToString("HH:mm"),
                    Plantao = e.IsPlantao ?? 0
                };
            }).ToList();
        }
    }
}
