import 'dart:convert';
import 'package:http/http.dart' as http;
// Importe seus arquivos corretamente de acordo com as pastas do seu projeto
import '../entities/ala.dart';
import '../../data/resources/api_response.dart';

class AlaService {
  // ATENÇÃO À URL:
  // Se estiver testando no Emulador Android, use '10.0.2.2'.
  // Se for Web, iOS ou Desktop, use 'localhost' ou '127.0.0.1'.
  // Substitua a porta '5000' pela porta correta que aparece no seu console da API C#.
  static const String baseUrl = 'http://localhost:5111/api/Ala';

  // ==========================================
  // GET: Obter todas as Alas (Listagem)
  // ==========================================
  Future<List<Ala>> getAllAlas() async {
    try {
      final response = await http.get(Uri.parse(baseUrl));

      if (response.statusCode == 200) {
        // Desempacota a resposta ResultWrapper da sua API
        final jsonResponse = jsonDecode(response.body);
        final apiResponse = ApiResponse<List<Ala>>.fromJson(
          jsonResponse,
          (dados) => (dados as List).map((item) => Ala.fromJson(item)).toList(),
        );

        return apiResponse.dados ?? [];
      } else {
        throw Exception(
            'Erro ao carregar alas da API. Status: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Erro de conexão com o servidor: $e');
    }
  }

  // ==========================================
  // POST: Cadastrar nova Ala
  // ==========================================
  Future<bool> createAla(Ala ala) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/cadastrar'),
        headers: {'Content-Type': 'application/json'},
        // O toJson() que criamos agora entra em ação!
        body: jsonEncode(ala.toJson()),
      );

      return response.statusCode == 200 || response.statusCode == 201;
    } catch (e) {
      throw Exception('Erro ao cadastrar ala: $e');
    }
  }

  // ==========================================
  // PUT: Atualizar Ala existente
  // ==========================================
  Future<bool> updateAla(int id, Ala ala) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/$id'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(ala.toJson()),
      );

      return response.statusCode == 200;
    } catch (e) {
      throw Exception('Erro ao atualizar ala: $e');
    }
  }

  // ==========================================
  // DELETE: Excluir Ala
  // ==========================================
  Future<bool> deleteAla(int id) async {
    try {
      final response = await http.delete(Uri.parse('$baseUrl/$id'));

      return response.statusCode == 200;
    } catch (e) {
      throw Exception('Erro ao excluir ala: $e');
    }
  }
}
