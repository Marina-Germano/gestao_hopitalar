import 'entitie.dart';

class Convenio extends Entitie {
  @override
  final int? id;
  final int? ativo;
  final String? nomeConvenio;
  final String? tipoLeito;
  final bool cobreInternacao;
  final bool cobreExames;
  final bool cobreCirurgia;
  final double? limiteMedicamento;
  final double? percentualCobertura;

  Convenio({
    this.id,
    this.ativo = 1,
    this.nomeConvenio,
    this.tipoLeito = 'COMUM',
    this.cobreInternacao = true,
    this.cobreExames = true,
    this.cobreCirurgia = true,
    this.limiteMedicamento,
    this.percentualCobertura,
  });

  Convenio copyWith({int? ativo}) {
    return Convenio(
      id: id,
      ativo: ativo ?? this.ativo,
      nomeConvenio: nomeConvenio,
      tipoLeito: tipoLeito,
      cobreInternacao: cobreInternacao,
      cobreExames: cobreExames,
      cobreCirurgia: cobreCirurgia,
      limiteMedicamento: limiteMedicamento,
      percentualCobertura: percentualCobertura,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'ativo': ativo ?? 1,
      'nomeConvenio': nomeConvenio,
      'tipoLeito': tipoLeito,
      // Convertendo booleano para int (1 ou 0) para o C# conseguir ler como System.Int32
      'cobreInternacao': cobreInternacao ? 1 : 0,
      'cobreExames': cobreExames ? 1 : 0,
      'cobreCirurgia': cobreCirurgia ? 1 : 0,
      'limiteMedicamento': limiteMedicamento,
      'percentualCobertura': percentualCobertura,
    };
  }

  factory Convenio.fromJson(Map<String, dynamic> json) {
    // Função auxiliar robusta para converter tanto bool quanto int (0 ou 1)
    bool parseBool(dynamic value) {
      if (value == null) return true;
      if (value is bool) return value;
      if (value is int) return value == 1;
      if (value is String) return value.toLowerCase() == 'true' || value == '1';
      return true;
    }

    return Convenio(
      id: json['idConvenio'] != null 
          ? int.tryParse(json['idConvenio'].toString()) 
          : (json['id'] != null ? int.tryParse(json['id'].toString()) : null),
      ativo: json['ativo'] != null ? int.tryParse(json['ativo'].toString()) : 1,
      nomeConvenio: json['nomeConvenio']?.toString() ?? json['nome_convenio']?.toString(),
      tipoLeito: json['tipoLeito']?.toString() ?? json['tipo_leito']?.toString() ?? 'COMUM',
      cobreInternacao: parseBool(json['cobreInternacao'] ?? json['cobre_internacao']),
      cobreExames: parseBool(json['cobreExames'] ?? json['cobre_exames']),
      cobreCirurgia: parseBool(json['cobreCirurgia'] ?? json['cobre_cirurgia']),
      limiteMedicamento: json['limiteMedicamento'] != null 
          ? (json['limiteMedicamento'] as num).toDouble() 
          : (json['limite_medicamento'] != null ? (json['limite_medicamento'] as num).toDouble() : null),
      percentualCobertura: json['percentualCobertura'] != null 
          ? (json['percentualCobertura'] as num).toDouble() 
          : (json['percentual_cobertura'] != null ? (json['percentual_cobertura'] as num).toDouble() : null),
    );
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is Convenio && other.id == id;
  }

  @override
  int get hashCode => id.hashCode;
}