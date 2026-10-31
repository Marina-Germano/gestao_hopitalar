import 'entitie.dart';

class Exame extends Entitie {
  String nome;
  double? valor;
  String? descricao;

  Exame({
    super.id,
    required this.nome,
    this.valor,
    this.descricao,
  });

  // Usado para ENVIAR dados para a API no cadastro/atualização (POST/PUT)
  Map<String, dynamic> toJson() {
    return {
      'nome': nome,
      'valor': valor,
      'descricao': descricao,
    };
  }

  // Usado para RECEBER dados da API (GET)
  factory Exame.fromJson(Map<String, dynamic> json) {
    return Exame(
      id: json['idExame'] != null 
          ? int.tryParse(json['idExame'].toString()) 
          : (json['id'] != null ? int.tryParse(json['id'].toString()) : null),
      nome: json['nome']?.toString() ?? 'Sem Nome',
      valor: json['valor'] != null ? (json['valor'] as num).toDouble() : null,
      descricao: json['descricao']?.toString(),
    );
  }
}