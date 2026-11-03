import 'dart:convert';
import 'package:http/http.dart' as http;
import '../../data/resources/api_response.dart';

class ProntuarioService {
  final String baseUrl = "http://localhost:5111"; // Ajuste para sua URL base real (ApiConfig)

  Future<void> encaminharTriagem(int idTriagem, Map<String, dynamic> dto) async {
    final response = await http.post(
      Uri.parse('$baseUrl/api/Prontuario/encaminhar/$idTriagem'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(dto),
    );

    if (response.statusCode != 200 && response.statusCode != 201) {
      throw Exception("Erro ao encaminhar: ${response.body}");
    }
  }

  Future<void> cadastrarProntuario(Map<String, dynamic> dto) async {
    final response = await http.post(
      Uri.parse('$baseUrl/api/Prontuario/cadastrar'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(dto),
    );

    if (response.statusCode != 200 && response.statusCode != 201) {
      throw Exception("Erro ao cadastrar prontuário: ${response.body}");
    }
  }
}