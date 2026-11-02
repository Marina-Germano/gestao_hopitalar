
// lib/pages/medicos/listar_medico.dart
import 'package:flutter/material.dart';
import '../../../domain/entities/medico.dart';
import '../../../domain/services/medico_service.dart';
import 'cadastrar_medico.dart';

class ListarMedico extends StatefulWidget {
  final MedicoService service;

  const ListarMedico({
    super.key,
    required this.service,
  });

  @override
  State<ListarMedico> createState() => _ListarMedicoState();
}

class _ListarMedicoState extends State<ListarMedico> {
  List<Medico> _medicos = [];
  bool _isLoading = true;

  // Controle da pesquisa
  final TextEditingController _buscaCtrl = TextEditingController();
  String _termoBusca = '';

  @override
  void initState() {
    super.initState();
    _carregarMedicos();
  }

  @override
  void dispose() {
    _buscaCtrl.dispose();
    super.dispose();
  }

  Future<void> _carregarMedicos() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final medicos = await widget.service.listarMedicos();

      if (mounted) {
        setState(() {
          _medicos = medicos;
          _isLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isLoading = false;
        });

        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Erro ao carregar médicos: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    // Filtra os médicos pelo nome ou CRM
    final listaFiltrada = _medicos.where((medico) {
      final nome = (medico.nome ?? '').toLowerCase();
      final crm = medico.crm ?? '';
      final busca = _termoBusca.toLowerCase();

      return nome.contains(busca) || crm.contains(busca);
    }).toList();

    return Scaffold(
      appBar: AppBar(
        title: const Text('Central de Médicos'),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(70),
          child: Padding(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 12),
            child: TextField(
              controller: _buscaCtrl,
              onChanged: (valor) {
                setState(() {
                  _termoBusca = valor;
                });
              },
              style: const TextStyle(color: Colors.black87),
              decoration: InputDecoration(
                hintText: 'Buscar médico por nome ou CRM...',
                fillColor: Colors.white,
                filled: true,
                prefixIcon: const Icon(
                  Icons.search,
                  color: Colors.teal,
                ),
                suffixIcon: _termoBusca.isNotEmpty
                    ? IconButton(
                        icon: const Icon(
                          Icons.clear,
                          color: Colors.grey,
                        ),
                        onPressed: () {
                          _buscaCtrl.clear();
                          setState(() {
                            _termoBusca = '';
                          });
                        },
                      )
                    : null,
                contentPadding:
                    const EdgeInsets.symmetric(vertical: 0),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(30),
                  borderSide: BorderSide.none,
                ),
              ),
            ),
          ),
        ),
      ),

      body: _isLoading
          ? const Center(
              child: CircularProgressIndicator(color: Colors.teal),
            )
          : listaFiltrada.isEmpty
              ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(
                        Icons.search_off,
                        size: 60,
                        color: Colors.grey,
                      ),
                      const SizedBox(height: 16),
                      Text(
                        _termoBusca.isEmpty
                            ? 'Nenhum médico cadastrado.'
                            : 'Nenhum médico encontrado para "$_termoBusca".',
                        style: const TextStyle(
                          fontSize: 16,
                          color: Colors.grey,
                        ),
                      ),
                    ],
                  ),
                )
              : ListView.builder(
                  padding: const EdgeInsets.all(20),
                  itemCount: listaFiltrada.length,
                  itemBuilder: (context, index) {
                    final medico = listaFiltrada[index];
                    final nomeMedico = medico.nome ?? 'Sem Nome';

                    return Card(
                      elevation: 2,
                      margin: const EdgeInsets.symmetric(vertical: 6),
                      child: ListTile(
                        leading: const CircleAvatar(
                          backgroundColor: Colors.teal,
                          child: Icon(
                            Icons.person,
                            color: Colors.white,
                          ),
                        ),
                        title: Text(
                          nomeMedico,
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        subtitle: Text(
                          'CRM: ${medico.crm ?? 'Não informado'}',
                        ),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            // Editar médico
                            IconButton(
                              icon: const Icon(
                                Icons.edit,
                                color: Colors.blue,
                              ),
                              tooltip: 'Editar Médico',
                              onPressed: () async {
                                await showModalBottomSheet(
                                  context: context,
                                  isScrollControlled: true,
                                  backgroundColor: Colors.transparent,
                                  builder: (context) => CadastrarMedico(
                                    service: widget.service,
                                    medicoEdicao: medico,
                                  ),
                                );

                                _carregarMedicos();
                              },
                            ),

                            // Arquivar médico
                            IconButton(
                              icon: const Icon(
                                Icons.archive_outlined,
                                color: Colors.orange,
                              ),
                              tooltip: 'Arquivar Médico',
                              onPressed: () async {
                                final confirmar = await showDialog<bool>(
                                  context: context,
                                  builder: (context) => AlertDialog(
                                    title: const Text("Arquivar Médico?"),
                                    content: Text(
                                      "O médico $nomeMedico sairá desta lista ativa e será movido para o arquivo.",
                                    ),
                                    actions: [
                                      TextButton(
                                        onPressed: () =>
                                            Navigator.pop(context, false),
                                        child: const Text("Cancelar"),
                                      ),
                                      ElevatedButton(
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor: Colors.orange,
                                          foregroundColor: Colors.white,
                                        ),
                                        onPressed: () =>
                                            Navigator.pop(context, true),
                                        child: const Text("Arquivar"),
                                      ),
                                    ],
                                  ),
                                );

                                if (confirmar == true) {
                                  try {
                                    await widget.service.arquivarMedico(medico);
                                    await _carregarMedicos();

                                    if (context.mounted) {
                                      ScaffoldMessenger.of(context).showSnackBar(
                                        SnackBar(
                                          content: Text(
                                            'Médico $nomeMedico arquivado com sucesso!',
                                          ),
                                          backgroundColor: Colors.orange,
                                        ),
                                      );
                                    }
                                  } catch (e) {
                                    if (context.mounted) {
                                      ScaffoldMessenger.of(context).showSnackBar(
                                        SnackBar(
                                          content: Text('Erro ao arquivar: $e'),
                                          backgroundColor: Colors.red,
                                        ),
                                      );
                                    }
                                  }
                                }
                              },
                            ),

                            // Excluir médico permanentemente
                            IconButton(
                              icon: const Icon(
                                Icons.delete,
                                color: Color.fromARGB(255, 244, 67, 54),
                              ),
                              tooltip: "Excluir Médico",
                              onPressed: () async {
                                if (medico.id == null) return;

                                final confirmar = await showDialog<bool>(
                                  context: context,
                                  builder: (context) => AlertDialog(
                                    title: const Text("Excluir Médico?"),
                                    content: Text(
                                      "Tem certeza que deseja excluir permanentemente o cadastro de $nomeMedico? Esta ação não poderá ser desfeita.",
                                    ),
                                    actions: [
                                      TextButton(
                                        onPressed: () =>
                                            Navigator.pop(context, false),
                                        child: const Text("Cancelar"),
                                      ),
                                      ElevatedButton(
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor:
                                              const Color.fromARGB(255, 244, 67, 54),
                                          foregroundColor: Colors.white,
                                        ),
                                        onPressed: () =>
                                            Navigator.pop(context, true),
                                        child: const Text("Excluir"),
                                      ),
                                    ],
                                  ),
                                );

                                if (confirmar == true) {
                                  try {
                                    await widget.service.excluirMedico(medico.id!);
                                    await _carregarMedicos();

                                    if (context.mounted) {
                                      ScaffoldMessenger.of(context).showSnackBar(
                                        SnackBar(
                                          content: Text(
                                            'Médico $nomeMedico excluído permanentemente!',
                                          ),
                                          backgroundColor:
                                              const Color.fromARGB(255, 244, 67, 54),
                                        ),
                                      );
                                    }
                                  } catch (e) {
                                    if (context.mounted) {
                                      ScaffoldMessenger.of(context).showSnackBar(
                                        SnackBar(
                                          content: Text('Erro ao excluir: $e'),
                                          backgroundColor: Colors.red,
                                        ),
                                      );
                                    }
                                  }
                                }
                              },
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ),

      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: Colors.teal,
        icon: const Icon(
          Icons.person_add,
          color: Colors.white,
        ),
        label: const Text(
          'Registrar Médico',
          style: TextStyle(color: Colors.white),
        ),
        onPressed: () async {
          await showModalBottomSheet(
            context: context,
            isScrollControlled: true,
            backgroundColor: Colors.transparent,
            builder: (context) => CadastrarMedico(
              service: widget.service,
            ),
          );

          _carregarMedicos();
        },
      ),
    );
  }
}
