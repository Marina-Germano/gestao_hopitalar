
import 'package:flutter/material.dart';
import '../../domain/entities/escala_medica.dart';
import '../../domain/services/escala_medica_service.dart';
import 'cadastrar_escala_form.dart';

class AgendaMedicaTela extends StatefulWidget {
  const AgendaMedicaTela({super.key});

  @override
  State<AgendaMedicaTela> createState() => _AgendaMedicaTelaState();
}

class _AgendaMedicaTelaState extends State<AgendaMedicaTela> {
  final EscalaService _escalaService = EscalaService();

  DateTime _dataAtual = DateTime.now();
  List<EscalaMedica> _escalasDoMes = [];
  bool _carregando = true;

  @override
  void initState() {
    super.initState();
    _carregarEscalas();
  }

  // Carrega as escalas do mês pela API
  Future<void> _carregarEscalas() async {
    if (!mounted) return;

    setState(() => _carregando = true);

    try {
      final escalas = await _escalaService.buscarEscalaDoMes(
        _dataAtual.year,
        _dataAtual.month,
      );

      if (!mounted) return;

      setState(() {
        _escalasDoMes = escalas;
        _carregando = false;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _escalasDoMes = [];
        _carregando = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Erro ao carregar escalas: $e'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  void _mudarMes(int incremento) {
    setState(() {
      _dataAtual = DateTime(
        _dataAtual.year,
        _dataAtual.month + incremento,
        1,
      );
    });

    _carregarEscalas();
  }

  // Seleciona uma data específica
  Future<void> _selecionarData() async {
    final DateTime? escolhida = await showDatePicker(
      context: context,
      initialDate: _dataAtual,
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
      helpText: 'Buscar Data',
      cancelText: 'CANCELAR',
      confirmText: 'BUSCAR',
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: Colors.teal,
              onPrimary: Colors.white,
              onSurface: Colors.black,
            ),
          ),
          child: child!,
        );
      },
    );

    if (escolhida != null) {
      setState(() {
        _dataAtual = DateTime(
          escolhida.year,
          escolhida.month,
          1,
        );
      });

      await _carregarEscalas();

      if (mounted) {
        _mostrarDetalhesDoDia(escolhida);
      }
    }
  }

  // Compara a data da API com a data selecionada,
  // ignorando o horário que pode vir no DateTime.
  List<EscalaMedica> _escalasDoDia(DateTime dia) {
    return _escalasDoMes.where((e) {
      final dataEscala = DateTime.tryParse(e.dataEscala);

      if (dataEscala == null) {
        return false;
      }

      return dataEscala.year == dia.year &&
          dataEscala.month == dia.month &&
          dataEscala.day == dia.day;
    }).toList();
  }

  // Exibe os médicos escalados no dia selecionado
  void _mostrarDetalhesDoDia(DateTime dia) {
    final List<EscalaMedica> escalas = _escalasDoDia(dia);

    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(20),
        ),
      ),
      builder: (context) {
        return Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Center(
                child: Container(
                  width: 40,
                  height: 5,
                  decoration: BoxDecoration(
                    color: Colors.grey.shade400,
                    borderRadius: BorderRadius.circular(10),
                  ),
                ),
              ),
              const SizedBox(height: 16),
              Text(
                "Agenda do dia ${dia.day.toString().padLeft(2, '0')}/${dia.month.toString().padLeft(2, '0')}/${dia.year}",
                style: const TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: Colors.teal,
                ),
              ),
              const SizedBox(height: 16),

              if (escalas.isEmpty)
                const Padding(
                  padding: EdgeInsets.symmetric(vertical: 20),
                  child: Center(
                    child: Text(
                      "Nenhum médico escalado para este dia.",
                    ),
                  ),
                )
              else
                Flexible(
                  child: ListView.builder(
                    shrinkWrap: true,
                    itemCount: escalas.length,
                    itemBuilder: (context, index) {
                      final escala = escalas[index];

                      return ListTile(
                        leading: Icon(
                          Icons.person,
                          color: escala.isPlantao
                              ? Colors.red
                              : Colors.teal,
                        ),
                        title: Text(
                          escala.nomeMedico ?? "Médico",
                          style: const TextStyle(
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        subtitle: Text(
                          "${escala.horaInicio} às ${escala.horaFim}",
                        ),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            if (escala.isPlantao)
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 8,
                                  vertical: 4,
                                ),
                                margin: const EdgeInsets.only(right: 8),
                                decoration: BoxDecoration(
                                  color: Colors.red,
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                child: const Text(
                                  "PLANTÃO",
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 10,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),

                            // Editar escala
                            IconButton(
                              icon: const Icon(
                                Icons.edit,
                                color: Colors.blue,
                                size: 20,
                              ),
                              onPressed: () {
                                Navigator.pop(context);
                                _abrirCadastroEscala(
                                  escalaEditar: escala,
                                );
                              },
                            ),

                            // Excluir escala
                            IconButton(
                              icon: const Icon(
                                Icons.delete,
                                color: Colors.red,
                                size: 20,
                              ),
                              onPressed: () {
                                _confirmarExclusao(escala);
                              },
                            ),
                          ],
                        ),
                      );
                    },
                  ),
                ),
            ],
          ),
        );
      },
    );
  }

  // Confirma a exclusão da escala
  void _confirmarExclusao(EscalaMedica escala) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Excluir Escala'),
        content: Text(
          'Tem certeza que deseja remover o plantão de ${escala.nomeMedico}?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text('Cancelar'),
          ),
          TextButton(
            onPressed: () async {
              Navigator.pop(ctx);

              if (escala.id != null) {
                try {
                  await _escalaService.excluirEscala(escala.id!);

                  if (!mounted) return;

                  Navigator.pop(context);

                  await _carregarEscalas();

                  if (!mounted) return;

                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Escala removida com sucesso!'),
                      backgroundColor: Colors.red,
                    ),
                  );
                } catch (e) {
                  if (!mounted) return;

                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('Erro ao excluir escala: $e'),
                      backgroundColor: Colors.red,
                    ),
                  );
                }
              }
            },
            child: const Text(
              'Excluir',
              style: TextStyle(
                color: Colors.red,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ),
    );
  }

  // Abre o formulário lateral para cadastrar ou editar
  Future<void> _abrirCadastroEscala({
    EscalaMedica? escalaEditar,
  }) async {
    final atualizou = await showGeneralDialog<bool>(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Fechar",
      barrierColor: Colors.black54,
      transitionDuration: const Duration(milliseconds: 300),
      pageBuilder: (context, animation, secondaryAnimation) {
        return Align(
          alignment: Alignment.centerRight,
          child: Material(
            elevation: 10,
            color: Colors.white,
            child: SizedBox(
              width: 450,
              height: double.infinity,
              child: CadastrarEscalaForm(
                escalaService: _escalaService,
                escalaParaEditar: escalaEditar,
              ),
            ),
          ),
        );
      },
    );

    if (atualizou == true && mounted) {
      await _carregarEscalas();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Agenda & Plantões Médicos"),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
      ),
      body: Column(
        children: [
          // Navegação entre meses
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: 16.0,
              vertical: 8.0,
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                IconButton(
                  icon: const Icon(Icons.arrow_back_ios),
                  onPressed: () => _mudarMes(-1),
                ),
                InkWell(
                  onTap: _selecionarData,
                  borderRadius: BorderRadius.circular(8),
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 16.0,
                      vertical: 8.0,
                    ),
                    child: Row(
                      children: [
                        Text(
                          "${_nomeMes(_dataAtual.month)} ${_dataAtual.year}",
                          style: const TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(width: 8),
                        const Icon(
                          Icons.search,
                          color: Colors.teal,
                          size: 20,
                        ),
                      ],
                    ),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.arrow_forward_ios),
                  onPressed: () => _mudarMes(1),
                ),
              ],
            ),
          ),

          // Cabeçalho dos dias da semana
          const Padding(
            padding: EdgeInsets.symmetric(horizontal: 16.0),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                _DiaSemanaText("Dom"),
                _DiaSemanaText("Seg"),
                _DiaSemanaText("Ter"),
                _DiaSemanaText("Qua"),
                _DiaSemanaText("Qui"),
                _DiaSemanaText("Sex"),
                _DiaSemanaText("Sáb"),
              ],
            ),
          ),

          const SizedBox(height: 8),

          // Calendário
          Expanded(
            child: _carregando
                ? const Center(
                    child: CircularProgressIndicator(),
                  )
                : _buildCalendarioGrid(),
          ),
        ],
      ),

      floatingActionButton: FloatingActionButton(
        onPressed: _abrirCadastroEscala,
        backgroundColor: Colors.teal,
        child: const Icon(
          Icons.add,
          color: Colors.white,
        ),
      ),
    );
  }

  // Constrói a grade visual do calendário
  Widget _buildCalendarioGrid() {
    final int diasNoMes = DateTime(
      _dataAtual.year,
      _dataAtual.month + 1,
      0,
    ).day;

    final int primeiroDiaSemana = DateTime(
      _dataAtual.year,
      _dataAtual.month,
      1,
    ).weekday;

    final int offset = primeiroDiaSemana == 7
        ? 0
        : primeiroDiaSemana;

    return GridView.builder(
      padding: const EdgeInsets.all(16),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 7,
        childAspectRatio: 0.8,
        crossAxisSpacing: 8,
        mainAxisSpacing: 8,
      ),
      itemCount: diasNoMes + offset,
      itemBuilder: (context, index) {
        if (index < offset) {
          return const SizedBox.shrink();
        }

        final int diaNumero = index - offset + 1;

        final DateTime diaAtual = DateTime(
          _dataAtual.year,
          _dataAtual.month,
          diaNumero,
        );

        final List<EscalaMedica> escalasHoje =
            _escalasDoDia(diaAtual);

        final bool temPlantao =
            escalasHoje.any((e) => e.isPlantao);

        final bool temExpediente =
            escalasHoje.any((e) => !e.isPlantao);

        final bool isHoje =
            diaAtual.year == DateTime.now().year &&
            diaAtual.month == DateTime.now().month &&
            diaAtual.day == DateTime.now().day;

        return InkWell(
          onTap: () => _mostrarDetalhesDoDia(diaAtual),
          borderRadius: BorderRadius.circular(12),
          child: Container(
            decoration: BoxDecoration(
              color: isHoje ? Colors.teal.shade50 : Colors.white,
              border: Border.all(
                color: isHoje ? Colors.teal : Colors.grey.shade300,
              ),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  diaNumero.toString(),
                  style: TextStyle(
                    fontWeight:
                        isHoje ? FontWeight.bold : FontWeight.normal,
                    fontSize: 16,
                    color: isHoje ? Colors.teal : Colors.black87,
                  ),
                ),
                const SizedBox(height: 6),

                // Bolinhas que indicam expediente e plantão
                if (escalasHoje.isNotEmpty)
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      if (temExpediente)
                        _buildBolinha(Colors.green),
                      if (temExpediente && temPlantao)
                        const SizedBox(width: 4),
                      if (temPlantao)
                        _buildBolinha(Colors.red),
                    ],
                  ),
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildBolinha(Color cor) {
    return Container(
      width: 8,
      height: 8,
      decoration: BoxDecoration(
        color: cor,
        shape: BoxShape.circle,
      ),
    );
  }

  String _nomeMes(int mes) {
    const meses = [
      "Janeiro",
      "Fevereiro",
      "Março",
      "Abril",
      "Maio",
      "Junho",
      "Julho",
      "Agosto",
      "Setembro",
      "Outubro",
      "Novembro",
      "Dezembro",
    ];

    return meses[mes - 1];
  }
}

class _DiaSemanaText extends StatelessWidget {
  final String texto;

  const _DiaSemanaText(this.texto);

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 40,
      child: Text(
        texto,
        textAlign: TextAlign.center,
        style: const TextStyle(
          fontWeight: FontWeight.bold,
          color: Colors.grey,
        ),
      ),
    );
  }
}
