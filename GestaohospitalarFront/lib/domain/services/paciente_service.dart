
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

import '../entities/paciente.dart';
import '../../data/resources/api_response.dart';

class PacienteService with ChangeNotifier {
  static const String baseUrl = 'http://localhost:5111/api/Paciente';

  List<Paciente> _pacientes = [];
  bool _isLoading = false;

  List<Paciente> get pacientes => _pacientes;
  bool get isLoading => _isLoading;

  Map<String, String> get _headers => {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };

  /// Trata respostas de erro da API.
  void _verificarResposta(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return;
    }

    String mensagem = 'Erro ao comunicar com a API.';

    try {
      final json = jsonDecode(response.body);
      mensagem = json['mensagem'] ??
          json['title'] ??
          json['errors']?.toString() ??
          mensagem;
    } catch (_) {
      if (response.body.isNotEmpty) {
        mensagem = response.body;
      }
    }

    throw Exception(mensagem);
  }

  /// Converte a resposta envelopada da API em uma lista de pacientes.
  List<Paciente> _lerLista(
    http.Response response, {
    required int ativo,
  }) {
    _verificarResposta(response);

    final json = jsonDecode(response.body) as Map<String, dynamic>;

    final resultado = ApiResponse<List<Paciente>>.fromJson(
      json,
      (dados) => (dados as List)
          .map(
            (item) => Paciente.fromJson(
              item as Map<String, dynamic>,
              ativoOverride: ativo,
            ),
          )
          .toList(),
    );

    if (!resultado.sucesso) {
      throw Exception(
        resultado.mensagem ?? 'Não foi possível carregar pacientes.',
      );
    }

    return resultado.dados ?? [];
  }

  /// Carrega pacientes ativos.
  Future<void> carregarPacientes() async {
    _isLoading = true;
    notifyListeners();

    try {
      final response = await http.get(
        Uri.parse('$baseUrl/ativos'),
        headers: _headers,
      );

      _pacientes = _lerLista(response, ativo: 1);
    } catch (e) {
      debugPrint('Erro ao buscar pacientes: $e');
      rethrow;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  /// Retorna pacientes ativos.
  Future<List<Paciente>> listarPacientes() async {
    await carregarPacientes();
    return _pacientes;
  }

  /// Retorna pacientes arquivados.
  Future<List<Paciente>> listarPacientesArquivados() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/inativos'),
        headers: _headers,
      );

      return _lerLista(response, ativo: 0);
    } catch (e) {
      debugPrint('Erro ao buscar pacientes arquivados: $e');
      rethrow;
    }
  }

  /// Retorna pacientes arquivados em formato Map para a TelaArquivo.
  Future<List<Map<String, dynamic>>> listarPacientesArquivadosEmMap() async {
    final pacientes = await listarPacientesArquivados();

    return pacientes.map((paciente) {
      return {
        'id': paciente.id,
        'nome': paciente.nome,
        'cpf': paciente.cpf,
      };
    }).toList();
  }

  /// Busca os dados completos do paciente para edição.
  /// Busca os dados completos do paciente para edição.
Future<Paciente> buscarPorId(int id) async {
  final response = await http.get(
    Uri.parse('$baseUrl/$id'),
    headers: _headers,
  );

  _verificarResposta(response);

  final json = jsonDecode(response.body) as Map<String, dynamic>;

  if (json['sucesso'] == false) {
    throw Exception(json['mensagem'] ?? 'Paciente não encontrado.');
  }

  // A API utiliza o padrão { sucesso, mensagem, dados }.
  // Também aceita uma resposta sem envelope, caso o controller retorne assim.
  final dados = json['dados'] is Map<String, dynamic>
      ? json['dados'] as Map<String, dynamic>
      : json;

  return Paciente.fromJson(
    dados,
    idOverride: id,
  );
}

  /// Busca os dados do vínculo de convênio do paciente.
  /// Busca os dados do vínculo de convênio do paciente.
Future<Map<String, dynamic>?> buscarVinculoConvenio(
  int idPaciente,
) async {
  final response = await http.get(
    Uri.parse('$baseUrl/$idPaciente'),
    headers: _headers,
  );

  _verificarResposta(response);

  final json = jsonDecode(response.body) as Map<String, dynamic>;

  if (json['sucesso'] == false) {
    throw Exception(
      json['mensagem'] ?? 'Não foi possível buscar o convênio.',
    );
  }

  // Extrai os dados de dentro do envelope da API.
  final dados = json['dados'] is Map<String, dynamic>
      ? json['dados'] as Map<String, dynamic>
      : json;

  final convenio = dados['convenio'];

  if (convenio == null) return null;

  return Map<String, dynamic>.from(convenio);
}
  /// Cadastra ou atualiza um paciente.
  Future<void> salvarPaciente(
    Paciente paciente, {
    int? idConvenio,
    String? nomeConvenio,
    String? numeroCarteira,
    DateTime? validade,
  }) async {
    if (paciente.nome.trim().isEmpty) {
      throw Exception('O nome do paciente é obrigatório.');
    }

    if (paciente.nascimento == null) {
      throw Exception('A data de nascimento do paciente é obrigatória.');
    }

    if (idConvenio != null &&
        (nomeConvenio == null || nomeConvenio.trim().isEmpty)) {
      throw Exception(
        'A API precisa do nome do convênio para realizar o vínculo. '
        'Envie nomeConvenio em vez de somente idConvenio.',
      );
    }

    final body = paciente.toJson(
      nomeConvenio: nomeConvenio,
      numeroCarteira: numeroCarteira,
      validade: validade,
    );

    if (nomeConvenio != null && validade == null) {
      throw Exception('Informe a validade da carteira do convênio.');
    }

    final bool editando = paciente.id != null;

    final response = editando
        ? await http.put(
            Uri.parse('$baseUrl/${paciente.id}'),
            headers: _headers,
            body: jsonEncode(body),
          )
        : await http.post(
            Uri.parse('$baseUrl/cadastrar'),
            headers: _headers,
            body: jsonEncode(body),
          );

    _verificarResposta(response);

    final json = jsonDecode(response.body) as Map<String, dynamic>;

    final resultado = ApiResponse<dynamic>.fromJson(
      json,
      (dados) => dados,
    );

    if (!resultado.sucesso) {
      throw Exception(
        resultado.mensagem ?? 'Não foi possível salvar o paciente.',
      );
    }

    await carregarPacientes();
  }

  /// Arquiva um paciente pela API.
  Future<void> arquivarPaciente(Paciente paciente) async {
    if (paciente.id == null) {
      throw Exception('Não foi possível identificar o paciente.');
    }

    try {
      final response = await http.patch(
        Uri.parse('$baseUrl/${paciente.id}/arquivar'),
        headers: _headers,
      );

      _verificarResposta(response);

      await carregarPacientes();
    } catch (e) {
      debugPrint('Erro ao arquivar paciente: $e');
      rethrow;
    }
  }

  /// Restaura um paciente arquivado pela API.
  Future<void> restaurarPacientePorId(int id) async {
    try {
      final response = await http.patch(
        Uri.parse('$baseUrl/$id/ativar'),
        headers: _headers,
      );

      _verificarResposta(response);

      await carregarPacientes();
    } catch (e) {
      debugPrint('Erro ao restaurar paciente: $e');
      rethrow;
    }
  }

  /// Restaura um paciente utilizando seu objeto.
  Future<void> restaurarPaciente(Paciente paciente) async {
    if (paciente.id == null) {
      throw Exception('Não foi possível identificar o paciente.');
    }

    await restaurarPacientePorId(paciente.id!);
  }

  /// Exclui permanentemente um paciente.
  Future<void> deletarPaciente(int id) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/$id'),
        headers: _headers,
      );

      _verificarResposta(response);

      await carregarPacientes();
    } catch (e) {
      debugPrint('Erro ao deletar paciente: $e');
      rethrow;
    }
  }
}
