// lib/domain/services/convenio_service.dart
import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import '../entities/convenio.dart';
import '../../data/resources/api_response.dart';

class ConvenioService extends ChangeNotifier {
  
  static const String baseUrl = 'http://localhost:5111/api/Convenios';

  List<Convenio> _convenios = [];
  List<Convenio> get convenios => _convenios;

  bool _isLoading = false;
  bool get isLoading => _isLoading;

  // ==========================================
  // GET: Carregar Convênios
  // ==========================================
  Future<void> carregarConvenios() async {
    _isLoading = true;
    notifyListeners();

    try {
      final response = await http.get(Uri.parse(baseUrl));

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);
        final apiResponse = ApiResponse<List<Convenio>>.fromJson(
          jsonResponse,
          (dados) => (dados as List).map((item) => Convenio.fromJson(item)).toList(),
        );

        final todos = apiResponse.dados ?? [];
        // Filtra apenas ativos
        _convenios = todos.where((c) => c.ativo != 0).toList();
      } else {
        throw Exception('Erro ao carregar convênios. Status: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint("Erro ao carregar convênios: $e");
      throw Exception("Erro de conexão com o servidor: $e");
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  // ==========================================
  // POST / PUT: Salvar Convênio (Novo ou Atualizar)
  // ==========================================
  Future<void> salvarConvenio(Convenio convenio) async {
    try {
      http.Response response;

      if (convenio.id == null) {
        response = await http.post(
          Uri.parse(baseUrl),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode(convenio.toJson()),
        );
      } else {
        response = await http.put(
          Uri.parse('$baseUrl/${convenio.id}'),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode(convenio.toJson()),
        );
      }

      if (response.statusCode == 200 || response.statusCode == 201) {
        await carregarConvenios();
      } else {
        throw Exception('Erro ao salvar convênio. Status: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint("Erro ao salvar convênio: $e");
      rethrow;
    }
  }

  // ==========================================
  // DELETE: Excluir Convênio Definitivamente
  // ==========================================
  Future<void> excluirConvenio(Convenio convenio) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/${convenio.id}'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        await carregarConvenios();
      } else {
        throw Exception('Erro ao excluir convênio. Status: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint("Erro ao excluir convênio: $e");
      rethrow;
    }
  }
}