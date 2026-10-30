
import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

class AuthService {
  // Para executar no Windows, use localhost.
  // No emulador Android, troque localhost por 10.0.2.2.
  static const String baseUrl = 'http://localhost:5111/api/Usuarios';

  Future<String?> autenticarMedico(String crm, String senha) async {
    final uri = Uri.parse('$baseUrl/login');

    try {
      final response = await http
          .post(
            uri,
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/json',
            },
            body: jsonEncode({
              'login': crm.trim(),
              'senha': senha,
            }),
          )
          .timeout(const Duration(seconds: 15));

      debugPrint('Login API - status: ${response.statusCode}');

      if (response.statusCode == 401) {
        return null;
      }

      if (response.statusCode != 200) {
        throw Exception(
          'A API retornou o erro ${response.statusCode}.',
        );
      }

      final Map<String, dynamic> resposta =
          jsonDecode(response.body) as Map<String, dynamic>;

      // A API retorna o token dentro de "dados".
      final dados = resposta['dados'] ?? resposta['Dados'];

      if (dados is Map) {
        final token = dados['token'] ?? dados['Token'];

        if (token is String && token.isNotEmpty) {
          return token;
        }
      }

      throw Exception(
        'A API respondeu, mas não retornou o token esperado.',
      );
    } catch (e) {
      debugPrint('Erro ao autenticar médico: $e');
      rethrow;
    }
  }
}
