import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:flutter/foundation.dart';

import '../entities/agendamento.dart';
import '../../data/resources/api_response.dart';


class AgendamentoService extends ChangeNotifier {
  static const String baseUrl =
      'http://localhost:5111/api/Agendamento';


  // GET: api/agendamento
  Future<List<Agendamento>> obterTodos() async {
    final response = await http.get(Uri.parse(baseUrl));

    if (response.statusCode == 200) {
      final body = jsonDecode(response.body);
      final List dados = body['dados'] ?? [];
      return dados.map((item) => Agendamento.fromJson(item)).toList();
    } else {
      throw Exception('Falha ao carregar agendamentos');
    }
  }

  // POST: api/agendamento/cadastrar
  Future<void> cadastrar(Agendamento agendamento) async {
    final response = await http.post(
      Uri.parse('$baseUrl/cadastrar'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(agendamento.toCadastroJson()),
    );

    if (response.statusCode != 200) {
      final body = jsonDecode(response.body);
      throw Exception(body['mensagem'] ?? 'Erro ao cadastrar agendamento');
    }
  }
}