
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../domain/entities/paciente.dart';
import '../../../domain/services/paciente_service.dart';
import '../../../domain/services/convenio_service.dart';
import '../convenios/cadastrar_convenio.dart';

class CadastrarPaciente extends StatefulWidget {
  final PacienteService service;
  final ConvenioService convenioService;
  final Paciente? pacienteEdicao;

  const CadastrarPaciente({
    super.key,
    required this.service,
    required this.convenioService,
    this.pacienteEdicao,
  });

  @override
  State<CadastrarPaciente> createState() => _CadastrarPacienteState();
}

class _CadastrarPacienteState extends State<CadastrarPaciente> {
  final _formKey = GlobalKey<FormState>();

  final _nomeCtrl = TextEditingController();
  final _cpfCtrl = TextEditingController();
  final _nascimentoCtrl = TextEditingController();
  final _alergiasCtrl = TextEditingController();
  final _tipoSanguineoCtrl = TextEditingController();
  final _historicoCtrl = TextEditingController();
  final _telefoneCtrl = TextEditingController();
  final _ruaCtrl = TextEditingController();
  final _numeroCasaCtrl = TextEditingController();
  final _bairroCtrl = TextEditingController();
  final _cidadeCtrl = TextEditingController();
  final _estadoCtrl = TextEditingController();
  final _cepCtrl = TextEditingController();
  final _responsavelCtrl = TextEditingController();

  String? _sexoSelecionado;
  DateTime? _dataNascimento;

  int? _idConvenioSelecionado;
  final _carteiraCtrl = TextEditingController();
  final _validadeCtrl = TextEditingController();
  DateTime? _validadeCarteira;

  @override
  void initState() {
    super.initState();

    if (widget.pacienteEdicao != null) {
      final p = widget.pacienteEdicao!;

      _nomeCtrl.text = p.nome;
      _cpfCtrl.text = p.cpf ?? '';
      _alergiasCtrl.text = p.alergias ?? '';
      _tipoSanguineoCtrl.text = p.tipoSanguineo ?? '';
      _historicoCtrl.text = p.historicoClinico ?? '';
      _telefoneCtrl.text = p.telefone ?? '';
      _ruaCtrl.text = p.rua ?? '';
      _numeroCasaCtrl.text = p.numeroCasa?.toString() ?? '';
      _bairroCtrl.text = p.bairro ?? '';
      _cidadeCtrl.text = p.cidade ?? '';
      _estadoCtrl.text = p.estado ?? '';
      _cepCtrl.text = p.cep ?? '';
      _responsavelCtrl.text = p.nomeResponsavel ?? '';

      if (p.sexo != null && p.sexo!.isNotEmpty) {
        final sexo = p.sexo!.toUpperCase();

        if (['MASCULINO', 'FEMININO', 'OUTRO'].contains(sexo)) {
          _sexoSelecionado = sexo;
        }
      }

      _dataNascimento = p.nascimento;

      if (_dataNascimento != null) {
        _nascimentoCtrl.text =
            "${_dataNascimento!.day.toString().padLeft(2, '0')}/"
            "${_dataNascimento!.month.toString().padLeft(2, '0')}/"
            "${_dataNascimento!.year}";
      }

      _prepararEdicao(p.id!);
    } else {
      widget.convenioService.carregarConvenios();
    }
  }

  Future<void> _prepararEdicao(int idPaciente) async {
    try {
      await widget.convenioService.carregarConvenios();

      if (!mounted) return;

      await _carregarConvenio(idPaciente);
    } catch (e) {
      debugPrint('Erro ao preparar edição do paciente: $e');
    }
  }

  Future<void> _carregarConvenio(int idPaciente) async {
    try {
      final vinculo =
          await widget.service.buscarVinculoConvenio(idPaciente);

      if (vinculo == null || !mounted) return;

      final idRecebido = int.tryParse(
        (vinculo['idConvenio'] ??
                vinculo['id_convenio'] ??
                vinculo['IdConvenio'])
            ?.toString() ??
            '',
      );

      final numeroCarteira =
          vinculo['numeroCarteira'] ??
          vinculo['numero_carteira'] ??
          vinculo['numero'];

      final validade = vinculo['validade'] ?? vinculo['Validade'];

      final idsDisponiveis = widget.convenioService.convenios
          .map((c) => c.id)
          .whereType<int>()
          .toSet();

      debugPrint('Convênio recebido da API: $idRecebido');
      debugPrint('IDs disponíveis: $idsDisponiveis');

      setState(() {
        _idConvenioSelecionado =
            idRecebido != null && idsDisponiveis.contains(idRecebido)
                ? idRecebido
                : null;

        _carteiraCtrl.text = numeroCarteira?.toString() ?? '';

        if (validade != null) {
          _validadeCarteira = DateTime.tryParse(validade.toString());

          if (_validadeCarteira != null) {
            _validadeCtrl.text =
                "${_validadeCarteira!.day.toString().padLeft(2, '0')}/"
                "${_validadeCarteira!.month.toString().padLeft(2, '0')}/"
                "${_validadeCarteira!.year}";
          }
        }
      });
    } catch (e) {
      debugPrint('Erro ao carregar convênio do paciente: $e');
    }
  }

  @override
  void dispose() {
    _nomeCtrl.dispose();
    _cpfCtrl.dispose();
    _nascimentoCtrl.dispose();
    _alergiasCtrl.dispose();
    _tipoSanguineoCtrl.dispose();
    _historicoCtrl.dispose();
    _telefoneCtrl.dispose();
    _ruaCtrl.dispose();
    _numeroCasaCtrl.dispose();
    _bairroCtrl.dispose();
    _cidadeCtrl.dispose();
    _estadoCtrl.dispose();
    _cepCtrl.dispose();
    _responsavelCtrl.dispose();
    _carteiraCtrl.dispose();
    _validadeCtrl.dispose();

    super.dispose();
  }

  Future<void> _selecionarData(BuildContext context) async {
    final escolhida = await showDatePicker(
      context: context,
      initialDate: _dataNascimento ?? DateTime(2000),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
    );

    if (escolhida != null) {
      setState(() {
        _dataNascimento = escolhida;
        _nascimentoCtrl.text =
            "${escolhida.day.toString().padLeft(2, '0')}/"
            "${escolhida.month.toString().padLeft(2, '0')}/"
            "${escolhida.year}";
      });
    }
  }

  Future<void> _selecionarValidade(BuildContext context) async {
    final escolhida = await showDatePicker(
      context: context,
      initialDate: _validadeCarteira ?? DateTime.now(),
      firstDate: DateTime.now(),
      lastDate: DateTime(2100),
    );

    if (escolhida != null) {
      setState(() {
        _validadeCarteira = escolhida;
        _validadeCtrl.text =
            "${escolhida.day.toString().padLeft(2, '0')}/"
            "${escolhida.month.toString().padLeft(2, '0')}/"
            "${escolhida.year}";
      });
    }
  }

  void _confirmarExclusao() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Excluir Paciente"),
        content: Text(
          "Tem certeza que deseja excluir o paciente ${_nomeCtrl.text}? "
          "Esta ação não pode ser desfeita.",
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancelar"),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red,
              foregroundColor: Colors.white,
            ),
            onPressed: () async {
              Navigator.pop(context);

              try {
                final id = widget.pacienteEdicao?.id;

                if (id != null) {
                  await widget.service.deletarPaciente(id);

                  if (mounted) {
                    Navigator.pop(context);

                    ScaffoldMessenger.of(context).showSnackBar(
                      const SnackBar(
                        content: Text('Paciente excluído com sucesso!'),
                        backgroundColor: Colors.red,
                      ),
                    );
                  }
                }
              } catch (e) {
                if (mounted) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('Erro ao excluir: $e'),
                      backgroundColor: Colors.red,
                    ),
                  );
                }
              }
            },
            child: const Text("Excluir"),
          ),
        ],
      ),
    );
  }

  Future<void> _salvarPaciente(bool isEdicao) async {
    if (!_formKey.currentState!.validate()) return;

    if (_idConvenioSelecionado != null && _validadeCarteira == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Por favor, informe a validade da carteira do convênio!',
          ),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    try {
      final novoPaciente = Paciente(
        id: widget.pacienteEdicao?.id,
        ativo: widget.pacienteEdicao?.ativo ?? 1,
        nome: _nomeCtrl.text.trim(),
        cpf: _cpfCtrl.text.trim(),
        sexo: _sexoSelecionado,
        nascimento: _dataNascimento,
        alergias:
            _alergiasCtrl.text.isEmpty ? null : _alergiasCtrl.text,
        tipoSanguineo: _tipoSanguineoCtrl.text.isEmpty
            ? null
            : _tipoSanguineoCtrl.text,
        historicoClinico:
            _historicoCtrl.text.isEmpty ? null : _historicoCtrl.text,
        telefone:
            _telefoneCtrl.text.isEmpty ? null : _telefoneCtrl.text,
        rua: _ruaCtrl.text.isEmpty ? null : _ruaCtrl.text,
        numeroCasa: _numeroCasaCtrl.text.isEmpty
            ? null
            : int.tryParse(_numeroCasaCtrl.text),
        bairro: _bairroCtrl.text.isEmpty ? null : _bairroCtrl.text,
        cidade: _cidadeCtrl.text.isEmpty ? null : _cidadeCtrl.text,
        estado: _estadoCtrl.text.isEmpty ? null : _estadoCtrl.text,
        cep: _cepCtrl.text.length == 8 ? _cepCtrl.text : null,
        nomeResponsavel:
            _responsavelCtrl.text.isEmpty ? null : _responsavelCtrl.text,
      );

      String? nomeConvenio;

      if (_idConvenioSelecionado != null) {
        for (final convenio in widget.convenioService.convenios) {
          if (convenio.id == _idConvenioSelecionado) {
            nomeConvenio = convenio.nomeConvenio;
            break;
          }
        }

        if (nomeConvenio == null) {
          throw Exception(
            'O convênio selecionado não foi encontrado. '
            'Atualize a lista de convênios e tente novamente.',
          );
        }
      }

      await widget.service.salvarPaciente(
        novoPaciente,
        idConvenio: _idConvenioSelecionado,
        nomeConvenio: nomeConvenio,
        numeroCarteira: _carteiraCtrl.text.trim().isEmpty
            ? null
            : _carteiraCtrl.text.trim(),
        validade: _validadeCarteira,
      );

      if (mounted) {
        Navigator.pop(context);

        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              isEdicao
                  ? 'Paciente atualizado!'
                  : 'Cadastrado com sucesso!',
            ),
            backgroundColor: isEdicao ? Colors.blue : Colors.teal,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Erro: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isEdicao = widget.pacienteEdicao != null;

    return Container(
      height: MediaQuery.of(context).size.height * 0.88,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.only(
          topLeft: Radius.circular(20),
          topRight: Radius.circular(20),
        ),
      ),
      padding: const EdgeInsets.all(24),
      child: Form(
        key: _formKey,
        child: Column(
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    Icon(
                      isEdicao ? Icons.edit : Icons.assignment_ind,
                      color: Colors.teal,
                      size: 28,
                    ),
                    const SizedBox(width: 8),
                    Text(
                      isEdicao
                          ? "Editar Paciente"
                          : "Cadastrar Paciente",
                      style: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                        color: Colors.teal,
                      ),
                    ),
                  ],
                ),
                IconButton(
                  icon: const Icon(Icons.close),
                  onPressed: () => Navigator.pop(context),
                ),
              ],
            ),
            const Divider(height: 30),
            Expanded(
              child: SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      "1. Identificação Pessoal",
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        color: Colors.grey,
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _nomeCtrl,
                      decoration: const InputDecoration(
                        labelText: "Nome Completo *",
                        prefixIcon: Icon(Icons.person),
                        border: OutlineInputBorder(),
                      ),
                      validator: (v) =>
                          v == null || v.isEmpty ? "Obrigatório" : null,
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _cpfCtrl,
                            keyboardType: TextInputType.number,
                            inputFormatters: [
                              FilteringTextInputFormatter.digitsOnly,
                            ],
                            decoration: const InputDecoration(
                              labelText: "CPF *",
                              prefixIcon: Icon(Icons.badge),
                              border: OutlineInputBorder(),
                            ),
                            validator: (v) =>
                                v == null || v.length != 11
                                    ? "11 dígitos"
                                    : null,
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _telefoneCtrl,
                            keyboardType: TextInputType.number,
                            inputFormatters: [
                              FilteringTextInputFormatter.digitsOnly,
                            ],
                            decoration: const InputDecoration(
                              labelText: "Telefone",
                              prefixIcon: Icon(Icons.phone),
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _nascimentoCtrl,
                            readOnly: true,
                            onTap: () => _selecionarData(context),
                            decoration: const InputDecoration(
                              labelText: "Nascimento *",
                              prefixIcon: Icon(Icons.calendar_today),
                              border: OutlineInputBorder(),
                            ),
                            validator: (v) =>
                                v == null || v.isEmpty ? "Selecione" : null,
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: DropdownButtonFormField<String>(
                            initialValue: _sexoSelecionado,
                            decoration: const InputDecoration(
                              labelText: "Sexo Biológico",
                              border: OutlineInputBorder(),
                            ),
                            items: const [
                              'MASCULINO',
                              'FEMININO',
                              'OUTRO',
                            ].map((s) {
                              return DropdownMenuItem<String>(
                                value: s,
                                child: Text(s),
                              );
                            }).toList(),
                            onChanged: (v) {
                              setState(() => _sexoSelecionado = v);
                            },
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _responsavelCtrl,
                      decoration: const InputDecoration(
                        labelText: "Responsável Legal",
                        prefixIcon: Icon(Icons.family_restroom),
                        border: OutlineInputBorder(),
                      ),
                    ),
                    const SizedBox(height: 24),
                    const Text(
                      "2. Endereço e Localização",
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        color: Colors.grey,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          flex: 3,
                          child: TextFormField(
                            controller: _ruaCtrl,
                            decoration: const InputDecoration(
                              labelText: "Rua",
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          flex: 1,
                          child: TextFormField(
                            controller: _numeroCasaCtrl,
                            keyboardType: TextInputType.number,
                            inputFormatters: [
                              FilteringTextInputFormatter.digitsOnly,
                            ],
                            decoration: const InputDecoration(
                              labelText: "Nº",
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _bairroCtrl,
                            decoration: const InputDecoration(
                              labelText: "Bairro",
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _cidadeCtrl,
                            decoration: const InputDecoration(
                              labelText: "Cidade",
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _estadoCtrl,
                            maxLength: 2,
                            decoration: const InputDecoration(
                              labelText: "Estado (UF)",
                              border: OutlineInputBorder(),
                              counterText: "",
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _cepCtrl,
                            keyboardType: TextInputType.number,
                            inputFormatters: [
                              FilteringTextInputFormatter.digitsOnly,
                            ],
                            decoration: const InputDecoration(
                              labelText: "CEP",
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 24),
                    const Text(
                      "3. Ficha Clínica Inicial",
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        color: Colors.grey,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _tipoSanguineoCtrl,
                            decoration: const InputDecoration(
                              labelText: "Tipo Sanguíneo",
                              prefixIcon: Icon(Icons.bloodtype),
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _alergiasCtrl,
                            decoration: const InputDecoration(
                              labelText: "Alergias",
                              prefixIcon: Icon(Icons.warning_amber),
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _historicoCtrl,
                      maxLines: 3,
                      decoration: const InputDecoration(
                        labelText: "Histórico Clínico",
                        alignLabelWithHint: true,
                        border: OutlineInputBorder(),
                      ),
                    ),
                    const SizedBox(height: 24),
                    const Text(
                      "4. Cobertura de Saúde (Opcional)",
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        color: Colors.grey,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: ListenableBuilder(
                            listenable: widget.convenioService,
                            builder: (context, _) {
                              // Mantém somente IDs válidos e sem duplicidade.
                              final idsAdicionados = <int>{};

                              final convenios = widget
                                  .convenioService.convenios
                                  .where((c) {
                                final id = c.id;
                                return id != null &&
                                    id > 0 &&
                                    idsAdicionados.add(id);
                              }).toList();

                              final valorSelecionado =
                                  _idConvenioSelecionado != null &&
                                          convenios.any(
                                            (c) =>
                                                c.id ==
                                                _idConvenioSelecionado,
                                          )
                                      ? _idConvenioSelecionado
                                      : null;

                              return DropdownButtonFormField<int>(
                                key: ValueKey(
                                  'convenio-${valorSelecionado ?? 'nenhum'}-${convenios.length}',
                                ),
                                initialValue: valorSelecionado,
                                decoration: const InputDecoration(
                                  labelText: "Convênio",
                                  border: OutlineInputBorder(),
                                  prefixIcon: Icon(Icons.business),
                                ),
                                hint: const Text("Nenhum / Particular"),
                                items: convenios.map((c) {
                                  return DropdownMenuItem<int>(
                                    value: c.id!,
                                    child: Text(c.nomeConvenio ?? ''),
                                  );
                                }).toList(),
                                onChanged: (v) {
                                  setState(() {
                                    _idConvenioSelecionado = v;
                                  });
                                },
                              );
                            },
                          ),
                        ),
                        const SizedBox(width: 12),
                        Container(
                          height: 56,
                          decoration: BoxDecoration(
                            color: Colors.teal,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: IconButton(
                            icon: const Icon(
                              Icons.add,
                              color: Colors.white,
                            ),
                            tooltip: "Adicionar Novo Convênio Rápido",
                            onPressed: () {
                              showModalBottomSheet(
                                context: context,
                                isScrollControlled: true,
                                backgroundColor: Colors.transparent,
                                builder: (context) => CadastrarConvenio(
                                  service: widget.convenioService,
                                ),
                              );
                            },
                          ),
                        ),
                      ],
                    ),
                    if (_idConvenioSelecionado != null) ...[
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          Expanded(
                            child: TextFormField(
                              controller: _carteiraCtrl,
                              decoration: const InputDecoration(
                                labelText: "Nº da Carteira",
                                border: OutlineInputBorder(),
                              ),
                            ),
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            child: TextFormField(
                              controller: _validadeCtrl,
                              readOnly: true,
                              onTap: () => _selecionarValidade(context),
                              decoration: const InputDecoration(
                                labelText: "Validade *",
                                prefixIcon: Icon(Icons.calendar_today),
                                border: OutlineInputBorder(),
                              ),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ],
                ),
              ),
            ),
            const Divider(),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (isEdicao)
                  OutlinedButton.icon(
                    style: OutlinedButton.styleFrom(
                      foregroundColor: Colors.red,
                      side: const BorderSide(color: Colors.red),
                      padding: const EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 16,
                      ),
                    ),
                    icon: const Icon(Icons.delete),
                    label: const Text("Excluir"),
                    onPressed: _confirmarExclusao,
                  )
                else
                  const SizedBox.shrink(),
                Row(
                  children: [
                    TextButton(
                      onPressed: () => Navigator.pop(context),
                      child: const Text(
                        "Cancelar",
                        style: TextStyle(color: Colors.grey),
                      ),
                    ),
                    const SizedBox(width: 16),
                    ElevatedButton.icon(
                      style: ElevatedButton.styleFrom(
                        backgroundColor:
                            isEdicao ? Colors.blue : Colors.teal,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(
                          horizontal: 24,
                          vertical: 16,
                        ),
                      ),
                      icon: Icon(
                        isEdicao ? Icons.update : Icons.save,
                      ),
                      label: Text(
                        isEdicao
                            ? "Atualizar Paciente"
                            : "Salvar Prontuário",
                      ),
                      onPressed: () => _salvarPaciente(isEdicao),
                    ),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
