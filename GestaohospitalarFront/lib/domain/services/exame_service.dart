import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../entities/exame.dart';
import '../../data/resources/api_response.dart';

typedef JsonMap = Map<String, dynamic>;

class ExameService extends ChangeNotifier {
  // Ajustado para plural exato conforme o Swagger
  static const String baseUrl = 'http://localhost:5111/api/Exames';

  List<Exame> exames = [];
  bool isLoading = false;

  // ==========================================
  // GET: Obter todos os Exames (Listagem)
  // ==========================================
  Future<void> carregarExames() async {
    isLoading = true;
    notifyListeners();

    try {
      final response = await http.get(Uri.parse(baseUrl));

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);
        final apiResponse = ApiResponse<List<Exame>>.fromJson(
          jsonResponse,
          (dados) => (dados as List).map((item) => Exame.fromJson(item)).toList(),
        );

        exames = apiResponse.dados ?? [];
      } else {
        throw Exception('Erro ao carregar exames da API. Status: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Erro de conexão com o servidor: $e');
    } finally {
      isLoading = false;
      notifyListeners();
    }
  }

  // ==========================================
  // POST / PUT: Salvar Exame (Novo ou Atualizar)
  // ==========================================
  Future<void> salvarExame(Exame exame) async {
    try {
      http.Response response;

      if (exame.id == null) {
        // POST /api/Exames (Sem o /cadastrar)
        response = await http.post(
          Uri.parse(baseUrl),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode(exame.toJson()),
        );
      } else {
        // PUT /api/Exames/{id}
        response = await http.put(
          Uri.parse('$baseUrl/${exame.id}'),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode(exame.toJson()),
        );
      }

      if (response.statusCode == 200 || response.statusCode == 201) {
        await carregarExames();
      } else {
        throw Exception('Erro ao salvar exame. Status: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Erro ao salvar exame: $e');
    }
  }

  // ==========================================
  // DELETE: Excluir Exame
  // ==========================================
  Future<void> arquivarExame(Exame exame) async {
    try {
      // DELETE /api/Exames/{id}
      final response = await http.delete(Uri.parse('$baseUrl/${exame.id}'));

      if (response.statusCode == 200) {
        exames.removeWhere((e) => e.id == exame.id);
        notifyListeners();
      } else {
        throw Exception('Erro ao excluir exame. Status: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Erro ao excluir exame: $e');
    }
  }

  // ==========================================
  // MÉTODOS DE SOLICITAÇÃO
  // ==========================================

  Future<List<JsonMap>> buscarExamesPorProntuario(int idProntuario) async {
    try {
      final response = await http.get(
        Uri.parse('http://localhost:5111/api/SolicitacaoExame/prontuario/$idProntuario'),
      );

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);
        final apiResponse = ApiResponse<List<dynamic>>.fromJson(
          jsonResponse,
          (dados) => dados as List<dynamic>,
        );

        return (apiResponse.dados ?? [])
            .map((item) => JsonMap.from(item))
            .toList();
      } else {
        throw Exception('Erro ao buscar histórico de exames. Status: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Erro ao buscar histórico de exames: $e');
    }
  }

  Future<void> solicitarNovoExame({
    required int idProntuario,
    required int idExame,
    required int idMedico,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('http://localhost:5111/api/SolicitacaoExame'), // Ajustado caso precise também
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'idProntuario': idProntuario,
          'idExame': idExame,
          'idMedico': idMedico,
          'statusExame': 'SOLICITADO',
        }),
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        notifyListeners();
      } else {
        throw Exception('Erro ao solicitar exame. Status: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Erro ao salvar solicitação de exame: $e');
    }
  }
}