import 'entitie.dart';

class Ala extends Entitie {
  @override
  final int? id;
  final String nomeAla;
  final String andar;
  final String tipo; 

  Ala({
    this.id,
    required this.nomeAla,
    required this.andar,
    this.tipo = 'COMUM',
  });

  // Usado para ENVIAR dados para a API no cadastro (POST)
  Map<String, dynamic> toJson() {
    return {
      'nomeAla': nomeAla,
      'andar': andar,
      // O 'tipo' e o 'id' não vão no JSON de cadastro pois o C# não pede e o banco gera
    };
  }

  // Usado para RECEBER dados da API (GET)
  factory Ala.fromJson(Map<String, dynamic> json) {
    return Ala(
      // A API C# (ASP.NET Core) converte a primeira letra do DTO para minúscula (camelCase)
      id: json['idAla'] != null ? int.tryParse(json['idAla'].toString()) : null,
      nomeAla: json['nomeAla']?.toString() ?? 'Sem Nome',
      andar: json['andar']?.toString() ?? 'Sem Andar',
      tipo: 'COMUM', 
    );
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is Ala && other.id == id;
  }

  @override
  int get hashCode => id.hashCode;
}