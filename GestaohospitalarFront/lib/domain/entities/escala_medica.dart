
class EscalaMedica {
  final int? id;
  final int? idMedico;
  final String? crmMedico;
  final String? nomeMedico;
  final String dataEscala;
  final String horaInicio;
  final String horaFim;
  final bool isPlantao;

  EscalaMedica({
    this.id,
    this.idMedico,
    this.crmMedico,
    this.nomeMedico,
    required this.dataEscala,
    required this.horaInicio,
    required this.horaFim,
    this.isPlantao = false,
  });

  factory EscalaMedica.fromJson(Map<String, dynamic> json) {
    final plantao = json['plantao'];

    return EscalaMedica(
      id: json['idEscala'] as int?,
      idMedico: json['idMedico'] as int?,
      crmMedico: json['crmMedico']?.toString(),
      nomeMedico: json['nomeMedico']?.toString(),
      dataEscala: json['dataEscala']?.toString() ?? '',
      horaInicio: json['horaInicio']?.toString() ?? '',
      horaFim: json['horaFim']?.toString() ?? '',
      isPlantao: json['isPlantao'] == true || plantao == 1,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'crmMedico': crmMedico,
      'dataEscala': dataEscala,
      'horaInicio': horaInicio,
      'horaFim': horaFim,
      'plantao': isPlantao ? 1 : 0,
    };
  }
}
