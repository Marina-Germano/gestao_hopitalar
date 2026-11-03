
import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

import '../entities/almoxarifado.dart';
import '../../data/resources/api_response.dart';

class AlmoxarifadoService extends ChangeNotifier {
  static const String baseUrl =
      'http://localhost:5111/api/Almoxarifado';

  static const String medicamentoUrl =
      'http://localhost:5111/api/Medicamento';

  List<Almoxarifado> _itens = [];
  List<Almoxarifado> get itens => _itens;

  bool get temAlertaEstoque =>
      _itens.any((item) => item.quantidade < item.estoqueMinimo);

  bool _isLoading = false;
  bool get isLoading => _isLoading;

  Map<String, dynamic> _paraDto(Almoxarifado item) {
    return {
      'nome': item.nome,
      'categoria': item.categoria,
      'descricao': item.descricao,
      'quantidade': item.quantidade,
      'unidade': item.unidade,
      'valorUnitario': item.valorUnitario,
      'estoqueMinimo': item.estoqueMinimo,
      'lote': item.lote,
      'validade': item.validade?.toIso8601String(),
    };
  }

  Map<String, dynamic> _paraMedicamentoDto(Almoxarifado item) {
    return {
      'nome': item.nome,
      'descricao': item.descricao,
      'quantidade': item.quantidade,
      'unidade': item.unidade,
      'valorUnitario': item.valorUnitario,
      'estoqueMinimo': item.estoqueMinimo,
      'lote': item.lote,
      'validade': item.validade?.toIso8601String(),
      'principioAtivo': item.principioAtivo,
      'contraindicacoes': item.contraindicacoes,
    };
  }

  Future<void> carregarItens() async {
    _isLoading = true;
    notifyListeners();

    try {
      final response = await http.get(Uri.parse(baseUrl));

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);

        final resultado =
            ApiResponse<List<Almoxarifado>>.fromJson(
          json,
          (dados) => (dados as List)
              .map((item) => Almoxarifado.fromJson(item))
              .toList(),
        );

        if (resultado.sucesso) {
          _itens = resultado.dados ?? [];
        } else {
          throw Exception(
            resultado.mensagem ?? 'Erro ao carregar itens.',
          );
        }
      } else {
        throw Exception('Erro HTTP: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint('Erro ao carregar almoxarifado: $e');
      rethrow;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> carregarDetalhesMedicamento(
      Almoxarifado item) async {
    try {
      final response = await http.get(
        Uri.parse(medicamentoUrl),
      );

      if (response.statusCode != 200) {
        throw Exception(
          'Erro ao consultar medicamentos: ${response.statusCode}',
        );
      }

      final json = jsonDecode(response.body);

      final resultado =
          ApiResponse<List<Map<String, dynamic>>>.fromJson(
        json,
        (dados) => (dados as List)
            .map((med) => Map<String, dynamic>.from(med))
            .toList(),
      );

      if (!resultado.sucesso) {
        throw Exception(
          resultado.mensagem ?? 'Erro ao buscar medicamentos.',
        );
      }

      final medicamentos = resultado.dados ?? [];

      final encontrados = medicamentos.where(
        (med) =>
            (med['idAlmoxarifado'] as num?)?.toInt() == item.id,
      );

      if (encontrados.isEmpty) {
        throw Exception(
          'Não foi encontrado o cadastro clínico do medicamento.',
        );
      }

      final dados = encontrados.first;

      item.idMedicamento =
          (dados['idMedicamento'] as num?)?.toInt();

      item.principioAtivo =
          dados['principioAtivo']?.toString();

      item.contraindicacoes =
          dados['contraindicacoes']?.toString();

      debugPrint(
        'ID Medicamento: ${item.idMedicamento}',
      );
      debugPrint(
        'Princípio ativo: ${item.principioAtivo}',
      );
      debugPrint(
        'Contraindicações: ${item.contraindicacoes}',
      );
    } catch (e) {
      debugPrint('Erro ao carregar detalhes: $e');
      rethrow;
    }
  }

  Future<void> salvarItem(Almoxarifado item) async {
    try {
      final bool cadastrando = item.id == null;
      final bool medicamento =
          item.categoria?.toUpperCase() == 'MEDICAMENTO';

      late http.Response response;

      if (medicamento) {
        if (cadastrando) {
          // Novo medicamento
          response = await http.post(
            Uri.parse('$medicamentoUrl/cadastrar'),
            headers: {
              'Content-Type': 'application/json',
            },
            body: jsonEncode(_paraMedicamentoDto(item)),
          );
        } else {
          // Atualização de medicamento existente
          if (item.idMedicamento == null) {
            throw Exception(
              'ID do medicamento não encontrado. '
              'Não é possível atualizar com segurança.',
            );
          }

          response = await http.put(
            Uri.parse(
              '$medicamentoUrl/${item.idMedicamento}',
            ),
            headers: {
              'Content-Type': 'application/json',
            },
            body: jsonEncode(_paraMedicamentoDto(item)),
          );
        }
      } else {
        final url = cadastrando
            ? Uri.parse(baseUrl)
            : Uri.parse('$baseUrl/${item.id}');

        response = cadastrando
            ? await http.post(
                url,
                headers: {
                  'Content-Type': 'application/json',
                },
                body: jsonEncode(_paraDto(item)),
              )
            : await http.put(
                url,
                headers: {
                  'Content-Type': 'application/json',
                },
                body: jsonEncode(_paraDto(item)),
              );
      }

      if (response.statusCode == 200 ||
          response.statusCode == 201) {
        final json = jsonDecode(response.body);

        final resultado = ApiResponse<dynamic>.fromJson(
          json,
          (dados) => dados,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao salvar item.',
          );
        }

        await carregarItens();
      } else {
        throw Exception(
          'Erro ao salvar item. HTTP: ${response.statusCode} '
          '- ${response.body}',
        );
      }
    } catch (e) {
      debugPrint('Erro ao salvar item: $e');
      rethrow;
    }
  }

  Future<void> deletarItem(int id) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/$id'),
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);

        final resultado = ApiResponse<dynamic>.fromJson(
          json,
          (dados) => dados,
        );

        if (!resultado.sucesso) {
          throw Exception(
            resultado.mensagem ?? 'Erro ao excluir item.',
          );
        }

        await carregarItens();
      } else {
        throw Exception(
          'Erro ao excluir item: ${response.statusCode}',
        );
      }
    } catch (e) {
      debugPrint('Erro ao deletar item: $e');
      rethrow;
    }
  }

  Future<void> arquivarItem(Almoxarifado item) async {
    if (item.id != null) {
      await deletarItem(item.id!);
    }
  }
}
