
import 'package:flutter/material.dart';
import '../../../domain/entities/sala.dart';
import '../../../domain/services/sala_service.dart';
import '../../../domain/services/ala_service.dart';
import '../../../domain/entities/ala.dart';

class CadastrarSalas extends StatefulWidget {
  final SalaService salaService;
  final AlaService alaService;
  final Sala? salaEdicao;

  const CadastrarSalas({
    super.key,
    required this.salaService,
    required this.alaService,
    this.salaEdicao,
  });

  @override
  State<CadastrarSalas> createState() => _CadastrarSalasState();
}

class _CadastrarSalasState extends State<CadastrarSalas> {
  final _formKey = GlobalKey<FormState>();
  final _nomeController = TextEditingController();

  List<Ala> _alasDisponiveis = [];
  int? _idAlaSelecionada;
  String? _tipoSalaSelecionado;
  bool _carregando = true;
  bool _salvando = false;

  final List<String> _tiposSala = [
    'CONSULTÓRIO',
    'EXAME',
    'CIRÚRGICA',
    'EMERGÊNCIA',
    'REPOUSO',
    'ENFERMARIA',
  ];

  // Normaliza o texto para comparar tipos de sala.
  String _normalizarTipo(String tipo) {
    return tipo
        .trim()
        .toUpperCase()
        .replaceAll('Á', 'A')
        .replaceAll('À', 'A')
        .replaceAll('Ã', 'A')
        .replaceAll('Â', 'A')
        .replaceAll('É', 'E')
        .replaceAll('Ê', 'E')
        .replaceAll('Í', 'I')
        .replaceAll('Ó', 'O')
        .replaceAll('Ô', 'O')
        .replaceAll('Õ', 'O')
        .replaceAll('Ú', 'U')
        .replaceAll('Ç', 'C');
  }

  // Identifica o tipo correspondente na lista.
  String? _resolverTipo(String? tipo) {
    if (tipo == null || tipo.trim().isEmpty) {
      return null;
    }

    for (final opcao in _tiposSala) {
      if (_normalizarTipo(opcao) == _normalizarTipo(tipo)) {
        return opcao;
      }
    }

    // Preserva um tipo antigo que não esteja na lista.
    return tipo.trim();
  }

  @override
  void initState() {
    super.initState();

    if (widget.salaEdicao != null) {
      final sala = widget.salaEdicao!;

      _nomeController.text = sala.nomeSala ?? '';
      _tipoSalaSelecionado = _resolverTipo(sala.tipo);
      _idAlaSelecionada = sala.idAla;
    }

    _carregarAlas();
  }

  @override
  void dispose() {
    _nomeController.dispose();
    super.dispose();
  }

  Future<void> _carregarAlas() async {
    try {
      final alas = await widget.alaService.getAllAlas();

      if (!mounted) return;

      setState(() {
        // Evita alas duplicadas pelo identificador.
        final alasUnicas = <int, Ala>{};

        for (final ala in alas) {
          if (ala.id != null) {
            alasUnicas.putIfAbsent(ala.id!, () => ala);
          }
        }

        _alasDisponiveis = alasUnicas.values.toList();

        // Verifica se a ala selecionada ainda existe.
        if (_idAlaSelecionada != null &&
            !_alasDisponiveis.any(
              (ala) => ala.id == _idAlaSelecionada,
            )) {
          _idAlaSelecionada = null;
        }

        // No cadastro, seleciona a primeira ala disponível.
        if (widget.salaEdicao == null &&
            _idAlaSelecionada == null &&
            _alasDisponiveis.isNotEmpty) {
          _idAlaSelecionada = _alasDisponiveis.first.id;
        }

        _carregando = false;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _carregando = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('ERRO AO CARREGAR ALAS: $e'.toUpperCase()),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _salvar() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() {
      _salvando = true;
    });

    try {
      final sala = Sala(
        id: widget.salaEdicao?.id,
        nomeSala: _nomeController.text.trim(),
        tipo: _tipoSalaSelecionado,
        status: widget.salaEdicao?.status ?? 'LIVRE',
        idAla: _idAlaSelecionada,
      );

      await widget.salaService.salvar(sala);

      if (!mounted) return;

      Navigator.pop(context, true);
    } catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('ERRO AO SALVAR SALA: $e'.toUpperCase()),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      if (mounted) {
        setState(() {
          _salvando = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isEdicao = widget.salaEdicao != null;

    // Monta uma lista sem duplicações.
    final tiposDisponiveis = _tiposSala.toSet().toList();

    // Se o tipo antigo não estiver na lista, mantém disponível.
    if (_tipoSalaSelecionado != null &&
        !tiposDisponiveis.contains(_tipoSalaSelecionado)) {
      tiposDisponiveis.add(_tipoSalaSelecionado!);
    }

    return Container(
      height: double.infinity,
      padding: const EdgeInsets.all(24),
      decoration: const BoxDecoration(
        color: Colors.white,
      ),
      child: _carregando
          ? const Center(
              child: CircularProgressIndicator(color: Colors.teal),
            )
          : Form(
              key: _formKey,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  // Cabeçalho.
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        isEdicao ? 'EDITAR SALA' : 'NOVA SALA',
                        style: const TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.bold,
                          color: Colors.teal,
                        ),
                      ),
                      IconButton(
                        icon: const Icon(Icons.close),
                        onPressed: () => Navigator.pop(context),
                      ),
                    ],
                  ),

                  const Divider(),
                  const SizedBox(height: 16),

                  Expanded(
                    child: SingleChildScrollView(
                      child: Column(
                        children: [
                          // Nome da sala.
                          TextFormField(
                            controller: _nomeController,
                            decoration: const InputDecoration(
                              labelText: 'NOME DA SALA *',
                              prefixIcon: Icon(Icons.meeting_room),
                              border: OutlineInputBorder(),
                            ),
                            validator: (v) {
                              if (v == null || v.trim().isEmpty) {
                                return 'INFORME O NOME DA SALA';
                              }
                              return null;
                            },
                          ),

                          const SizedBox(height: 16),

                          // Tipo da sala.
                          DropdownButtonFormField<String>(
                            key: ValueKey(_tipoSalaSelecionado),
                            initialValue: _tipoSalaSelecionado,
                            isExpanded: true,
                            decoration: const InputDecoration(
                              labelText: 'TIPO DA SALA *',
                              prefixIcon: Icon(Icons.category),
                              border: OutlineInputBorder(),
                            ),
                            items: tiposDisponiveis.map((tipo) {
                              return DropdownMenuItem<String>(
                                value: tipo,
                                child: Text(tipo),
                              );
                            }).toList(),
                            onChanged: (valor) {
                              setState(() {
                                _tipoSalaSelecionado = valor;
                              });
                            },
                            validator: (value) {
                              if (value == null || value.isEmpty) {
                                return 'SELECIONE O TIPO DA SALA';
                              }
                              return null;
                            },
                          ),

                          const SizedBox(height: 16),

                          // Ala da sala.
                          DropdownButtonFormField<int>(
                            initialValue: _idAlaSelecionada,
                            isExpanded: true,
                            decoration: const InputDecoration(
                              labelText: 'SELECIONE A ALA *',
                              prefixIcon: Icon(Icons.apartment),
                              border: OutlineInputBorder(),
                            ),
                            items: _alasDisponiveis.map((ala) {
                              return DropdownMenuItem<int>(
                                value: ala.id,
                                child: Text(
                                  '${ala.nomeAla} - ${ala.andar ?? ''}'
                                      .toUpperCase(),
                                ),
                              );
                            }).toList(),
                            onChanged: (valor) {
                              setState(() {
                                _idAlaSelecionada = valor;
                              });
                            },
                            validator: (value) {
                              if (value == null) {
                                return 'SELECIONE A ALA';
                              }
                              return null;
                            },
                          ),
                        ],
                      ),
                    ),
                  ),

                  const SizedBox(height: 16),

                  // Botão de salvar.
                  ElevatedButton.icon(
                    style: ElevatedButton.styleFrom(
                      backgroundColor:
                          isEdicao ? Colors.blue : Colors.teal,
                      foregroundColor: Colors.white,
                      minimumSize: const Size(double.infinity, 50),
                    ),
                    icon: _salvando
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              color: Colors.white,
                              strokeWidth: 2,
                            ),
                          )
                        : Icon(isEdicao ? Icons.update : Icons.save),
                    label: Text(
                      _salvando
                          ? 'SALVANDO...'
                          : isEdicao
                              ? 'ATUALIZAR SALA'
                              : 'SALVAR SALA',
                      style: const TextStyle(fontSize: 16),
                    ),
                    onPressed: _salvando ? null : _salvar,
                  ),
                ],
              ),
            ),
    );
  }
}
