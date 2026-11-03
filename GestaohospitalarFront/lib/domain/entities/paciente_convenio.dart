class PacienteConvenio {
  final int? id;
  final int idPaciente;
  final int idConvenio;
  final String? numeroCarteira;
  final DateTime validade;
  final bool ativo;

  PacienteConvenio({
    this.id,
    required this.idPaciente,
    required this.idConvenio,
    this.numeroCarteira,
    required this.validade,
    this.ativo = true,
  });

  Map<String, dynamic> toJson() {
    return {
      'idPacienteConvenio': id,
      'idPaciente': idPaciente,
      'idConvenio': idConvenio,
      'numeroCarteira': numeroCarteira,
      'validade': validade.toIso8601String().split('T').first,
      'ativo': ativo,
    };
  }

  factory PacienteConvenio.fromJson(Map<String, dynamic> json) {
    return PacienteConvenio(
      id: json['idPacienteConvenio'] ?? json['id'],
      idPaciente: json['idPaciente'],
      idConvenio: json['idConvenio'],
      numeroCarteira: json['numeroCarteira'],
      validade: DateTime.parse(json['validade']),
      ativo: json['ativo'] == true || json['ativo'] == 1,
    );
  }
}