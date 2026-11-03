import 'dart:convert';
import 'package:http/http.dart' as http;
import '../entities/triagem.dart';

class TriagemService {
  final String baseUrl = "http://localhost:5111";

  Future<int> salvarTriagem(Triagem triagem) async {
    final response = await http.post(
      Uri.parse('$baseUrl/api/Triagem/cadastrar'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(triagem.toJsonCadastroDto()),
    );

    if (response.statusCode == 200 || response.statusCode == 201) {
      final jsonResponse = jsonDecode(response.body);

      if (jsonResponse['sucesso'] == true) {
        return jsonResponse['dados'];
      } else {
        throw Exception(jsonResponse['mensagem']);
      }
    } else {
      throw Exception("Erro na API: ${response.body}");
    }
  }

  Future<List<Map<String, dynamic>>> buscarFilaTriagem() async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/Triagem'),
      headers: {'Content-Type': 'application/json'},
    );

    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);

      if (jsonResponse['sucesso'] == true) {
        final List<dynamic> dados = jsonResponse['dados'];

        return dados.map<Map<String, dynamic>>((item) {
          return {
            'id_triagem': item['idTriagem'],
            'id_paciente': item['idPaciente'],
            'nome': item['nomePaciente'],
            'cpf': item['cpfPaciente'],
            'risco': item['classificacaoRisco'],
            'queixa': item['queixaPrincipal'],
          };
        }).toList();
      }
    }

    return [];
  }
}