namespace GestaoHospitalarApi.Application.Wrappers
{
    public class ResultWrapper<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public T? Dados { get; set; }
        public List<string>? Erros { get; set; }

        public ResultWrapper() { }

        // Método estático para facilitar retornos de SUCESSO
        public static ResultWrapper<T> Ok(T dados, string mensagem = "")
        {
            return new ResultWrapper<T>
            {
                Sucesso = true,
                Mensagem = mensagem,
                Dados = dados
            };
        }

        // Método estático para facilitar retornos de ERRO
        public static ResultWrapper<T> Erro(string mensagem, List<string>? erros = null)
        {
            return new ResultWrapper<T>
            {
                Sucesso = false,
                Mensagem = mensagem,
                Erros = erros
            };
        }
    }
}