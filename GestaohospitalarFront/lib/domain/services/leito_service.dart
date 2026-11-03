import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../entities/leito.dart';
import '../../data/resources/api_response.dart';

/// Alias de tipo para evitar o conflito do operador '>>>' no compilador Dart
typedef JsonMap = Map<String, dynamic>;

class LeitoService extends ChangeNotifier {
  static const String baseUrl = 'http://localhost:5111/api';

  List<Leito> _leitos = [];
  List<Leito> get leitos => _leitos;

  final Map<String, String> _headers = {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  };

  /// Busca o mapa completo de leitos normalizado para os DTOs C# e telas Flutter
  Future<List<JsonMap>> buscarMapaLeitos() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/Leito'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> json = jsonDecode(response.body);

        final resultado = ApiResponse<List<dynamic>>.fromJson(
          json,
          (dados) => dados as List<dynamic>,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao buscar mapa de leitos.',
          );
        }

        final listaMapa = (resultado.dados ?? []).map((item) {
          final map = Map<String, dynamic>.from(item as Map);

          final idLeito = map['idLeito'] ?? map['id_leito'];
          final numero = map['numero']?.toString() ?? map['numeroLeito']?.toString();
          final situacao = map['situacao'] ?? map['situacaoLeito'] ?? map['status_leito'] ?? 'VAGO';
          final idAla = map['idAla'] ?? map['id_ala'];
          final nomeAla = map['nomeAla'] ?? map['nome_ala'] ?? map['ala'] ?? '';
          final andarAla = map['andarAla'] ?? map['andar'] ?? '';

          return <String, dynamic>{
            'id_leito': idLeito,
            'idLeito': idLeito,
            'numero_leito': numero,
            'numero': numero,
            'id_ala': idAla,
            'idAla': idAla,
            'ala': nomeAla,
            'nomeAla': nomeAla,
            'status_leito': situacao,
            'situacao': situacao,
            'andar': andarAla,
            'nome': map['nome'] ?? map['nome_paciente'] ?? map['nomePaciente'],
            'nome_paciente': map['nome_paciente'] ?? map['nome'] ?? map['nomePaciente'],
            'id_prontuario': map['id_prontuario'] ?? map['idProntuario'],
            'id_internacao': map['id_internacao'] ?? map['idInternacao'],
            'id_medico': map['id_medico'] ?? map['idMedico'],
          };
        }).toList();

        _leitos = listaMapa.map((map) => Leito.fromJson(map)).toList();
        notifyListeners();

        return listaMapa;
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao buscar mapa de leitos: $e');
    }
  }

  Future<List<Leito>> buscarTodosLeitos() async {
    await buscarMapaLeitos();
    return _leitos;
  }

  Future<List<Leito>> listarLeitosDisponiveis() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/Leito/vagos'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> json = jsonDecode(response.body);

        final resultado = ApiResponse<List<dynamic>>.fromJson(
          json,
          (dados) => dados as List<dynamic>,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao buscar leitos vagos.',
          );
        }

        _leitos = (resultado.dados ?? [])
            .map((item) => Leito.fromJson(Map<String, dynamic>.from(item as Map)))
            .toList();

        notifyListeners();
        return _leitos;
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao buscar leitos vagos: $e');
    }
  }

  Future<List<Leito>> buscarLeitosDisponiveis() async {
    return await listarLeitosDisponiveis();
  }

  Future<bool> cadastrarLeito(Leito leito) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/Leito/cadastrar'),
        headers: _headers,
        body: jsonEncode(leito.toMap()),
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        await buscarMapaLeitos();
        return true;
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao cadastrar leito: $e');
    }
  }

  Future<bool> atualizarLeito(int idLeito, Leito leito) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/Leito/$idLeito'),
        headers: _headers,
        body: jsonEncode(leito.toMap()),
      );

      if (response.statusCode == 200) {
        await buscarMapaLeitos();
        return true;
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao atualizar leito: $e');
    }
  }

  Future<bool> atualizarStatusLeito(int idLeito, String novaSituacao) async {
    try {
      if (novaSituacao.toUpperCase() == 'VAGO') {
        return await liberarHigienizacao(idLeito);
      }
      final response = await http.put(
        Uri.parse('$baseUrl/Leito/$idLeito'),
        headers: _headers,
        body: jsonEncode({'situacao': novaSituacao}),
      );

      if (response.statusCode == 200) {
        await buscarMapaLeitos();
        return true;
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao atualizar status do leito: $e');
    }
  }

  Future<bool> liberarHigienizacao(int idLeito) async {
    try {
      final response = await http.patch(
        Uri.parse('$baseUrl/Leito/$idLeito/liberar-higienizacao'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        await buscarMapaLeitos();
        return true;
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao liberar higienização: $e');
    }
  }

  Future<bool> excluirLeito(int idLeito) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/Leito/$idLeito'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        await buscarMapaLeitos();
        return true;
      } else {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao excluir leito: $e');
    }
  }

  // =========================================================================
  // INTEGRAÇÃO COM PRONTUÁRIOS E EXAMES
  // =========================================================================

  Future<void> atualizarEvolucaoProntuario(int idProntuario, String novaEvolucao) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/Prontuarios/$idProntuario'),
        headers: _headers,
        body: jsonEncode({
          'evolucao': novaEvolucao,
          'statusProntuario': 'ATIVO',
        }),
      );

      if (response.statusCode != 200) {
        throw Exception(
          'Erro HTTP ${response.statusCode}: ${response.body}',
        );
      }
    } catch (e) {
      throw Exception('Erro ao atualizar evolução do prontuário: $e');
    }
  }

  Future<List<JsonMap>> listarExamesCatalogo() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/Exame'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> json = jsonDecode(response.body);

        final resultado = ApiResponse<List<dynamic>>.fromJson(
          json,
          (dados) => dados as List<dynamic>,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao listar exames.',
          );
        }

        return (resultado.dados ?? [])
            .map((item) => Map<String, dynamic>.from(item as Map))
            .toList();
      } else {
        return [];
      }
    } catch (e) {
      return [];
    }
  }

  Future<List<JsonMap>> listarExamesSolicitados(int idProntuario) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/SolicitacaoExame/prontuario/$idProntuario'),
        headers: _headers,
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> json = jsonDecode(response.body);

        final resultado = ApiResponse<List<dynamic>>.fromJson(
          json,
          (dados) => dados as List<dynamic>,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao buscar exames solicitados.',
          );
        }

        return (resultado.dados ?? [])
            .map((item) => Map<String, dynamic>.from(item as Map))
            .toList();
      } else {
        return [];
      }
    } catch (e) {
      return [];
    }
  }
}