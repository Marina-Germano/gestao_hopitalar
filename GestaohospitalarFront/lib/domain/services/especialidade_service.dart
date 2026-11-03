import 'dart:convert';
import 'package:http/http.dart' as http;

import '../entities/especialidade.dart';
import '../../data/resources/api_response.dart';

class EspecialidadeService {
  static const String baseUrl =
      'http://localhost:5111/api/Especialidades';

  // GET: Listar especialidades
  Future<List<Especialidade>> listarEspecialidades() async {
    try {
      final response = await http.get(Uri.parse(baseUrl));

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse =
            ApiResponse<List<Especialidade>>.fromJson(
          jsonResponse,
          (dados) => (dados as List)
              .map(
                (item) => Especialidade.fromJson(
                  Map<String, dynamic>.from(item),
                ),
              )
              .toList(),
        );

        return apiResponse.dados ?? [];
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao carregar especialidades: $e');
    }
  }

  // POST: Cadastrar especialidade
  Future<bool> cadastrarEspecialidade(String descricao) async {
    try {
      final response = await http.post(
        Uri.parse(baseUrl),
        headers: {
          'Content-Type': 'application/json',
        },
        body: jsonEncode({
          'descricaoEspecialidade': descricao,
        }),
      );

      if (response.statusCode == 200 ||
          response.statusCode == 201) {
        return true;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao cadastrar especialidade: $e');
    }
  }
}