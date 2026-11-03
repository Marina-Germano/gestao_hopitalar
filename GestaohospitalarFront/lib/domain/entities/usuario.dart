
import 'entitie.dart';

class Usuario extends Entitie {
  String nome;
  String? cpf;
  DateTime? nascimento;
  String? sexo;
  String? telefone;
  String? email;
  String? rua;
  int? numeroCasa;
  String? bairro;
  String? cidade;
  String? estado;
  String? cep;

  String login;
  String? senha;
  String perfil;

  Usuario({
    super.id,
    this.nome = '',
    this.cpf,
    this.nascimento,
    this.sexo,
    this.telefone,
    this.email,
    this.rua,
    this.numeroCasa,
    this.bairro,
    this.cidade,
    this.estado,
    this.cep,
    this.login = '',
    this.senha,
    this.perfil = '',
  });

  // ==========================================
  // JSON PARA CADASTRO E ATUALIZAÇÃO
  // ==========================================
  Map<String, dynamic> toJson() {
    return {
      'nome': nome,
      'cpf': cpf,
      'nascimento': nascimento?.toIso8601String(),
      'sexo': sexo,
      'telefone': telefone,
      'email': email,
      'rua': rua,
      'numeroCasa': numeroCasa,
      'bairro': bairro,
      'cidade': cidade,
      'estado': estado,
      'cep': cep,
      'login': login,
      'senha': senha ?? '',
      'perfil': perfil,
    };
  }

  // ==========================================
  // JSON RECEBIDO DA API
  // ==========================================
  factory Usuario.fromJson(Map<String, dynamic> json) {
    return Usuario(
      id: int.tryParse(
        (json['idUsuario'] ?? json['id'] ?? '').toString(),
      ),
      nome: json['nome']?.toString() ?? '',
      login: json['login']?.toString() ?? '',
      perfil: json['perfil']?.toString() ?? '',
      cpf: json['cpf']?.toString(),
      nascimento: json['nascimento'] != null
          ? DateTime.tryParse(json['nascimento'].toString())
          : null,
      sexo: json['sexo']?.toString(),
      telefone: json['telefone']?.toString(),
      email: json['email']?.toString(),
      rua: json['rua']?.toString(),
      numeroCasa: json['numeroCasa'] != null
          ? int.tryParse(json['numeroCasa'].toString())
          : null,
      bairro: json['bairro']?.toString(),
      cidade: json['cidade']?.toString(),
      estado: json['estado']?.toString(),
      cep: json['cep']?.toString(),
    );
  }
}
