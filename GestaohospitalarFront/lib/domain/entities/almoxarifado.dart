
import 'entitie.dart';

class Almoxarifado extends Entitie {
  String nome;
  String? categoria;
  String? descricao;
  int quantidade;
  String? unidade;
  double valorUnitario;
  int estoqueMinimo;
  String? lote;
  DateTime? validade;

  // Dados exclusivos de medicamento
  int? idMedicamento;
  String? principioAtivo;
  String? contraindicacoes;

  Almoxarifado({
    super.id,
    required this.nome,
    this.categoria,
    this.descricao,
    this.quantidade = 0,
    this.unidade,
    required this.valorUnitario,
    required this.estoqueMinimo,
    this.lote,
    this.validade,
    this.idMedicamento,
    this.principioAtivo,
    this.contraindicacoes,
  });

  Map<String, dynamic> toJson() {
    return {
      'idAlmoxarifado': id,
      'idMedicamento': idMedicamento,
      'nome': nome,
      'categoria': categoria,
      'descricao': descricao,
      'quantidade': quantidade,
      'unidade': unidade,
      'valorUnitario': valorUnitario,
      'estoqueMinimo': estoqueMinimo,
      'lote': lote,
      'validade': validade?.toIso8601String(),
      'principioAtivo': principioAtivo,
      'contraindicacoes': contraindicacoes,
    };
  }

  factory Almoxarifado.fromJson(Map<String, dynamic> json) {
    return Almoxarifado(
      id: json['idAlmoxarifado'],
      nome: json['nome'] ?? '',
      categoria: json['categoria'],
      descricao: json['descricao'],
      quantidade: json['quantidade'] ?? 0,
      unidade: json['unidade'],
      valorUnitario: json['valorUnitario'] != null
          ? (json['valorUnitario'] as num).toDouble()
          : 0.0,
      estoqueMinimo: json['estoqueMinimo'] ?? 0,
      lote: json['lote'],
      validade: json['validade'] != null
          ? DateTime.tryParse(json['validade'].toString())
          : null,
      idMedicamento: json['idMedicamento'],
      principioAtivo: json['principioAtivo'],
      contraindicacoes: json['contraindicacoes'],
    );
  }
}
