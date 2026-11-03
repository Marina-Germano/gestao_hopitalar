class Triagem {
  final int? id;
  final int? idPaciente;
  final String? cpfPaciente; // 🟢 Novo campo adicionado para a API
  final String? pressao;
  final double? temperatura;
  final int? frequenciaCardiaca;
  final int? saturacao;
  final String? queixa;
  final int? escalaDor;
  final String? alergias;
  final String? observacoes;
  final String? risco;
  final String? internacao;

  Triagem({
    this.id,
    this.idPaciente,
    this.cpfPaciente,
    this.pressao,
    this.temperatura,
    this.frequenciaCardiaca,
    this.saturacao,
    this.queixa,
    this.escalaDor,
    this.alergias,
    this.observacoes,
    this.risco,
    this.internacao,
  });

  // 🟢 Converte a entidade para o JSON exato que o TriagemCadastroDto (C#) espera
  Map<String, dynamic> toJsonCadastroDto() {
    return {
      "cpfPaciente": cpfPaciente ?? "",
      "pressaoArterial": pressao ?? "",
      "temperatura": temperatura ?? 0.0,
      "frequenciaCardiaca": frequenciaCardiaca ?? 0,
      "saturacao": saturacao ?? 0,
      "queixaPrincipal": queixa ?? "",
      "escalaDor": escalaDor ?? 0,
      "alergiasRelatadas": alergias ?? "",
      "observacoes": observacoes ?? "",
      "classificacaoRisco": risco ?? "VERDE"
    };
  }
}