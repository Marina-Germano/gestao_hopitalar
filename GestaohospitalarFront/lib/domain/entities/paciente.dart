// lib/domain/entities/paciente.dart
import 'entitie.dart';

class Paciente extends Entitie {
  final int? ativo;
  final String nome;
  final String? cpf;
  final String? sexo;
  final DateTime? nascimento;
  final String? alergias;
  final String? tipoSanguineo;
  final String? historicoClinico;
  final String? telefone;
  final String? rua;
  final int? numeroCasa;
  final String? bairro;
  final String? cidade;
  final String? estado;
  final String? cep;
  final String? nomeResponsavel;

  Paciente({
    super.id,
    this.ativo = 1,
    required this.nome,
    this.cpf,
    this.sexo,
    this.nascimento,
    this.alergias,
    this.tipoSanguineo,
    this.historicoClinico,
    this.telefone,
    this.rua,
    this.numeroCasa,
    this.bairro,
    this.cidade,
    this.estado,
    this.cep,
    this.nomeResponsavel,
  });

  Paciente copyWith({
    int? id,
    int? ativo,
    String? nome,
    String? cpf,
    String? sexo,
    DateTime? nascimento,
    String? alergias,
    String? tipoSanguineo,
    String? historicoClinico,
    String? telefone,
    String? rua,
    int? numeroCasa,
    String? bairro,
    String? cidade,
    String? estado,
    String? cep,
    String? nomeResponsavel,
  }) {
    return Paciente(
      id: id ?? this.id,
      ativo: ativo ?? this.ativo,
      nome: nome ?? this.nome,
      cpf: cpf ?? this.cpf,
      sexo: sexo ?? this.sexo,
      nascimento: nascimento ?? this.nascimento,
      alergias: alergias ?? this.alergias,
      tipoSanguineo: tipoSanguineo ?? this.tipoSanguineo,
      historicoClinico: historicoClinico ?? this.historicoClinico,
      telefone: telefone ?? this.telefone,
      rua: rua ?? this.rua,
      numeroCasa: numeroCasa ?? this.numeroCasa,
      bairro: bairro ?? this.bairro,
      cidade: cidade ?? this.cidade,
      estado: estado ?? this.estado,
      cep: cep ?? this.cep,
      nomeResponsavel: nomeResponsavel ?? this.nomeResponsavel,
    );
  }

  /// Converte o paciente para o formato esperado pela API.
  Map<String, dynamic> toJson({
    String? nomeConvenio,
    String? numeroCarteira,
    DateTime? validade,
  }) {
    return {
      'nome': nome,
      'cpf': cpf ?? '',
      'telefone': telefone,
      'dataNascimento': nascimento?.toIso8601String(),
      'sexo': sexo ?? '',
      'cep': cep ?? '',
      'rua': rua ?? '',
      'numero': numeroCasa?.toString() ?? '',
      'bairro': bairro ?? '',
      'cidade': cidade ?? '',
      'estado': estado ?? '',
      'alergias': alergias,
      'tipoSanguineo': tipoSanguineo,
      'historicoClinico': historicoClinico,
      'nomeResponsavel': nomeResponsavel,
      'convenio': (nomeConvenio != null &&
              nomeConvenio.trim().isNotEmpty)
          ? {
              'nomeConvenio': nomeConvenio,
              'numero': numeroCarteira ?? '',
              'validade': validade?.toIso8601String(),
            }
          : null,
    };
  }

  /// Converte os dados recebidos da API em Paciente.
  factory Paciente.fromJson(
    Map<String, dynamic> json, {
    int? idOverride,
    int? ativoOverride,
  }) {
    final nascimentoJson =
        json['dataNascimento'] ?? json['nascimento'];

    return Paciente(
      id: idOverride ??
          _toInt(json['id'] ?? json['idPaciente'] ?? json['id_paciente']),
      ativo: ativoOverride ?? _toInt(json['ativo']) ?? 1,
      nome: json['nome']?.toString() ?? '',
      cpf: json['cpf']?.toString(),
      sexo: json['sexo']?.toString(),
      nascimento: nascimentoJson != null
          ? DateTime.tryParse(nascimentoJson.toString())
          : null,
      alergias: json['alergias']?.toString(),
      tipoSanguineo:
          (json['tipoSanguineo'] ?? json['tipo_sanguineo'])?.toString(),
      historicoClinico:
          (json['historicoClinico'] ?? json['historico_clinico'])?.toString(),
      telefone: json['telefone']?.toString(),
      rua: json['rua']?.toString(),
      numeroCasa: _toInt(json['numero'] ?? json['numeroCasa'] ?? json['numero_casa']),
      bairro: json['bairro']?.toString(),
      cidade: json['cidade']?.toString(),
      estado: json['estado']?.toString(),
      cep: json['cep']?.toString(),
      nomeResponsavel:
          (json['nomeResponsavel'] ?? json['nome_responsavel'])?.toString(),
    );
  }

  static int? _toInt(dynamic value) {
    if (value == null) return null;
    if (value is int) return value;
    return int.tryParse(value.toString());
  }

  // Mantidos temporariamente para compatibilidade com partes antigas do app.
  Map<String, dynamic> toMap() {
    return {
      'id_paciente': id,
      'ativo': ativo ?? 1,
      'nome': nome,
      'cpf': cpf,
      'sexo': sexo,
      'nascimento': nascimento?.toIso8601String(),
      'alergias': alergias,
      'tipo_sanguineo': tipoSanguineo,
      'historico_clinico': historicoClinico,
      'telefone': telefone,
      'rua': rua,
      'numero_casa': numeroCasa,
      'bairro': bairro,
      'cidade': cidade,
      'estado': estado,
      'cep': cep,
      'nome_responsavel': nomeResponsavel,
    };
  }

  factory Paciente.fromMap(Map<String, dynamic> map) {
    return Paciente.fromJson(map);
  }
}