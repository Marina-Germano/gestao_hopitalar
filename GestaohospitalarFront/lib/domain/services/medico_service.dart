import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../entities/medico.dart';
import '../../data/resources/api_response.dart';

class MedicoService extends ChangeNotifier {
  static const String baseUrl = 'http://localhost:5111/api/Medico';

  List<Medico> _medicos = [];

  List<Medico> get medicos => _medicos;

  // GET: Listar médicos ativos
  Future<List<Medico>> listarMedicos() async {
    try {
      final response = await http.get(Uri.parse('$baseUrl/ativos'));

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse = ApiResponse<List<Medico>>.fromJson(
          jsonResponse,
          (dados) => (dados as List)
              .map(
                (item) => Medico.fromJson(
                  Map<String, dynamic>.from(item),
                ),
              )
              .toList(),
        );

        _medicos = apiResponse.dados ?? [];
        notifyListeners();
        return _medicos;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao carregar médicos: $e');
    }
  }

  // GET: Atualizar a lista de médicos
  Future<void> carregarMedicos() async {
    await listarMedicos();
  }

  // GET: Listar médicos arquivados
  Future<List<Medico>> listarMedicosArquivados() async {
    try {
      final response = await http.get(Uri.parse('$baseUrl/inativos'));

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse = ApiResponse<List<Medico>>.fromJson(
          jsonResponse,
          (dados) => (dados as List)
              .map(
                (item) => Medico.fromJson(
                  Map<String, dynamic>.from(item),
                ),
              )
              .toList(),
        );

        return apiResponse.dados ?? [];
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao carregar médicos arquivados: $e');
    }
  }

  // GET: Buscar médico pelo ID (modelo básico Medico)
  Future<Medico?> buscarMedicoPorId(int id) async {
    try {
      final response = await http.get(Uri.parse('$baseUrl/$id'));

      if (response.statusCode == 404) {
        return null;
      }

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);

        final apiResponse = ApiResponse<Medico>.fromJson(
          jsonResponse,
          (dados) => Medico.fromJson(
            Map<String, dynamic>.from(dados),
          ),
        );

        return apiResponse.dados;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao buscar médico: $e');
    }
  }

  // NOVO GET: Buscar todos os dados do médico para preencher a edição
    Future<Map<String, dynamic>?> buscarDadosMedicoParaEdicao(
    int id,
  ) async {
    try {
      final response = await http.get(Uri.parse('$baseUrl/$id'));

      debugPrint('Buscar médico para edição - status: ${response.statusCode}');
      debugPrint('Buscar médico para edição - resposta: ${response.body}');

      if (response.statusCode == 404) {
        return null;
      }

      if (response.statusCode != 200) {
        throw Exception(
          'Status ${response.statusCode}: ${response.body}',
        );
      }

      final json = jsonDecode(response.body);

      if (json is! Map) {
        throw Exception('A API não retornou um objeto JSON válido.');
      }

      Map<String, dynamic> dados = Map<String, dynamic>.from(json);

      // Aceita envelopes comuns usados pela API.
      for (final chave in ['dados', 'data', 'medico', 'resultado']) {
        final entrada = dados.entries.where(
          (e) => e.key.toLowerCase() == chave,
        );

        if (entrada.isNotEmpty && entrada.first.value is Map) {
          dados = Map<String, dynamic>.from(entrada.first.value as Map);
          break;
        }
      }

      debugPrint('Dados encontrados para edição: $dados');

      return dados;
    } catch (e) {
      debugPrint('Erro ao buscar dados para edição: $e');
      rethrow;
    }
  }

  // POST: Cadastrar médico usando o DTO da API
  Future<bool> cadastrarMedico(Medico medico) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/cadastrar'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(medico.toJson()),
      );

      debugPrint('Cadastro de médico: ${response.statusCode}');
      debugPrint('Resposta da API: ${response.body}');

      if (response.statusCode == 200 || response.statusCode == 201) {
        await carregarMedicos();
        return true;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao cadastrar médico: $e');
    }
  }

  // POST/PUT: Salvar médico usando todos os dados do formulário
  Future<void> salvarMedicoComDados({
    required int? id,
    required Map<String, dynamic> dados,
  }) async {
    try {
      final bool cadastrando = id == null;

      final response = cadastrando
          ? await http.post(
              Uri.parse('$baseUrl/cadastrar'),
              headers: {'Content-Type': 'application/json'},
              body: jsonEncode(dados),
            )
          : await http.put(
              Uri.parse('$baseUrl/$id'),
              headers: {'Content-Type': 'application/json'},
              body: jsonEncode(dados),
            );

      debugPrint('Salvar médico - status: ${response.statusCode}');
      debugPrint('Salvar médico - resposta: ${response.body}');

      if (response.statusCode == 200 || response.statusCode == 201) {
        await carregarMedicos();
        return;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      debugPrint('Erro ao salvar médico: $e');
      rethrow;
    }
  }

  // Mantido para compatibilidade com chamadas antigas
  Future<void> salvarMedicoComEspecialidade(
    Medico medico,
    String especialidade,
  ) async {
    try {
      if (medico.id == null) {
        await cadastrarMedico(medico);
      } else {
        await atualizarMedico(medico.id!, medico);
      }
    } catch (e) {
      throw Exception('Erro ao salvar médico: $e');
    }
  }

  // PUT: Atualizar médico pelo modelo Medico
  Future<bool> atualizarMedico(int id, Medico medico) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/$id'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(medico.toJson()),
      );

      debugPrint('Atualizar médico - status: ${response.statusCode}');
      debugPrint('Atualizar médico - resposta: ${response.body}');

      if (response.statusCode == 200) {
        await carregarMedicos();
        return true;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao atualizar médico: $e');
    }
  }

  // PATCH: Arquivar médico
  Future<bool> arquivarMedico(Medico medico) async {
    final id = medico.id;

    if (id == null) {
      throw Exception('O médico não possui um ID válido.');
    }

    try {
      final response = await http.patch(
        Uri.parse('$baseUrl/$id/arquivar'),
        headers: {'Content-Type': 'application/json'},
      );

      debugPrint('Arquivar médico - ID: $id');
      debugPrint('Arquivar médico - status: ${response.statusCode}');
      debugPrint('Arquivar médico - resposta: ${response.body}');

      if (response.statusCode == 200 || response.statusCode == 204) {
        await carregarMedicos();
        notifyListeners();
        return true;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      debugPrint('Erro ao arquivar médico: $e');
      rethrow;
    }
  }

  // PATCH: Reativar médico
  Future<bool> restaurarMedico(Medico medico) async {
    try {
      if (medico.id == null) {
        throw Exception('O médico não possui um ID válido.');
      }

      final response = await http.patch(
        Uri.parse('$baseUrl/${medico.id}/ativar'),
      );

      if (response.statusCode == 200) {
        await carregarMedicos();
        notifyListeners(); // <--- ADICIONE ESTA LINHA AQUI
        return true;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao reativar médico: $e');
    }
  }

  // DELETE: Excluir médico
  Future<bool> excluirMedico(int id) async {
    try {
      final response = await http.delete(Uri.parse('$baseUrl/$id'));

      if (response.statusCode == 200) {
        await carregarMedicos();
        return true;
      }

      throw Exception(
        'Status ${response.statusCode}: ${response.body}',
      );
    } catch (e) {
      throw Exception('Erro ao excluir médico: $e');
    }
  }
  // GET: Listar médicos arquivados como Map
Future<List<Map<String, dynamic>>> listarMedicosArquivadosEmMap() async {
  final medicosArquivados = await listarMedicosArquivados();

  return medicosArquivados.map((medico) => medico.toJson()).toList();
}
// PATCH: Restaurar médico pelo ID
Future<bool> restaurarMedicoPorId(int id) async {
  try {
    final response = await http.patch(
      Uri.parse('$baseUrl/$id/ativar'),
      headers: {'Content-Type': 'application/json'},
    );

    debugPrint('Restaurar médico - ID: $id');
    debugPrint('Restaurar médico - status: ${response.statusCode}');
    debugPrint('Restaurar médico - resposta: ${response.body}');

    if (response.statusCode == 200 || response.statusCode == 204) {
      await carregarMedicos();
      notifyListeners();
      return true;
    }

    throw Exception(
      'Status ${response.statusCode}: ${response.body}',
    );
  } catch (e) {
    debugPrint('Erro ao restaurar médico: $e');
    rethrow;
  }
}
}