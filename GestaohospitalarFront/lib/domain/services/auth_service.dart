
import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

class AuthResultado {
  final String token;
  final String perfil;
  final String login;
  final int? idUsuario;

  AuthResultado({
    required this.token,
    required this.perfil,
    required this.login,
    this.idUsuario,
  });
}

class AuthSession {
  static String? token;
  static String? perfil;
  static String? login;
  static int? idUsuario;

  static void iniciar(AuthResultado resultado) {
    token = resultado.token;
    perfil = resultado.perfil;
    login = resultado.login;
    idUsuario = resultado.idUsuario;
  }

  static void encerrar() {
    token = null;
    perfil = null;
    login = null;
    idUsuario = null;
  }
}

class AuthService {
  static const String baseUrl = 'http://localhost:5111/api/Usuarios';

  Future<AuthResultado?> autenticar(String login, String senha) async {
    final uri = Uri.parse('$baseUrl/login');

    try {
      final response = await http.post(
        uri,
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        body: jsonEncode({
          'login': login.trim(),
          'senha': senha,
        }),
      ).timeout(const Duration(seconds: 15));

      debugPrint('Login API - status: ${response.statusCode}');

      if (response.statusCode == 401) {
        return null;
      }

      if (response.statusCode != 200) {
        throw Exception(
          'A API retornou o erro ${response.statusCode}.',
        );
      }

      final resposta =
          jsonDecode(response.body) as Map<String, dynamic>;

      final dados = resposta['dados'] ?? resposta['Dados'];

      if (dados is! Map) {
        throw Exception('Resposta da API inválida.');
      }

      final token = dados['token'] ?? dados['Token'];

      if (token is! String || token.isEmpty) {
        throw Exception('A API não retornou o token.');
      }

      final partes = token.split('.');

      if (partes.length != 3) {
        throw Exception('Token JWT inválido.');
      }

      final payload = base64Url.normalize(partes[1]);

      final claims = jsonDecode(
        utf8.decode(base64Url.decode(payload)),
      ) as Map<String, dynamic>;

      final perfil = (
        claims['role'] ??
        claims[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] ??
        ''
      ).toString().trim().toUpperCase();

      final loginToken = (
        claims['unique_name'] ??
        login
      ).toString();

      final id = int.tryParse(
        (claims['sub'] ?? '').toString(),
      );

      if (perfil != 'ADMIN' &&
          perfil != 'MEDICO' &&
          perfil != 'MÉDICO') {
        throw Exception('Perfil não reconhecido: $perfil');
      }

      return AuthResultado(
        token: token,
        perfil: perfil == 'MÉDICO' ? 'MEDICO' : perfil,
        login: loginToken,
        idUsuario: id,
      );
    } catch (e) {
      debugPrint('Erro ao autenticar: $e');
      rethrow;
    }
  }
}
