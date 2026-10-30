using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class FaturamentoService : IFaturamentoService
    {
        private readonly IFaturamentoRepository _faturamentoRepository;
        private readonly IInternacaoRepository _internacaoRepository;
        private readonly IConsumoItemRepository _consumoRepository;
        private readonly ISolicitacaoExameRepository _exameRepository;

        public FaturamentoService(
            IFaturamentoRepository faturamentoRepository,
            IInternacaoRepository internacaoRepository,
            IConsumoItemRepository consumoRepository,
            ISolicitacaoExameRepository exameRepository)
        {
            _faturamentoRepository = faturamentoRepository;
            _internacaoRepository = internacaoRepository;
            _consumoRepository = consumoRepository;
            _exameRepository = exameRepository;
        }

        public async Task GerarFaturamentoAltaAsync(int idInternacao)
        {
            // Verifica se já existe faturamento
            var existe = await _faturamentoRepository.ObterPorInternacaoAsync(idInternacao);
            if (existe != null) return;

            // Busca a internação com todos os vínculos (Médico, Paciente, Convênio, Leito)
            var internacao = await _internacaoRepository.ObterComDetalhesFaturamentoAsync(idInternacao);
            if (internacao == null) throw new Exception("Internação não encontrada.");

            // 1. Calcula Diárias e Tipos de Leito
            int dias = internacao.DataAlta.HasValue
                ? (internacao.DataAlta.Value - internacao.DataEntrada).Days
                : 1;
            if (dias <= 0) dias = 1;

            string tipoLeito = internacao.IdLeitoNavigation?.TipoLeito?.ToUpper() ?? "COMUM";
            decimal valorDiaria = tipoLeito switch
            {
                "PREMIUM" => 5000m,
                "PRIVADO" => 1000m,
                _ => 500m // COMUM
            };
            decimal totalInternacao = dias * valorDiaria;

            // 2. Honorários Médicos (Busca direto do cadastro do médico)
            // Certifique-se de que a classe Medico tenha a propriedade "Honorario"
            decimal totalHonorarios = internacao.IdProntuarioNavigation?.IdMedicoNavigation?.Honorario ?? 0m;

            // 3. Consumo de Itens e Medicamentos
            var consumos = await _consumoRepository.ObterPorInternacaoAsync(idInternacao);
            decimal totalMedicamentos = 0;
            decimal totalInsumos = 0;

            foreach (var c in consumos)
            {
                decimal valorItem = c.Quantidade * c.IdAlmoxarifadoNavigation.ValorUnitario;
                if (c.IdAlmoxarifadoNavigation.Categoria == "MEDICAMENTO")
                    totalMedicamentos += valorItem;
                else
                    totalInsumos += valorItem;
            }

            // 4. Exames Realizados
            var exames = await _exameRepository.ObterPorProntuarioAsync(internacao.IdProntuario);
            decimal totalExames = exames.Sum(e => e.IdExameNavigation?.Valor ?? 0);

            // 5. Soma Total e Regras da Tabela Paciente_Convenio
            decimal subTotal = totalMedicamentos + totalExames + totalInternacao + totalHonorarios + totalInsumos;
            decimal valorFinal = subTotal;
            string observacao = $"Alta processada. {dias} dia(s) no leito {tipoLeito}.";

            var paciente = internacao.IdProntuarioNavigation?.IdPacienteNavigation;
            
            // Busca o primeiro convênio que esteja ATIVO e na VALIDADE
            var vinculoAtivo = paciente?.PacienteConvenios?.FirstOrDefault(pc => pc.Ativo == 1 && pc.Validade >= DateOnly.FromDateTime(DateTime.Now));
            var convenio = vinculoAtivo?.IdConvenioNavigation;

            if (convenio != null)
            {
                if (!string.IsNullOrEmpty(convenio.TipoLeito) && convenio.TipoLeito?.ToUpper() == tipoLeito)
                {
                    decimal porcentagem = convenio.PercentualCobertura ?? 0m;
                    decimal desconto = subTotal * (porcentagem / 100m);
                    valorFinal -= desconto;
                    observacao += $" Convênio cobriu {porcentagem}% ({desconto:C}). Carteira: {vinculoAtivo.NumeroCarteira}.";
                }
                else
                {
                    observacao += $" ALERTA: Convênio cobre leito {convenio.TipoLeito}, mas paciente usou {tipoLeito}. Desconto não aplicado.";
                }
            }
            else
            {
                observacao += " Paciente particular (Sem convênio ativo/válido).";
            }

            // Cria o Faturamento
            var faturamento = new Faturamento
            {
                IdInternacao = idInternacao,
                TotalMedicamentos = totalMedicamentos,
                TotalExames = totalExames,
                TotalInternacao = totalInternacao,
                TotalHonorarios = totalHonorarios,
                TotalConsumo = totalInsumos,
                ValorTotal = valorFinal, // Aplica o desconto do convênio no total
                StatusPagamento = "PENDENTE",
                DataFechamento = DateTime.Now,
                Observacao = observacao
            };

            await _faturamentoRepository.AddAsync(faturamento);
            await _faturamentoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<FaturamentoListagemDto>> ObterTodosAsync()
        {
            var fat = await _faturamentoRepository.ObterTodosComDetalhesAsync();
            return fat.Select(f => new FaturamentoListagemDto
            {
                IdFaturamento = f.IdFaturamento,
                IdInternacao = f.IdInternacao,
                // Ajustado para buscar o nome do paciente conforme a sua estrutura
                NomePaciente = f.IdInternacaoNavigation?.IdProntuarioNavigation?.IdPacienteNavigation?.IdPessoaNavigation?.Nome 
                            ?? "",
                TotalMedicamentos = f.TotalMedicamentos ?? 0,
                TotalExames = f.TotalExames ?? 0,
                TotalInternacao = f.TotalInternacao ?? 0,
                TotalHonorarios = f.TotalHonorarios ?? 0,
                TotalConsumo = f.TotalConsumo ?? 0,
                ValorTotal = f.ValorTotal ?? 0,
                StatusPagamento = f.StatusPagamento ?? "PENDENTE",
                DataFechamento = f.DataFechamento,
                Observacao = f.Observacao
            });
        }

        public async Task PagarFaturamentoAsync(int idFaturamento)
        {
            var fat = await _faturamentoRepository.GetByIdAsync(idFaturamento);
            if (fat == null) throw new Exception("Faturamento não encontrado.");
            
            fat.StatusPagamento = "PAGO";
            _faturamentoRepository.Update(fat);
            await _faturamentoRepository.SaveChangesAsync();
        }
    }
}