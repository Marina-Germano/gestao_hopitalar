using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IProntuarioService
    {
        Task<int> CadastrarAsync(ProntuarioCadastroDto dto);
        Task<int> EncaminharTriagemAsync(int idTriagem, TriagemEncaminhamentoDto dto);
    }
}