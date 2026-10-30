
import 'entitie.dart';

class Medico extends Entitie {
  final int? ativo;
  final int? idEspecialidade;
  final String? nome;
  final String? telefone;
  final String? email;
  final String? crm;
  final String? cpf;
  final double? honorario;

  Medico({
    super.id,
    this.ativo = 1,
    this.idEspecialidade,
    this.nome,
    this.telefone,
    this.email,
    this.crm,
    this.cpf,
    this.honorario,
  });

  static int? _lerInt(dynamic valor) {
    if (valor == null) return null;
    if (valor is num) return valor.toInt();
    return int.tryParse(valor.toString());
  }

  static double? _lerDouble(dynamic valor) {
    if (valor == null) return null;
    if (valor is num) return valor.toDouble();
    return double.tryParse(valor.toString());
  }

  static String? _lerString(dynamic valor) {
    return valor == null ? null : valor.toString();
  }

  Medico copyWith({int? ativo}) {
    return Medico(
      id: id,
      ativo: ativo ?? this.ativo,
      idEspecialidade: idEspecialidade,
      nome: nome,
      telefone: telefone,
      email: email,
      crm: crm,
      cpf: cpf,
      honorario: honorario,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,  
      'id_medico': id,
      'ativo': ativo ?? 1,
      'id_especialidade': idEspecialidade,
      'nome': nome,
      'telefone': telefone,
      'email': email,
      'crm': crm,
      'cpf': cpf,
      'honorario': honorario,
    };
  }

  factory Medico.fromJson(Map<String, dynamic> json) {
    return Medico(
      id: _lerInt(
        json['id_medico'] ??
        json['idMedico'] ??
        json['IdMedico'] ??
        json['id']
      ),
      ativo: _lerInt(json['ativo'] ?? json['Ativo']) ?? 1,
      idEspecialidade: _lerInt(
        json['id_especialidade'] ??
        json['idEspecialidade'] ??
        json['IdEspecialidade']
      ),
      nome: _lerString(json['nome'] ?? json['Nome']),
      telefone: _lerString(json['telefone'] ?? json['Telefone']),
      email: _lerString(json['email'] ?? json['Email']),
      crm: _lerString(json['crm'] ?? json['Crm'] ?? json['CRM']),
      cpf: _lerString(json['cpf'] ?? json['Cpf'] ?? json['CPF']),
      honorario: _lerDouble(
        json['honorario'] ?? json['Honorario']
      ),
    );
  }
}
