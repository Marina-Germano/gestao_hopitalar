
import 'dart:convert';
import 'package:http/http.dart' as http;

import '../entities/usuario.dart';
import '../../data/resources/api_response.dart';

class UsuarioService {
  static const String baseUrl = 'http://localhost:5111/api/Usuarios';

  // ==========================================
  // CABEÇALHOS COM AUTENTICAÇÃO JWT
  // ==========================================
  Map<String, String> _headers(String token) {
    return {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': 'Bearer $token',
    };
  }

  // ==========================================
  // GET: Listar todos os usuários
  // ==========================================
  Future<List<Usuario>> listarTodos(String token) async {
    try {
      final response = await http.get(
        Uri.parse(baseUrl),
        headers: _headers(token),
      );

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse = ApiResponse<List<Usuario>>.fromJson(
          jsonResponse,
          (dados) => (dados as List)
              .map((item) => Usuario.fromJson(
                    Map<String, dynamic>.from(item),
                  ))
              .toList(),
        );

        return apiResponse.dados ?? [];
      } else {
        throw Exception(
          'Erro ao carregar usuários. Status: ${response.statusCode} - ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao listar usuários: $e');
    }
  }

  // ==========================================
  // GET: Buscar usuário por ID
  // ==========================================
  Future<Usuario> buscarPorId(int id, String token) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/$id'),
        headers: _headers(token),
      );

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse = ApiResponse<Usuario>.fromJson(
          jsonResponse,
          (dados) => Usuario.fromJson(
            Map<String, dynamic>.from(dados),
          ),
        );

        if (apiResponse.dados == null) {
          throw Exception('Usuário não encontrado na resposta.');
        }

        return apiResponse.dados!;
      } else {
        throw Exception(
          'Erro ao buscar usuário. Status: ${response.statusCode} - ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao buscar usuário: $e');
    }
  }

  // ==========================================
  // POST / PUT: Salvar usuário
  // ==========================================
  Future<void> salvar(Usuario usuario, String token) async {
    try {
      late http.Response response;

      if (usuario.id == null) {
        response = await http.post(
          Uri.parse(baseUrl),
          headers: _headers(token),
          body: jsonEncode(usuario.toJson()),
        );
      } else {
        response = await http.put(
          Uri.parse('$baseUrl/${usuario.id}'),
          headers: _headers(token),
          body: jsonEncode(usuario.toJson()),
        );
      }

      if (response.statusCode != 200 &&
          response.statusCode != 201 &&
          response.statusCode != 204) {
        throw Exception(
          'Erro ao salvar usuário. Status: ${response.statusCode} - ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao salvar usuário: $e');
    }
  }

  // ==========================================
  // DELETE: Excluir usuário
  // ==========================================
  Future<void> excluir(int id, String token) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/$id'),
        headers: _headers(token),
      );

      if (response.statusCode != 200 &&
          response.statusCode != 204) {
        throw Exception(
          'Erro ao excluir usuário. Status: ${response.statusCode} - ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao excluir usuário: $e');
    }
  }
}
