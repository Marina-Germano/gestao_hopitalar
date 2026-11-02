class Agendamento {
  final int? idAgendamento;
  final int? idPaciente;
  final int? idMedico;
  final int? idSala;
  final String? nomePaciente;
  final String? nomeMedico;
  final String? nomeSala;
  final DateTime dataHora;
  final String status;

  Agendamento({
    this.idAgendamento,
    this.idPaciente,
    this.idMedico,
    this.idSala,
    this.nomePaciente,
    this.nomeMedico,
    this.nomeSala,
    required this.dataHora,
    this.status = 'AGENDADO',
  });

  // Mapeia o JSON que vem do GET /api/agendamento (AgendamentoListagemDto)
  factory Agendamento.fromJson(Map<String, dynamic> json) {
    return Agendamento(
      idAgendamento: json['idAgendamento'],
      nomePaciente: json['nomePaciente'] ?? '',
      nomeMedico: json['nomeMedico'] ?? '',
      nomeSala: json['nomeSala'] ?? '',
      dataHora: json['dataHora'] != null
          ? DateTime.parse(json['dataHora'])
          : DateTime.now(),
      status: json['status'] ?? 'AGENDADO',
    );
  }

  // Mapeia para o JSON enviado no POST /api/agendamento/cadastrar (AgendamentoCadastroDto)
  Map<String, dynamic> toCadastroJson() {
    return {
      'idPaciente': idPaciente,
      'idMedico': idMedico,
      'idSala': idSala,
      'dataHora': dataHora.toIso8601String(),
      'status': status,
    };
  }
}