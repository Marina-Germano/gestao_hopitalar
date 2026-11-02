
import 'package:flutter/material.dart';
import '../../../domain/services/paciente_service.dart';
import '../../../domain/services/convenio_service.dart';
import '../../../domain/entities/paciente.dart';
import 'cadastrar_paciente.dart';

class ListarPaciente extends StatefulWidget {
  final PacienteService service;
  final ConvenioService convenioService;

  const ListarPaciente({
    super.key,
    required this.service,
    required this.convenioService,
  });

  @override
  State<ListarPaciente> createState() => _ListarPacienteState();
}

class _ListarPacienteState extends State<ListarPaciente> {
  final TextEditingController _buscaCtrl = TextEditingController();

  String _termoBusca = '';
  bool _carregandoEdicao = false;
  bool _processandoAcao = false;

  @override
  void initState() {
    super.initState();
    widget.service.carregarPacientes();
  }

  @override
  void dispose() {
    _buscaCtrl.dispose();
    super.dispose();
  }

  void _abrirFormularioCadastro() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) => CadastrarPaciente(
        service: widget.service,
        convenioService: widget.convenioService,
      ),
    );
  }

  Future<void> _abrirFormularioEdicao(Paciente paciente) async {
    if (_carregandoEdicao || _processandoAcao) return;

    if (paciente.id == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Não foi possível identificar o paciente.'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    setState(() {
      _carregandoEdicao = true;
    });

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => const Center(
        child: CircularProgressIndicator(color: Colors.teal),
      ),
    );

    try {
      final pacienteCompleto =
          await widget.service.buscarPorId(paciente.id!);

      if (!mounted) return;

      Navigator.of(context, rootNavigator: true).pop();

      setState(() {
        _carregandoEdicao = false;
      });

      _abrirFormularioComDados(pacienteCompleto);
    } catch (e) {
      if (!mounted) return;

      Navigator.of(context, rootNavigator: true).pop();

      setState(() {
        _carregandoEdicao = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Erro ao buscar dados do paciente: $e'),
          backgroundColor: Colors.red,
          duration: const Duration(seconds: 5),
        ),
      );
    }
  }

  void _abrirFormularioComDados(Paciente paciente) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) => CadastrarPaciente(
        service: widget.service,
        convenioService: widget.convenioService,
        pacienteEdicao: paciente,
      ),
    );
  }

  // Arquiva o paciente diretamente pela API.
  Future<void> _arquivarPaciente(Paciente paciente) async {
    if (paciente.id == null || _processandoAcao) return;

    final confirmar = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Arquivar Paciente?'),
        content: Text(
          'O paciente ${paciente.nome} sairá da lista de ativos '
          'e será movido para o Arquivo de Pacientes.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancelar'),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.orange,
              foregroundColor: Colors.white,
            ),
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Arquivar'),
          ),
        ],
      ),
    );

    if (confirmar != true || !mounted) return;

    setState(() {
      _processandoAcao = true;
    });

    try {
      await widget.service.arquivarPaciente(paciente);

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Paciente ${paciente.nome} arquivado com sucesso!',
          ),
          backgroundColor: Colors.orange,
        ),
      );
    } catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Erro ao arquivar paciente: $e'),
          backgroundColor: Colors.red,
          duration: const Duration(seconds: 6),
        ),
      );
    } finally {
      if (mounted) {
        setState(() {
          _processandoAcao = false;
        });
      }
    }
  }

  // Exclui o paciente diretamente pela API.
  Future<void> _excluirPaciente(Paciente paciente) async {
    if (paciente.id == null || _processandoAcao) return;

    final confirmar = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Excluir Paciente?'),
        content: Text(
          'Tem certeza que deseja excluir permanentemente '
          'o cadastro de ${paciente.nome}?\n\n'
          'Esta ação não poderá ser desfeita.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancelar'),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red,
              foregroundColor: Colors.white,
            ),
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Excluir'),
          ),
        ],
      ),
    );

    if (confirmar != true || !mounted) return;

    setState(() {
      _processandoAcao = true;
    });

    try {
      await widget.service.deletarPaciente(paciente.id!);

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Paciente ${paciente.nome} excluído com sucesso!',
          ),
          backgroundColor: Colors.red,
        ),
      );
    } catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Erro ao excluir paciente: $e'),
          backgroundColor: Colors.red,
          duration: const Duration(seconds: 6),
        ),
      );
    } finally {
      if (mounted) {
        setState(() {
          _processandoAcao = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: widget.service,
      builder: (context, _) {
        if (widget.service.isLoading) {
          return const Scaffold(
            body: Center(
              child: CircularProgressIndicator(color: Colors.teal),
            ),
          );
        }

        final listaFiltrada = widget.service.pacientes.where((p) {
          final nome = p.nome.toLowerCase();
          final cpf = p.cpf ?? '';
          final busca = _termoBusca.toLowerCase();

          return nome.contains(busca) || cpf.contains(busca);
        }).toList();

        return Scaffold(
          appBar: AppBar(
            title: const Text('Central de Pacientes'),
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
                    hintText: 'Buscar paciente por nome ou CPF...',
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

          floatingActionButton: FloatingActionButton.extended(
            onPressed: _processandoAcao
                ? null
                : _abrirFormularioCadastro,
            backgroundColor: Colors.teal,
            icon: const Icon(
              Icons.person_add,
              color: Colors.white,
            ),
            label: const Text(
              'Registrar Paciente',
              style: TextStyle(color: Colors.white),
            ),
          ),

          body: listaFiltrada.isEmpty
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
                            ? 'Nenhum prontuário ativo encontrado.'
                            : 'Nenhum paciente encontrado para '
                                '"$_termoBusca".',
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
                  itemBuilder: (context, i) {
                    final p = listaFiltrada[i];

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
                          p.nome,
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        subtitle: Text(
                          'CPF: ${p.cpf ?? 'Não Informado'}',
                        ),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            if (p.tipoSanguineo != null)
                              Chip(
                                label: Text(p.tipoSanguineo!),
                                backgroundColor:
                                    Colors.red.withValues(alpha: 0.1),
                                labelStyle: const TextStyle(
                                  color: Colors.red,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),

                            const SizedBox(width: 8),

                            // Editar paciente.
                            IconButton(
                              icon: const Icon(
                                Icons.edit,
                                color: Colors.blue,
                              ),
                              tooltip: 'Editar Paciente',
                              onPressed:
                                  (_carregandoEdicao || _processandoAcao)
                                      ? null
                                      : () => _abrirFormularioEdicao(p),
                            ),

                            // Arquivar paciente.
                            IconButton(
                              icon: const Icon(
                                Icons.archive_outlined,
                                color: Colors.orange,
                              ),
                              tooltip: 'Arquivar Paciente',
                              onPressed: _processandoAcao
                                  ? null
                                  : () => _arquivarPaciente(p),
                            ),

                            // Excluir paciente.
                            IconButton(
                              style: ButtonStyle(
                                foregroundColor:
                                    WidgetStateProperty.all<Color>(
                                  const Color.fromARGB(255, 244, 67, 54),
                                ),
                                overlayColor:
                                    WidgetStateProperty.all<Color>(
                                  const Color.fromARGB(255, 244, 67, 54)
                                      .withValues(alpha: 0.1),
                                ),
                              ),
                              icon: const Icon(
                                Icons.delete,
                                color: Color.fromARGB(255, 244, 67, 54),
                              ),
                              tooltip: 'Excluir Paciente',
                              onPressed: _processandoAcao
                                  ? null
                                  : () => _excluirPaciente(p),
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ),
        );
      },
    );
  }
}
