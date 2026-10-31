
import 'dart:convert';
import 'package:http/http.dart' as http;

import '../entities/sala.dart';
import '../../data/resources/api_response.dart';

class SalaService {
  static const String baseUrl = 'http://localhost:5111/api/Sala';

  // ==========================================
  // POST / PUT: Salvar Sala
  // ==========================================
  Future<void> salvar(Sala sala) async {
    try {
      late http.Response response;

      if (sala.id == null) {
        response = await http.post(
          Uri.parse('$baseUrl/cadastrar'),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode(sala.toJson()),
        );
      } else {
        response = await http.put(
          Uri.parse('$baseUrl/${sala.id}'),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode(sala.toJson()),
        );
      }

      if (response.statusCode != 200 &&
          response.statusCode != 201 &&
          response.statusCode != 204) {
        throw Exception(
          'Erro ao salvar sala. Status: ${response.statusCode} - ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao salvar sala: $e');
    }
  }

  // ==========================================
  // DELETE: Excluir Sala
  // ==========================================
  Future<void> excluir(int id) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/$id'),
      );

      if (response.statusCode != 200 &&
          response.statusCode != 204) {
        throw Exception(
          'Erro ao excluir sala. Status: ${response.statusCode}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao excluir sala: $e');
    }
  }

  // ==========================================
  // GET: Listar todas as Salas
  // ==========================================
  Future<List<Sala>> listarTodas() async {
    try {
      final response = await http.get(Uri.parse(baseUrl));

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse = ApiResponse<List<Sala>>.fromJson(
          jsonResponse,
          (dados) => (dados as List)
              .map((item) => Sala.fromJson(item))
              .toList(),
        );

        return apiResponse.dados ?? [];
      } else {
        throw Exception(
          'Erro ao carregar salas. Status: ${response.statusCode}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao listar salas: $e');
    }
  }

  // ==========================================
  // PUT: Atualizar Status da Sala
  // ==========================================
  Future<void> atualizarStatusSala(int idSala, String status) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/$idSala/status'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'status': status,
        }),
      );

      if (response.statusCode != 200 &&
          response.statusCode != 204) {
        throw Exception(
          'Erro ao atualizar status da sala. Status: ${response.statusCode}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao atualizar status da sala: $e');
    }
  }

  // ==========================================
  // GET: Buscar Pacientes por Sala
  // ==========================================
  Future<List<Map<String, dynamic>>> buscarPacientesPorSala(
      int idSala) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/$idSala/pacientes'),
      );

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse =
            ApiResponse<List<Map<String, dynamic>>>.fromJson(
          jsonResponse,
          (dados) => (dados as List)
              .map((item) => Map<String, dynamic>.from(item))
              .toList(),
        );

        return apiResponse.dados ?? [];
      } else {
        throw Exception(
          'Erro ao buscar pacientes da sala. Status: ${response.statusCode}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao buscar pacientes por sala: $e');
    }
  }
}
