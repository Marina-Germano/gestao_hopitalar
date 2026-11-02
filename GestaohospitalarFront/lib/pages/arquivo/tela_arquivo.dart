
import 'package:flutter/material.dart';

class TelaArquivo extends StatefulWidget {
  final String titulo;
  final Future<List<Map<String, dynamic>>> Function() carregarArquivados;
  final Future<void> Function(int id) restaurar;

  // Campos configuráveis para médicos e pacientes.
  final String campoDocumento;
  final String rotuloDocumento;
  final String hintBusca;

  const TelaArquivo({
    super.key,
    required this.titulo,
    required this.carregarArquivados,
    required this.restaurar,
    this.campoDocumento = 'crm',
    this.rotuloDocumento = 'CRM',
    this.hintBusca = 'Buscar por nome ou CRM',
  });

  static Future<bool> confirmarArquivamento({
    required BuildContext context,
    required String nomeItem,
  }) async {
    final confirmou = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text('Confirmar Arquivamento'),
        content: Text('Tem certeza que deseja arquivar "$nomeItem"?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancelar'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Arquivar'),
          ),
        ],
      ),
    );

    return confirmou ?? false;
  }

  @override
  State<TelaArquivo> createState() => _TelaArquivoState();
}

class _TelaArquivoState extends State<TelaArquivo> {
  List<Map<String, dynamic>> todos = [];
  String busca = '';
  bool carregando = true;
  String? erro;

  @override
  void initState() {
    super.initState();
    _carregar();
  }

  Future<void> _carregar() async {
    setState(() {
      carregando = true;
      erro = null;
    });

    try {
      final dados = await widget.carregarArquivados();

      if (!mounted) return;

      setState(() {
        todos = dados;
        carregando = false;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        erro = e.toString();
        carregando = false;
      });
    }
  }

  String _norm(String s) => s
      .toLowerCase()
      .replaceAll(RegExp('[áàâãä]'), 'a')
      .replaceAll(RegExp('[éèêë]'), 'e')
      .replaceAll(RegExp('[íìîï]'), 'i')
      .replaceAll(RegExp('[óòôõö]'), 'o')
      .replaceAll(RegExp('[úùûü]'), 'u')
      .replaceAll('ç', 'c');

  String _digitos(String s) => s.replaceAll(RegExp(r'\D'), '');

  List<Map<String, dynamic>> get filtrados {
    final q = busca.trim();

    if (q.isEmpty) return todos;

    final qNome = _norm(q);
    final qNum = _digitos(q);

    return todos.where((item) {
      final nomeOk = _norm('${item['nome'] ?? ''}').contains(qNome);

      final documento = '${item[widget.campoDocumento] ?? ''}';

      final documentoOk = qNum.isNotEmpty &&
          _digitos(documento).contains(qNum);

      return nomeOk || documentoOk;
    }).toList();
  }

  Future<void> _abrirItem(Map<String, dynamic> item) async {
    final id = item['id'];

    if (id is! int) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Não foi possível identificar este registro.'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    final restaurar = await showDialog<bool>(
      context: context,
      builder: (_) => AlertDialog(
        title: Text('${item['nome'] ?? 'Registro'}'),
        content: Text(
          '${widget.rotuloDocumento}: '
          '${item[widget.campoDocumento] ?? '-'}\n\n'
          'Deseja restaurar este registro?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Fechar'),
          ),
          FilledButton.icon(
            onPressed: () => Navigator.pop(context, true),
            icon: const Icon(Icons.restore),
            label: const Text('Restaurar'),
          ),
        ],
      ),
    );

    if (restaurar == true) {
      try {
        await widget.restaurar(id);

        if (!mounted) return;

        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Registro restaurado com sucesso!'),
            backgroundColor: Colors.green,
          ),
        );

        await _carregar();
      } catch (e) {
        if (!mounted) return;

        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Erro ao restaurar registro: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.titulo),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            TextField(
              decoration: InputDecoration(
                prefixIcon: const Icon(Icons.search),
                hintText: widget.hintBusca,
                border: const OutlineInputBorder(),
              ),
              onChanged: (v) => setState(() => busca = v),
            ),
            const SizedBox(height: 12),
            Expanded(
              child: carregando
                  ? const Center(child: CircularProgressIndicator())
                  : erro != null
                      ? Center(
                          child: Column(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text('Erro ao carregar registros:\n$erro'),
                              const SizedBox(height: 12),
                              ElevatedButton(
                                onPressed: _carregar,
                                child: const Text('Tentar novamente'),
                              ),
                            ],
                          ),
                        )
                      : filtrados.isEmpty
                          ? const Center(
                              child: Text(
                                'Nenhum registro arquivado encontrado',
                              ),
                            )
                          : ListView.builder(
                              itemCount: filtrados.length,
                              itemBuilder: (_, i) {
                                final item = filtrados[i];

                                return ListTile(
                                  leading: const Icon(Icons.person),
                                  title: Text('${item['nome'] ?? '-'}'),
                                  subtitle: Text(
                                    '${widget.rotuloDocumento}: '
                                    '${item[widget.campoDocumento] ?? '-'}',
                                  ),
                                  onTap: () => _abrirItem(item),
                                );
                              },
                            ),
            ),
          ],
        ),
      ),
    );
  }
}
