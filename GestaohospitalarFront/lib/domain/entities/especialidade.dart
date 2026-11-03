
import 'entitie.dart';

class Especialidade extends Entitie {
  final String? descricaoEspecialidade;

  Especialidade({
    super.id,
    this.descricaoEspecialidade,
  });

  // Converte de Dart para JSON
  Map<String, dynamic> toJson() {
    return {
      'idEspecialidade': id,
      'descricaoEspecialidade': descricaoEspecialidade,
    };
  }

  // Converte de JSON para Dart
  factory Especialidade.fromJson(Map<String, dynamic> json) {
    return Especialidade(
      id: json['idEspecialidade'],
      descricaoEspecialidade: json['descricaoEspecialidade'],
    );
  }
}