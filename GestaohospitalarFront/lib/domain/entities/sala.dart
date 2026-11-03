
import 'entitie.dart';

class Sala extends Entitie {
  String? nomeSala;
  String? tipo;
  String status;
  int? idAla;
  String? nomeAla;

  Sala({
    super.id,
    this.nomeSala,
    this.tipo,
    this.status = 'LIVRE',
    this.idAla,
    this.nomeAla,
  });

  Map<String, dynamic> toJson() {
    return {
      if (id != null) 'idSala': id,
      'nome': nomeSala,
      'tipo': tipo,
      'status': status,
      if (idAla != null) 'idAla': idAla,
    };
  }

  factory Sala.fromJson(Map<String, dynamic> json) {
    return Sala(
      id: json['idSala'] ?? json['id'],
      nomeSala: json['nome'] ?? json['nomeSala'],
      tipo: json['tipo']?.toString(),
      status: json['status'] ?? 'LIVRE',
      idAla: json['idAla'],
      nomeAla: json['nomeAla'],
    );
  }
}
