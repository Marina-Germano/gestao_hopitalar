class Prontuario {
  final int? idPaciente;
  final int? idTriagem;
  final int? idMedico;
  final int? idSala;
  final String? riscoEvasao;
  final String? evolucao;
  final String? statusProntuario;

  Prontuario({
    this.idPaciente,
    this.idTriagem,
    this.idMedico,
    this.idSala,
    this.riscoEvasao,
    this.evolucao,
    this.statusProntuario,
  });

  Map<String, dynamic> toJsonCadastroDto() {
    return {
      'idPaciente': idPaciente,
      'idTriagem': idTriagem,
      'idMedico': idMedico,
      'idSala': idSala,
      'riscoEvasao': riscoEvasao ?? 'BAIXO',
      'evolucao': evolucao ?? '',
      'statusProntuario': statusProntuario ?? 'ATIVO'
    };
  }
}