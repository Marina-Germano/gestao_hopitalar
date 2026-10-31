
import 'entitie.dart';

class Medicamento extends Entitie {
  int idAlmoxarifado;
  String? principioAtivo;
  String? contraindicacoes;

  Medicamento({
    super.id,
    required this.idAlmoxarifado,
    this.principioAtivo,
    this.contraindicacoes,
  });

  Map<String, dynamic> toJson() {
    return {
      'idMedicamento': id,
      'idAlmoxarifado': idAlmoxarifado,
      'principioAtivo': principioAtivo,
      'contraindicacoes': contraindicacoes,
    };
  }

  factory Medicamento.fromJson(Map<String, dynamic> json) {
    return Medicamento(
      id: json['idMedicamento'],
      idAlmoxarifado: json['idAlmoxarifado'] ?? 0,
      principioAtivo: json['principioAtivo'],
      contraindicacoes: json['contraindicacoes'],
    );
  }
}
