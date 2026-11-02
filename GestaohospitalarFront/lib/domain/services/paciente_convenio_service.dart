
import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

import '../entities/paciente_convenio.dart';
import '../../data/resources/api_response.dart';

class PacienteConvenioService extends ChangeNotifier {
  static const String baseUrl =
      'http://localhost:5111/api/PacienteConvenio';

  List<PacienteConvenio> _vinculos = [];

  List<PacienteConvenio> get vinculos => _vinculos;

  // Vincular convênio ao paciente
  Future<void> vincularConvenio(PacienteConvenio vinculo) async {
    final url = Uri.parse('$baseUrl/cadastrar');

    final body = {
      'idPaciente': vinculo.idPaciente,
      'idConvenio': vinculo.idConvenio,
      'numeroCarteira': vinculo.numeroCarteira,
      'validade': vinculo.validade.toIso8601String().split('T').first,
    };

    final response = await http.post(
      url,
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: jsonEncode(body),
    );

    final json = jsonDecode(response.body);

    final resultado = ApiResponse<dynamic>.fromJson(
      json,
      (dados) => dados,
    );

    if (response.statusCode >= 200 &&
        response.statusCode < 300 &&
        resultado.sucesso) {
      await buscarPorPaciente(vinculo.idPaciente);
      notifyListeners();
    } else {
      throw Exception(
        resultado.mensagem ?? 'Erro ao vincular convênio ao paciente.',
      );
    }
  }

  // Buscar convênios vinculados a um paciente
  Future<PacienteConvenio?> buscarPorPaciente(int idPaciente) async {
    final url = Uri.parse('$baseUrl/paciente/$idPaciente');

    final response = await http.get(
      url,
      headers: {
        'Accept': 'application/json',
      },
    );

    final json = jsonDecode(response.body);

    final resultado = ApiResponse<List<PacienteConvenio>>.fromJson(
      json,
      (dados) => (dados as List)
          .map(
            (item) => PacienteConvenio.fromJson(
              item as Map<String, dynamic>,
            ),
          )
          .toList(),
    );

    if (response.statusCode >= 200 &&
        response.statusCode < 300 &&
        resultado.sucesso) {
      _vinculos = resultado.dados ?? [];

      notifyListeners();

      // Retorna somente um vínculo ativo, se existir
      for (final vinculo in _vinculos) {
        if (vinculo.ativo) {
          return vinculo;
        }
      }

      return null;
    }

    throw Exception(
      resultado.mensagem ?? 'Erro ao buscar convênio do paciente.',
    );
  }

  // Inativar vínculo com o convênio
  Future<void> inativarConvenio(int idVinculo) async {
    final url = Uri.parse('$baseUrl/$idVinculo/inativar');

    final response = await http.put(
      url,
      headers: {
        'Accept': 'application/json',
      },
    );

    final json = jsonDecode(response.body);

    final resultado = ApiResponse<dynamic>.fromJson(
      json,
      (dados) => dados,
    );

    if (response.statusCode >= 200 &&
        response.statusCode < 300 &&
        resultado.sucesso) {
      _vinculos.removeWhere((v) => v.id == idVinculo);
      notifyListeners();
    } else {
      throw Exception(
        resultado.mensagem ?? 'Erro ao inativar vínculo do convênio.',
      );
    }
  }
}