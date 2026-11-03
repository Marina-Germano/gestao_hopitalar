class Leito {
  final int? id;
  final String? numero;
  final int? idAla;
  final String? nomeAla;
  final DateTime? dataHigienizacao;
  final String? situacao;
  final String? tipo;

  Leito({
    this.id,
    this.numero,
    this.idAla,
    this.nomeAla,
    this.dataHigienizacao,
    this.situacao,
    this.tipo,
  });

  Map<String, dynamic> toMap() {
    return {
      if (id != null) 'idLeito': id,
      'numero': numero,
      'idAla': idAla,
      'tipo': tipo ?? 'COMUM',
      'situacao': situacao ?? 'VAGO',
    };
  }

  factory Leito.fromJson(Map<String, dynamic> json) {
    return Leito(
      id: json['idLeito'] ?? json['id_leito'] ?? json['id'],
      numero: json['numero']?.toString() ?? json['numeroLeito']?.toString(),
      idAla: json['idAla'] ?? json['id_ala'],
      nomeAla: json['nomeAla'] ?? json['nome_ala'] ?? json['ala'] ?? json['nomeAlaNavigation'],
      situacao: json['situacao'] ?? json['situacaoLeito'] ?? json['status_leito'] ?? json['status'] ?? 'VAGO',
      tipo: json['tipo'] ?? json['tipoLeito'] ?? json['tipo_leito'] ?? 'COMUM',
      dataHigienizacao: json['dataHigienizacao'] != null
          ? DateTime.tryParse(json['dataHigienizacao'].toString())
          : (json['data_higienizacao'] != null
              ? DateTime.tryParse(json['data_higienizacao'].toString())
              : null),
    );
  }

  factory Leito.fromMap(Map<String, dynamic> map) => Leito.fromJson(map);

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is Leito && other.id == id;
  }

  @override
  int get hashCode => id.hashCode;
}