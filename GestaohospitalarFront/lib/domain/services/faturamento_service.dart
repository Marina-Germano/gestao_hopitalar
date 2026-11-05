import 'package:sqflite/sqflite.dart';
import '../entities/faturamento.dart';
import '../repository/entitie_repository.dart';
import '../../data/repositories/generic_repository_impl.dart';

class FaturamentoService {
  final Database db;
  late final EntitieRepository<Faturamento> _repository;

  FaturamentoService(this.db) {
    _repository = GenericRepositoryImpl<Faturamento>(
      db: db,
      tableName: 'faturamento',
      fromMap: (map) => Faturamento.fromMap(map),
      toMap: (f) => f.toMap(),
    );
  }

  /// Método Único: Processa Alta, Calcula Valores e Salva tudo de uma vez
  Future<void> processarAltaEGerarFaturamento({
    required int idProntuario,
    required int? idInternacao,
    required int? idSala,
    required int? idLeito,
  }) async {
    try {
      print("Iniciando processamento de alta para Prontuário: $idProntuario");

      await db.transaction((txn) async {
        // 1. CÁLCULOS
        // Honorários
        final med = await txn.rawQuery('SELECT m.honorario FROM prontuario pr JOIN medico m ON pr.id_medico = m.id_medico WHERE pr.id_prontuario = ?', [idProntuario]);
        double valorHonorarios = (med.isNotEmpty) ? (med.first['honorario'] as num).toDouble() : 0.0;

        // Exames
        final ex = await txn.rawQuery('SELECT SUM(e.valor) as total FROM solicitacao_exame se JOIN exame e ON se.id_exame = e.id_exame WHERE se.id_prontuario = ?', [idProntuario]);
        double valorExames = (ex.first['total'] as num? ?? 0).toDouble();

        // Medicamentos/Consumo
        double valorConsumo = 0;
        if (idInternacao != null) {
          final cons = await txn.rawQuery('SELECT SUM(ci.quantidade * a.valor_unitario) as total FROM consumo_item ci JOIN almoxarifado a ON ci.id_almoxarifado = a.id_almoxarifado WHERE ci.id_internacao = ?', [idInternacao]);
          valorConsumo = (cons.first['total'] as num? ?? 0).toDouble();
        }

        double totalBruto = valorHonorarios + valorExames + valorConsumo;
        print("Cálculos: Honorários: $valorHonorarios, Exames: $valorExames, Consumo: $valorConsumo, Total: $totalBruto");

        // 2. INSERT NO FATURAMENTO
        int idFaturamento = await txn.insert('faturamento', {
          'id_prontuario': idProntuario,
          'id_internacao': idInternacao,
          'valor_medicamentos': valorConsumo,
          'valor_exames': valorExames,
          'valor_honorarios': valorHonorarios,
          'valor_consumo': valorConsumo,
          'valor_total': totalBruto,
          'status_pagamento': 'PENDENTE',
          'data_fechamento': DateTime.now().toIso8601String()
        });
        print("Faturamento inserido com ID: $idFaturamento");

        // 3. ARQUIVAR PRONTUÁRIO
        await txn.update('prontuario', {'status_prontuario': 'ARQUIVADO'}, where: 'id_prontuario = ?', whereArgs: [idProntuario]);
        print("Prontuário $idProntuario arquivado.");

        // 4. LIBERAR RECURSO (SALA OU LEITO)
        if (idSala != null) {
          await txn.update('sala', {'status': 'LIVRE'}, where: 'id_sala = ?', whereArgs: [idSala]);
          print("Sala $idSala liberada.");
        }
        if (idLeito != null) {
          await txn.update('leito', {'situacao': 'HIGIENIZACAO'}, where: 'id_leito = ?', whereArgs: [idLeito]);
          await txn.update('internacao', {'status_internacao': 'ALTA'}, where: 'id_internacao = ?', whereArgs: [idInternacao]);
          print("Leito $idLeito liberado e Internação marcada como ALTA.");
        }
      });
      print("Transação de alta finalizada com sucesso.");
    } catch (e) {
      print("ERRO CRÍTICO NA TRANSAÇÃO DE ALTA: $e");
      rethrow; // Isso é importante para capturar na tela
    }
  }

  Future<List<Map<String, dynamic>>> listarFaturamentosComPaciente() async {
    return await db.rawQuery('''
      SELECT f.*, p.nome as nome_paciente, p.cpf
      FROM faturamento f
      LEFT JOIN prontuario pr ON f.id_prontuario = pr.id_prontuario
      LEFT JOIN paciente p ON pr.id_paciente = p.id_paciente
      ORDER BY f.data_fechamento DESC
    ''');
  }

  Future<void> marcarComoPago(int idFaturamento, int idProntuario) async {
    await db.transaction((txn) async {
      // 1. Atualiza status no faturamento
      await txn.update(
        'faturamento', 
        {'status_pagamento': 'PAGO'}, 
        where: 'id_faturamento = ?', 
        whereArgs: [idFaturamento]
      );
      
      // 2. Arquiva o prontuário
      await txn.update(
        'prontuario', 
        {'status_prontuario': 'ARQUIVADO'}, 
        where: 'id_prontuario = ?', 
        whereArgs: [idProntuario]
      );
    });
  }
}