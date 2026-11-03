
import 'dart:convert';
import 'package:http/http.dart' as http;

import '../entities/escala_medica.dart';
import '../../data/resources/api_response.dart';

class EscalaService {
  static const String baseUrl =
      'http://localhost:5111/api/EscalaMedica';

  // Busca escalas de um mês específico
  Future<List<EscalaMedica>> buscarEscalaDoMes(
    int ano,
    int mes,
  ) async {
    final url = Uri.parse('$baseUrl/mes/$ano/$mes');

    final response = await http.get(url);

    if (response.statusCode == 200) {
      final json = jsonDecode(response.body);

      final resultado = ApiResponse<List<EscalaMedica>>.fromJson(
        json,
        (dados) => (dados as List)
            .map(
              (item) => EscalaMedica.fromJson(
                item as Map<String, dynamic>,
              ),
            )
            .toList(),
      );

      if (resultado.sucesso) {
        return resultado.dados ?? [];
      }

      throw Exception(
        resultado.mensagem ?? 'Erro ao buscar escalas.',
      );
    }

    throw Exception(
      'Erro ao buscar escalas: ${response.statusCode} - ${response.body}',
    );
  }

  // Cadastra escala
  Future<void> cadastrarEscala(EscalaMedica escala) async {
    final response = await http.post(
      Uri.parse('$baseUrl/cadastrar'),
      headers: {
        'Content-Type': 'application/json',
      },
      body: jsonEncode(escala.toJson()),
    );

    if (response.statusCode == 200 ||
        response.statusCode == 201) {
      final json = jsonDecode(response.body);

      final resultado = ApiResponse<dynamic>.fromJson(
        json,
        (dados) => dados,
      );

      if (!resultado.sucesso) {
        throw Exception(
          resultado.mensagem ?? 'Erro ao cadastrar escala.',
        );
      }

      return;
    }

    throw Exception(
      'Erro ao cadastrar escala: ${response.statusCode} - ${response.body}',
    );
  }

  // Atualiza escala
  Future<void> atualizarEscala(EscalaMedica escala) async {
    final response = await http.put(
      Uri.parse('$baseUrl/${escala.id}'),
      headers: {
        'Content-Type': 'application/json',
      },
      body: jsonEncode(escala.toJson()),
    );

    if (response.statusCode == 200) {
      final json = jsonDecode(response.body);

      final resultado = ApiResponse<dynamic>.fromJson(
        json,
        (dados) => dados,
      );

      if (!resultado.sucesso) {
        throw Exception(
          resultado.mensagem ?? 'Erro ao atualizar escala.',
        );
      }

      return;
    }

    throw Exception(
      'Erro ao atualizar escala: ${response.statusCode} - ${response.body}',
    );
  }

  // Exclui escala
  Future<void> excluirEscala(int id) async {
    final response = await http.delete(
      Uri.parse('$baseUrl/$id'),
    );

    if (response.statusCode == 200) {
      final json = jsonDecode(response.body);

      final resultado = ApiResponse<dynamic>.fromJson(
        json,
        (dados) => dados,
      );

      if (!resultado.sucesso) {
        throw Exception(
          resultado.mensagem ?? 'Erro ao excluir escala.',
        );
      }

      return;
    }

    throw Exception(
      'Erro ao excluir escala: ${response.statusCode} - ${response.body}',
    );
  }
}
