
import 'package:flutter/material.dart';

import '../../domain/entities/escala_medica.dart';
import '../../domain/entities/medico.dart';
import '../../domain/services/escala_medica_service.dart';
import '../../domain/services/medico_service.dart';

class CadastrarEscalaForm extends StatefulWidget {
  final EscalaService escalaService;
  final EscalaMedica? escalaParaEditar;

  const CadastrarEscalaForm({
    super.key,
    required this.escalaService,
    this.escalaParaEditar,
  });

  @override
  State<CadastrarEscalaForm> createState() =>
      _CadastrarEscalaFormState();
}

class _CadastrarEscalaFormState extends State<CadastrarEscalaForm> {
  final _formKey = GlobalKey<FormState>();

  // Serviço de médicos já integrado com a API
  late final MedicoService _medicoService;

  int? _idMedicoSelecionado;
  DateTime? _dataSelecionada;
  TimeOfDay? _horaInicio;
  TimeOfDay? _horaFim;

  bool _isPlantao = false;
  bool _salvando = false;

  List<Medico> _medicosAtivos = [];
  bool _carregandoMedicos = true;
  String? _erroCarregarMedicos;

  @override
  void initState() {
    super.initState();

    _medicoService = MedicoService();

    _carregarMedicos();

    // Preenche os dados para edição
    if (widget.escalaParaEditar != null) {
      final escala = widget.escalaParaEditar!;

      _idMedicoSelecionado = escala.idMedico;
      _dataSelecionada = DateTime.tryParse(escala.dataEscala);
      _horaInicio = _converterParaTimeOfDay(escala.horaInicio);
      _horaFim = _converterParaTimeOfDay(escala.horaFim);
      _isPlantao = escala.isPlantao;
    }
  }

  @override
  void dispose() {
    _medicoService.dispose();
    super.dispose();
  }

  TimeOfDay? _converterParaTimeOfDay(String? horario) {
    if (horario == null || horario.isEmpty) return null;

    try {
      final partes = horario.split(':');

      return TimeOfDay(
        hour: int.parse(partes[0]),
        minute: int.parse(partes[1]),
      );
    } catch (e) {
      return null;
    }
  }

  // Busca médicos ativos pela API
  Future<void> _carregarMedicos() async {
    try {
      final medicos = await _medicoService.listarMedicos();

      if (!mounted) return;

      setState(() {
        _medicosAtivos = medicos;
        _carregandoMedicos = false;
        _erroCarregarMedicos = null;
      });
    } catch (e) {
      debugPrint('Erro ao carregar médicos: $e');

      if (!mounted) return;

      setState(() {
        _carregandoMedicos = false;
        _erroCarregarMedicos = e.toString();
      });
    }
  }

  // Recupera o médico selecionado
  Medico? get _medicoSelecionado {
    try {
      return _medicosAtivos.firstWhere(
        (medico) => medico.id == _idMedicoSelecionado,
      );
    } catch (_) {
      return null;
    }
  }

  // Seletor de data
  Future<void> _selecionarData() async {
    final escolhida = await showDatePicker(
      context: context,
      initialDate: _dataSelecionada ?? DateTime.now(),
      firstDate: DateTime.now().subtract(
        const Duration(days: 365),
      ),
      lastDate: DateTime.now().add(
        const Duration(days: 365 * 2),
      ),
    );

    if (escolhida != null) {
      setState(() => _dataSelecionada = escolhida);
    }
  }

  // Seletor de horário
  Future<void> _selecionarHora(bool isInicio) async {
    final escolhida = await showTimePicker(
      context: context,
      initialTime: isInicio
          ? (_horaInicio ?? const TimeOfDay(hour: 8, minute: 0))
          : (_horaFim ?? const TimeOfDay(hour: 18, minute: 0)),
    );

    if (escolhida != null) {
      setState(() {
        if (isInicio) {
          _horaInicio = escolhida;
        } else {
          _horaFim = escolhida;
        }
      });
    }
  }

  String _formatarTimeOfDay(TimeOfDay? time) {
    if (time == null) return 'Não selecionado';

    final hora = time.hour.toString().padLeft(2, '0');
    final minuto = time.minute.toString().padLeft(2, '0');

    return '$hora:$minuto';
  }

  // Salva ou atualiza a escala pela API
  Future<void> _salvar() async {
    if (_salvando) return;

    if (!_formKey.currentState!.validate()) return;

    if (_dataSelecionada == null ||
        _horaInicio == null ||
        _horaFim == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Por favor, preencha a data e os horários!'),
          backgroundColor: Colors.orange,
        ),
      );
      return;
    }

    final medico = _medicoSelecionado;

    if (medico == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Selecione um médico válido.'),
          backgroundColor: Colors.orange,
        ),
      );
      return;
    }

    if (medico.crm == null || medico.crm!.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('O médico selecionado não possui CRM cadastrado.'),
          backgroundColor: Colors.orange,
        ),
      );
      return;
    }

    // Formata a data para YYYY-MM-DD
    final dataFormatada =
        '${_dataSelecionada!.year}-'
        '${_dataSelecionada!.month.toString().padLeft(2, '0')}-'
        '${_dataSelecionada!.day.toString().padLeft(2, '0')}';

    // Monta o objeto conforme o DTO da API C#
    final novaEscala = EscalaMedica(
      id: widget.escalaParaEditar?.id,
      idMedico: medico.id,
      crmMedico: medico.crm,
      nomeMedico: medico.nome,
      dataEscala: dataFormatada,
      horaInicio: _formatarTimeOfDay(_horaInicio),
      horaFim: _formatarTimeOfDay(_horaFim),
      isPlantao: _isPlantao,
    );

    setState(() => _salvando = true);

    try {
      if (widget.escalaParaEditar == null) {
        await widget.escalaService.cadastrarEscala(novaEscala);
      } else {
        await widget.escalaService.atualizarEscala(novaEscala);
      }

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            widget.escalaParaEditar == null
                ? 'Escala cadastrada com sucesso!'
                : 'Escala atualizada com sucesso!',
          ),
          backgroundColor: Colors.green,
        ),
      );

      Navigator.pop(context, true);
    } catch (e) {
      debugPrint('Erro ao salvar escala: $e');

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Erro ao salvar escala: $e'),
          backgroundColor: Colors.red,
          duration: const Duration(seconds: 6),
        ),
      );
    } finally {
      if (mounted) {
        setState(() => _salvando = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(
          widget.escalaParaEditar == null
              ? 'Adicionar Escala Médica'
              : 'Editar Escala Médica',
        ),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
      ),
      body: _carregandoMedicos
          ? const Center(
              child: CircularProgressIndicator(),
            )
          : _erroCarregarMedicos != null
              ? Center(
                  child: Padding(
                    padding: const EdgeInsets.all(20),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Icon(
                          Icons.error_outline,
                          color: Colors.red,
                          size: 40,
                        ),
                        const SizedBox(height: 12),
                        const Text(
                          'Não foi possível carregar os médicos.',
                          textAlign: TextAlign.center,
                        ),
                        const SizedBox(height: 12),
                        ElevatedButton(
                          onPressed: () {
                            setState(() {
                              _carregandoMedicos = true;
                              _erroCarregarMedicos = null;
                            });
                            _carregarMedicos();
                          },
                          child: const Text('Tentar novamente'),
                        ),
                      ],
                    ),
                  ),
                )
              : Padding(
                  padding: const EdgeInsets.all(20),
                  child: Form(
                    key: _formKey,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        // Seletor de médico
                        DropdownButtonFormField<int>(
                          decoration: const InputDecoration(
                            labelText: 'Médico Disponível *',
                            prefixIcon: Icon(
                              Icons.person,
                              color: Colors.teal,
                            ),
                          ),
                          initialValue: _medicosAtivos.any(
                            (m) => m.id == _idMedicoSelecionado,
                          )
                              ? _idMedicoSelecionado
                              : null,
                          items: _medicosAtivos
                              .where((m) => m.id != null)
                              .map((m) {
                            return DropdownMenuItem<int>(
                              value: m.id!,
                              child: Text(m.nome ?? 'Médico sem nome'),
                            );
                          }).toList(),
                          onChanged: (val) {
                            setState(() {
                              _idMedicoSelecionado = val;
                            });
                          },
                          validator: (value) => value == null
                              ? 'Selecione o Médico'
                              : null,
                        ),

                        const SizedBox(height: 20),

                        // Data
                        ListTile(
                          leading: const Icon(
                            Icons.calendar_month,
                            color: Colors.teal,
                          ),
                          title: const Text('Data da Escala'),
                          subtitle: Text(
                            _dataSelecionada == null
                                ? 'Clique para escolher...'
                                : '${_dataSelecionada!.day.toString().padLeft(2, '0')}/'
                                    '${_dataSelecionada!.month.toString().padLeft(2, '0')}/'
                                    '${_dataSelecionada!.year}',
                          ),
                          tileColor: Colors.grey.shade100,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                          onTap: _selecionarData,
                        ),

                        const SizedBox(height: 16),

                        // Horários
                        Row(
                          children: [
                            Expanded(
                              child: ListTile(
                                leading: const Icon(
                                  Icons.access_time,
                                  color: Colors.green,
                                ),
                                title: const Text('Início'),
                                subtitle: Text(
                                  _formatarTimeOfDay(_horaInicio),
                                ),
                                tileColor: Colors.grey.shade100,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(8),
                                ),
                                onTap: () => _selecionarHora(true),
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: ListTile(
                                leading: const Icon(
                                  Icons.access_time_filled,
                                  color: Colors.red,
                                ),
                                title: const Text('Término'),
                                subtitle: Text(
                                  _formatarTimeOfDay(_horaFim),
                                ),
                                tileColor: Colors.grey.shade100,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(8),
                                ),
                                onTap: () => _selecionarHora(false),
                              ),
                            ),
                          ],
                        ),

                        const SizedBox(height: 24),

                        // Plantão
                        SwitchListTile(
                          title: const Text(
                            'Médico de Plantão?',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          subtitle: const Text(
                            'Ative se for cobertura de urgência/emergência 24h',
                          ),
                          value: _isPlantao,
                          activeThumbColor: Colors.red,
                          secondary: Icon(
                            Icons.warning,
                            color: _isPlantao ? Colors.red : Colors.grey,
                          ),
                          onChanged: (val) {
                            setState(() => _isPlantao = val);
                          },
                        ),

                        const Spacer(),

                        ElevatedButton(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.teal,
                            foregroundColor: Colors.white,
                            padding: const EdgeInsets.symmetric(
                              vertical: 16,
                            ),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                          ),
                          onPressed: _salvando ? null : _salvar,
                          child: _salvando
                              ? const SizedBox(
                                  height: 22,
                                  width: 22,
                                  child: CircularProgressIndicator(
                                    strokeWidth: 2,
                                    color: Colors.white,
                                  ),
                                )
                              : const Text(
                                  'Salvar Escala no Calendário',
                                  style: TextStyle(fontSize: 16),
                                ),
                        ),
                      ],
                    ),
                  ),
                ),
    );
  }
}
