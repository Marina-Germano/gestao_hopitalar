
import 'package:flutter/material.dart';
import '../../../domain/entities/medico.dart';
import '../../../domain/services/medico_service.dart';

class CadastrarMedico extends StatefulWidget {
  final MedicoService service;
  final Medico? medicoEdicao;

  const CadastrarMedico({
    super.key,
    required this.service,
    this.medicoEdicao,
  });

  @override
  State<CadastrarMedico> createState() => _CadastrarMedicoState();
}

class _CadastrarMedicoState extends State<CadastrarMedico> {
  final _formKey = GlobalKey<FormState>();

  // Dados pessoais e profissionais
  final _nomeCtrl = TextEditingController();
  final _crmCtrl = TextEditingController();
  final _telefoneCtrl = TextEditingController();
  final _emailCtrl = TextEditingController();
  final _cpfCtrl = TextEditingController();
  final _nascimentoCtrl = TextEditingController();
  String? _sexoSelecionado;

  // Endereço
  final _cepCtrl = TextEditingController();
  final _ruaCtrl = TextEditingController();
  final _numeroCtrl = TextEditingController();
  final _bairroCtrl = TextEditingController();
  final _cidadeCtrl = TextEditingController();
  final _estadoCtrl = TextEditingController();

  // Acesso e outros dados
  final _honorarioCtrl = TextEditingController();
  final _senhaCtrl = TextEditingController();

  String? _perfilSelecionado;
  String? _especialidadeSelecionada;

  bool _carregandoDadosEdicao = false;
  bool _salvando = false;
  bool _falhaCarregamentoDadosEdicao = false;

  final List<String> _sexos = [
    'MASCULINO',
    'FEMININO',
    'OUTRO',
  ];

  final List<String> _perfisDisponiveis = [
    'ADMIN',
    'MEDICO',
    'ENFERMEIRO',
    'RECEPCAO',
    'FINANCEIRO',
    'PACIENTE',
  ];

  final List<String> _especialidadesDisponiveis = [
    'CARDIOLOGIA',
    'CLÍNICA GERAL',
    'DERMATOLOGIA',
    'GINECOLOGIA/OBSTETRÍCIA',
    'NEUROLOGIA',
    'OFTALMOLOGIA',
    'ORTOPEDIA',
    'PEDIATRIA',
    'PSIQUIATRIA',
    'UTI / INTENSIVISTA',
  ];

  @override
  void initState() {
    super.initState();

    final medico = widget.medicoEdicao;

    if (medico != null) {
      // Preenche inicialmente os dados disponíveis na listagem.
      _nomeCtrl.text = medico.nome ?? '';
      _crmCtrl.text = medico.crm ?? '';
      _telefoneCtrl.text = medico.telefone ?? '';
      _emailCtrl.text = medico.email ?? '';
      _honorarioCtrl.text = medico.honorario?.toString() ?? '';

      // Busca os dados completos na API.
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _carregarDadosCompletosParaEdicao();
      });
    }
  }

  dynamic _valorJson(
    Map<String, dynamic> dados,
    String chave,
  ) {
    final aliases = <String, List<String>>{
      'id': ['id_medico', 'idMedico'],
      'dataNascimento': [
        'data_nascimento',
        'nascimento',
        'dataDeNascimento',
      ],
      'numero': [
        'numero_casa',
        'numeroCasa',
        'numeroEndereco',
      ],
      'descricaoEspecialidade': [
        'descricao_especialidade',
        'especialidade',
        'nomeEspecialidade',
      ],
      'idEspecialidade': [
        'id_especialidade',
        'idEspecialidade',
      ],
      'honorario': [
        'valorHonorario',
        'valor_honorario',
      ],
    };

    final procuradas = <String>{
      chave.toLowerCase(),
      ...(aliases[chave] ?? []).map((item) => item.toLowerCase()),
    };

    for (final entrada in dados.entries) {
      if (procuradas.contains(entrada.key.toLowerCase())) {
        return entrada.value;
      }
    }

    return null;
  }

  String _textoJson(
    Map<String, dynamic> dados,
    String chave,
  ) {
    final valor = _valorJson(dados, chave);

    if (valor == null) return '';

    if (valor is Map) {
      final mapa = Map<String, dynamic>.from(valor);

      for (final nomeCampo in [
        'descricao',
        'nome',
        'descricaoEspecialidade',
        'descricao_especialidade',
      ]) {
        for (final entrada in mapa.entries) {
          if (entrada.key.toLowerCase() == nomeCampo.toLowerCase()) {
            return entrada.value?.toString() ?? '';
          }
        }
      }

      return '';
    }

    return valor.toString();
  }

  String? _encontrarOpcao(
    List<String> opcoes,
    String valor,
  ) {
    final procurado = _normalizar(valor);

    if (procurado.isEmpty) return null;

    for (final opcao in opcoes) {
      if (_normalizar(opcao) == procurado) {
        return opcao;
      }
    }

    return null;
  }

  String _normalizar(String valor) {
    return valor
        .trim()
        .toUpperCase()
        .replaceAll('Ç', 'C')
        .replaceAll('Ã', 'A')
        .replaceAll('Á', 'A')
        .replaceAll('Â', 'A')
        .replaceAll('É', 'E')
        .replaceAll('Ê', 'E')
        .replaceAll('Í', 'I')
        .replaceAll('Ó', 'O')
        .replaceAll('Ô', 'O')
        .replaceAll('Õ', 'O')
        .replaceAll('Ú', 'U')
        .replaceAll(RegExp(r'\s+'), ' ');
  }

  String _formatarData(dynamic valor) {
    if (valor == null || valor.toString().trim().isEmpty) {
      return '';
    }

    final texto = valor.toString().trim();

    // Formato DD/MM/AAAA.
    final partes = texto.split('/');

    if (partes.length == 3) {
      return '${partes[0].padLeft(2, '0')}/'
          '${partes[1].padLeft(2, '0')}/'
          '${partes[2]}';
    }

    // Formatos ISO, por exemplo 2000-12-31T00:00:00.
    final data = DateTime.tryParse(texto);

    if (data == null) return '';

    return '${data.day.toString().padLeft(2, '0')}/'
        '${data.month.toString().padLeft(2, '0')}/'
        '${data.year}';
  }

  Future<void> _carregarDadosCompletosParaEdicao() async {
    final id = widget.medicoEdicao?.id;

    if (id == null) {
      if (!mounted) return;

      setState(() {
        _falhaCarregamentoDadosEdicao = true;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Não foi possível identificar o médico para edição.',
          ),
          backgroundColor: Colors.red,
        ),
      );

      return;
    }

    setState(() {
      _carregandoDadosEdicao = true;
      _falhaCarregamentoDadosEdicao = false;
    });

    try {
      final dados =
          await widget.service.buscarDadosMedicoParaEdicao(id);

      if (!mounted) return;

      if (dados == null) {
        setState(() {
          _falhaCarregamentoDadosEdicao = true;
        });

        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Médico não encontrado na API.'),
            backgroundColor: Colors.red,
          ),
        );

        return;
      }

      _preencherCamposComDadosDaApi(dados);

      debugPrint('Formulário preenchido para o médico ID: $id');
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _falhaCarregamentoDadosEdicao = true;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Erro ao carregar os dados do médico: $e'),
          backgroundColor: Colors.red,
          duration: const Duration(seconds: 6),
        ),
      );
    } finally {
      if (mounted) {
        setState(() {
          _carregandoDadosEdicao = false;
        });
      }
    }
  }

  void _preencherCamposComDadosDaApi(
    Map<String, dynamic> dados,
  ) {
    void preencherTexto(
      TextEditingController controller,
      String chave,
    ) {
      final valor = _valorJson(dados, chave);

      if (valor != null) {
        controller.text = valor.toString();
      }
    }

    preencherTexto(_nomeCtrl, 'nome');
    preencherTexto(_crmCtrl, 'crm');
    preencherTexto(_cpfCtrl, 'cpf');
    preencherTexto(_telefoneCtrl, 'telefone');
    preencherTexto(_emailCtrl, 'email');
    preencherTexto(_cepCtrl, 'cep');
    preencherTexto(_ruaCtrl, 'rua');
    preencherTexto(_numeroCtrl, 'numero');
    preencherTexto(_bairroCtrl, 'bairro');
    preencherTexto(_cidadeCtrl, 'cidade');
    preencherTexto(_estadoCtrl, 'estado');
    preencherTexto(_honorarioCtrl, 'honorario');

    final nascimento = _valorJson(dados, 'dataNascimento');

    if (nascimento != null) {
      _nascimentoCtrl.text = _formatarData(nascimento);
    }

    final sexo = _textoJson(dados, 'sexo');
    final perfil = _textoJson(dados, 'perfil');
    final especialidade = _textoJson(
      dados,
      'descricaoEspecialidade',
    );

    setState(() {
      if (sexo.isNotEmpty) {
        _sexoSelecionado = _encontrarOpcao(_sexos, sexo);
      }

      if (perfil.isNotEmpty) {
        _perfilSelecionado = _encontrarOpcao(
          _perfisDisponiveis,
          perfil,
        );
      }

      if (especialidade.isNotEmpty) {
        final opcaoEncontrada = _encontrarOpcao(
          _especialidadesDisponiveis,
          especialidade,
        );

        // Mantém também especialidades existentes no banco
        // que ainda não estejam na lista fixa do Flutter.
        if (opcaoEncontrada == null &&
            !_especialidadesDisponiveis.contains(
              especialidade.trim(),
            )) {
          _especialidadesDisponiveis.add(especialidade.trim());
        }

        _especialidadeSelecionada =
            opcaoEncontrada ?? especialidade.trim();
      }
    });

    debugPrint('Dados recebidos para edição: $dados');
  }

  @override
  void dispose() {
    _nomeCtrl.dispose();
    _crmCtrl.dispose();
    _telefoneCtrl.dispose();
    _emailCtrl.dispose();
    _cpfCtrl.dispose();
    _nascimentoCtrl.dispose();
    _cepCtrl.dispose();
    _ruaCtrl.dispose();
    _numeroCtrl.dispose();
    _bairroCtrl.dispose();
    _cidadeCtrl.dispose();
    _estadoCtrl.dispose();
    _honorarioCtrl.dispose();
    _senhaCtrl.dispose();

    super.dispose();
  }

  Widget _buildSecaoTitulo(String titulo) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 12),
      child: Text(
        titulo,
        style: const TextStyle(
          fontSize: 16,
          fontWeight: FontWeight.bold,
          color: Colors.teal,
        ),
      ),
    );
  }

  Future<void> _excluirMedico() async {
    final confirmar = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Arquivar médico?'),
        content: const Text(
          'O médico será retirado da lista de ativos. '
          'O cadastro será mantido e poderá ser restaurado.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('CANCELAR'),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.orange,
              foregroundColor: Colors.white,
            ),
            onPressed: () => Navigator.pop(context, true),
            child: const Text('ARQUIVAR'),
          ),
        ],
      ),
    );

    if (confirmar != true || widget.medicoEdicao == null) {
      return;
    }

    try {
      await widget.service.arquivarMedico(widget.medicoEdicao!);

      if (!mounted) return;

      Navigator.pop(context);

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Médico arquivado com sucesso!'),
          backgroundColor: Colors.orange,
        ),
      );
    } catch (e) {
      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Erro ao arquivar médico: $e'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _salvarMedico(bool isEdicao) async {
    if (_salvando || _carregandoDadosEdicao) return;

    if (isEdicao && _falhaCarregamentoDadosEdicao) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Não foi possível carregar os dados completos. '
            'Feche o formulário e tente editar novamente.',
          ),
          backgroundColor: Colors.red,
        ),
      );

      return;
    }

    if (!_formKey.currentState!.validate()) return;

    final crm = _crmCtrl.text.trim();
    final cpf = _cpfCtrl.text.replaceAll(RegExp(r'\D'), '');
    final cep = _cepCtrl.text.replaceAll(RegExp(r'\D'), '');
    final dataTexto = _nascimentoCtrl.text.trim();
    final partesData = dataTexto.split('/');

    if (crm.isEmpty ||
        cpf.length != 11 ||
        _especialidadeSelecionada == null ||
        _sexoSelecionado == null ||
        _perfilSelecionado == null ||
        (!isEdicao && _senhaCtrl.text.trim().isEmpty)) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            isEdicao
                ? 'Verifique o CRM, o CPF, a especialidade, o sexo e o perfil.'
                : 'Preencha CRM, CPF válido, especialidade, sexo, perfil e senha.',
          ),
          backgroundColor: Colors.red,
        ),
      );

      return;
    }

    if (partesData.length != 3) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Informe o nascimento no formato DD/MM/AAAA.',
          ),
          backgroundColor: Colors.red,
        ),
      );

      return;
    }

    final dia = int.tryParse(partesData[0]);
    final mes = int.tryParse(partesData[1]);
    final ano = int.tryParse(partesData[2]);

    if (dia == null || mes == null || ano == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Data de nascimento inválida.'),
          backgroundColor: Colors.red,
        ),
      );

      return;
    }

    final nascimento = DateTime.tryParse(
      '${ano.toString().padLeft(4, '0')}-'
      '${mes.toString().padLeft(2, '0')}-'
      '${dia.toString().padLeft(2, '0')}',
    );

    if (nascimento == null ||
        nascimento.year != ano ||
        nascimento.month != mes ||
        nascimento.day != dia) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Data de nascimento inválida.'),
          backgroundColor: Colors.red,
        ),
      );

      return;
    }

    final honorario = double.tryParse(
      _honorarioCtrl.text.trim().replaceAll(',', '.'),
    );

    if (honorario == null || honorario < 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Informe um honorário válido.'),
          backgroundColor: Colors.red,
        ),
      );

      return;
    }

    final dados = <String, dynamic>{
      'nome': _nomeCtrl.text.trim(),
      'cpf': cpf,
      'telefone': _telefoneCtrl.text.trim().isEmpty
          ? null
          : _telefoneCtrl.text.trim(),
      'dataNascimento': nascimento.toIso8601String(),
      'sexo': _sexoSelecionado,
      'email': _emailCtrl.text.trim().isEmpty
          ? null
          : _emailCtrl.text.trim(),
      'cep': cep,
      'rua': _ruaCtrl.text.trim(),
      'numero': _numeroCtrl.text.trim(),
      'bairro': _bairroCtrl.text.trim(),
      'cidade': _cidadeCtrl.text.trim(),
      'estado': _estadoCtrl.text.trim().toUpperCase(),
      'crm': crm,
      'honorario': honorario,
      'descricaoEspecialidade': _especialidadeSelecionada,
      'perfil': _perfilSelecionado,
    };

    // No cadastro a senha é obrigatória.
    // Na edição, só envia uma senha quando o usuário informar uma nova.
    if (_senhaCtrl.text.trim().isNotEmpty) {
      dados['senha'] = _senhaCtrl.text;
    }

    setState(() {
      _salvando = true;
    });

    try {
      await widget.service.salvarMedicoComDados(
        id: widget.medicoEdicao?.id,
        dados: dados,
      );

      if (!mounted) return;

      Navigator.pop(context);

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            isEdicao
                ? 'Médico atualizado com sucesso!'
                : 'Médico cadastrado com sucesso!',
          ),
          backgroundColor: Colors.teal,
        ),
      );
    } catch (e) {
      if (!mounted) return;

      // 🟢 Mostra um Pop-up que fica travado na tela para podermos ler o erro!
      showDialog(
        context: context,
        builder: (ctx) => AlertDialog(
          title: const Row(
            children: [
              Icon(Icons.error_outline, color: Colors.red),
              SizedBox(width: 8),
              Text('Erro da API', style: TextStyle(color: Colors.red)),
            ],
          ),
          content: SingleChildScrollView(
            child: Text(
              'O backend C# recusou o salvamento:\n\n$e',
              style: const TextStyle(fontWeight: FontWeight.bold),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(ctx), 
              child: const Text('FECHAR')
            )
          ],
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
    final isEdicao = widget.medicoEdicao != null;

    return Container(
      height: double.infinity,
      padding: const EdgeInsets.all(24),
      decoration: const BoxDecoration(
        color: Colors.white,
      ),
      child: Form(
        key: _formKey,
        child: Column(
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  isEdicao ? 'EDITAR MÉDICO' : 'NOVO MÉDICO',
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
            if (_carregandoDadosEdicao || _salvando)
              const LinearProgressIndicator(),
            const Divider(),
            Expanded(
              child: SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _buildSecaoTitulo('INFORMAÇÕES BÁSICAS'),
                    TextFormField(
                      controller: _nomeCtrl,
                      decoration: const InputDecoration(
                        labelText: 'NOME COMPLETO *',
                        prefixIcon: Icon(Icons.person),
                        border: OutlineInputBorder(),
                      ),
                      validator: (v) =>
                          v == null || v.trim().isEmpty
                              ? 'CAMPO OBRIGATÓRIO'
                              : null,
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _crmCtrl,
                            decoration: const InputDecoration(
                              labelText: 'CRM *',
                              prefixIcon: Icon(Icons.badge),
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: DropdownButtonFormField<String>(
                            key: ValueKey(
                              'especialidade-$_especialidadeSelecionada',
                            ),
                            initialValue: _especialidadeSelecionada,
                            isExpanded: true,
                            decoration: const InputDecoration(
                              labelText: 'ESPECIALIDADE',
                              prefixIcon: Icon(Icons.local_hospital),
                              border: OutlineInputBorder(),
                            ),
                            items: _especialidadesDisponiveis
                                .map(
                                  (esp) => DropdownMenuItem<String>(
                                    value: esp,
                                    child: Text(
                                      esp,
                                      overflow: TextOverflow.ellipsis,
                                      maxLines: 1,
                                    ),
                                  ),
                                )
                                .toList(),
                            onChanged: (v) {
                              setState(() {
                                _especialidadeSelecionada = v;
                              });
                            },
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _cpfCtrl,
                            decoration: const InputDecoration(
                              labelText: 'CPF',
                              prefixIcon: Icon(Icons.pin),
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _telefoneCtrl,
                            decoration: const InputDecoration(
                              labelText: 'TELEFONE',
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
                            decoration: const InputDecoration(
                              labelText: 'NASCIMENTO (DD/MM/AAAA)',
                              prefixIcon: Icon(Icons.calendar_today),
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: DropdownButtonFormField<String>(
                            key: ValueKey('sexo-$_sexoSelecionado'),
                            initialValue: _sexoSelecionado,
                            isExpanded: true,
                            decoration: const InputDecoration(
                              labelText: 'SEXO',
                              prefixIcon: Icon(Icons.wc),
                              border: OutlineInputBorder(),
                            ),
                            items: _sexos
                                .map(
                                  (s) => DropdownMenuItem<String>(
                                    value: s,
                                    child: Text(
                                      s,
                                      overflow: TextOverflow.ellipsis,
                                      maxLines: 1,
                                    ),
                                  ),
                                )
                                .toList(),
                            onChanged: (v) {
                              setState(() {
                                _sexoSelecionado = v;
                              });
                            },
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _emailCtrl,
                      decoration: const InputDecoration(
                        labelText: 'E-MAIL',
                        prefixIcon: Icon(Icons.email),
                        border: OutlineInputBorder(),
                      ),
                    ),
                    const SizedBox(height: 16),
                    _buildSecaoTitulo('ENDEREÇO'),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _cepCtrl,
                            decoration: const InputDecoration(
                              labelText: 'CEP',
                              prefixIcon: Icon(Icons.map),
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          flex: 2,
                          child: TextFormField(
                            controller: _ruaCtrl,
                            decoration: const InputDecoration(
                              labelText: 'RUA',
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _numeroCtrl,
                            decoration: const InputDecoration(
                              labelText: 'NÚMERO',
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
                              labelText: 'BAIRRO',
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _cidadeCtrl,
                            decoration: const InputDecoration(
                              labelText: 'CIDADE',
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _estadoCtrl,
                            decoration: const InputDecoration(
                              labelText: 'ESTADO (UF)',
                              border: OutlineInputBorder(),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    _buildSecaoTitulo('ACESSO E PROFISSIONAL'),
                    Row(
                      children: [
                        Expanded(
                          child: DropdownButtonFormField<String>(
                            key: ValueKey('perfil-$_perfilSelecionado'),
                            initialValue: _perfilSelecionado,
                            isExpanded: true,
                            decoration: const InputDecoration(
                              labelText: 'PERFIL',
                              prefixIcon: Icon(Icons.security),
                              border: OutlineInputBorder(),
                            ),
                            items: _perfisDisponiveis
                                .map(
                                  (p) => DropdownMenuItem<String>(
                                    value: p,
                                    child: Text(
                                      p,
                                      overflow: TextOverflow.ellipsis,
                                      maxLines: 1,
                                    ),
                                  ),
                                )
                                .toList(),
                            onChanged: (v) {
                              setState(() {
                                _perfilSelecionado = v;
                              });
                            },
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: TextFormField(
                            controller: _senhaCtrl,
                            obscureText: true,
                            decoration: InputDecoration(
                              labelText: isEdicao
                                  ? 'NOVA SENHA (OPCIONAL)'
                                  : 'SENHA',
                              prefixIcon: const Icon(Icons.lock),
                              border: const OutlineInputBorder(),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _honorarioCtrl,
                      keyboardType: const TextInputType.numberWithOptions(
                        decimal: true,
                      ),
                      decoration: const InputDecoration(
                        labelText: 'HONORÁRIO',
                        prefixIcon: Icon(Icons.attach_money),
                        border: OutlineInputBorder(),
                      ),
                    ),
                    const SizedBox(height: 24),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),
            if (isEdicao) ...[
              ElevatedButton.icon(
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.orange,
                  foregroundColor: Colors.white,
                  minimumSize: const Size(double.infinity, 50),
                ),
                icon: const Icon(Icons.archive_outlined),
                label: const Text('ARQUIVAR MÉDICO'),
                onPressed: (_carregandoDadosEdicao || _salvando)
                    ? null
                    : _excluirMedico,
              ),
              const SizedBox(height: 12),
            ],
            ElevatedButton.icon(
              style: ElevatedButton.styleFrom(
                backgroundColor: isEdicao ? Colors.blue : Colors.teal,
                foregroundColor: Colors.white,
                minimumSize: const Size(double.infinity, 50),
              ),
              icon: Icon(isEdicao ? Icons.update : Icons.save),
              label: Text(
                _salvando
                    ? 'SALVANDO...'
                    : isEdicao
                        ? 'ATUALIZAR MÉDICO'
                        : 'SALVAR MÉDICO',
              ),
              onPressed: (_carregandoDadosEdicao || _salvando)
                  ? null
                  : () => _salvarMedico(isEdicao),
            ),
          ],
        ),
      ),
    );
  }
}
