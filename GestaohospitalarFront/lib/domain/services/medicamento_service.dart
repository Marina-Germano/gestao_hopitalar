
import 'dart:convert';
import 'package:http/http.dart' as http;

import '../../data/resources/api_response.dart';
class MedicamentoService {
  static const String baseUrlMedicamento =
      'http://localhost:5111/api/Medicamento';

  static const String baseUrlPrescricao =
      'http://localhost:5111/api/Prescricao';

  // Busca todos os medicamentos cadastrados na API.
  Future<List<Map<String, dynamic>>> listarMedicamentosCatalogo() async {
    try {
      final response = await http.get(
        Uri.parse(baseUrlMedicamento),
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);

        final resultado = ApiResponse<List<dynamic>>.fromJson(
          json,
          (dados) => dados as List<dynamic>,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao buscar medicamentos.',
          );
        }

        return (resultado.dados ?? []).map((item) {
          final medicamento = item as Map<String, dynamic>;

          return {
            'id_medicamento': medicamento['idMedicamento'],
            'id_almoxarifado': medicamento['idAlmoxarifado'],
            'nome': medicamento['nome'],
            'principio_ativo': medicamento['principioAtivo'],
            'contraindicacoes': medicamento['contraindicacoes'],
          };
        }).toList();
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao buscar catálogo de medicamentos: $e');
    }
  }

  // Grava a prescrição através da API.
  Future<void> prescreverMedicamento({
    required int idProntuario,
    required int idMedicamento,
    required int idMedico,
    required String dosagem,
    required String viaAplicacao,
    required String frequencia,
    String? observacao,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrlPrescricao/cadastrar'),
        headers: {
          'Content-Type': 'application/json',
        },
        body: jsonEncode({
          'idProntuario': idProntuario,
          'idMedicamento': idMedicamento,
          'idMedico': idMedico,
          'dosagem': dosagem,
          'aplicacao': viaAplicacao,
          'horario': frequencia,
          'observacao': observacao,
        }),
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
            resultado.mensagem ?? 'Erro ao salvar prescrição.',
          );
        }
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao salvar a prescrição: $e');
    }
  }

  // Busca as prescrições de um prontuário através da API.
  Future<List<Map<String, dynamic>>> listarPrescricoesPorProntuario(
    int idProntuario,
  ) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrlPrescricao/prontuario/$idProntuario'),
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);

        final resultado = ApiResponse<List<dynamic>>.fromJson(
          json,
          (dados) => dados as List<dynamic>,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao buscar prescrições.',
          );
        }

        return (resultado.dados ?? []).map((item) {
          final prescricao = item as Map<String, dynamic>;

          return {
            'id_prescricao': prescricao['idPrescricao'],
            'id_prontuario': prescricao['idProntuario'],
            'id_medicamento': prescricao['idMedicamento'],
            'id_medico': prescricao['idMedico'],
            'dosagem': prescricao['dosagem'],
            'aplicacao': prescricao['aplicacao'],
            'horario': prescricao['horario'],
            'observacao': prescricao['observacao'],
            'nome_medicamento': prescricao['nomeMedicamento'],
            'nome_medico': prescricao['nomeMedico'],
          };
        }).toList();
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao buscar histórico de prescrições: $e');
    }
  }
}
