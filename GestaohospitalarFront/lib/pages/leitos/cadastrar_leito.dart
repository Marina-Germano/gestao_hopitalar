// // lib/pages/leitos/cadastrar_leito.dart
// import 'package:flutter/material.dart';
// import '../../domain/entities/leito.dart';
// import '../../domain/services/leito_service.dart';
// import '../../domain/services/ala_service.dart';
// import '../../domain/entities/ala.dart';

// class CadastrarLeito extends StatefulWidget {
//   final LeitoService leitoService;
//   final AlaService alaService;
//   final Leito? leitoEdicao; // Opcional para suportar edição

//   const CadastrarLeito({
//     super.key,
//     required this.leitoService,
//     required this.alaService,
//     this.leitoEdicao,
//   });

//   @override
//   State<CadastrarLeito> createState() => _CadastrarLeitoState();
// }

// class _CadastrarLeitoState extends State<CadastrarLeito> {
//   final _formKey = GlobalKey<FormState>();
//   final _numeroController = TextEditingController();
  
//   List<Ala> _alasDisponiveis = [];
//   int? _idAlaSelecionada;
//   String? _tipoSelecionado;
//   bool _carregando = true;

//   final List<String> _tiposLeito = ['COMUM', 'PRIVADO', 'PREMIUM', 'UTI', 'ISOLAMENTO'];

//   @override
//   void initState() {
//     super.initState();
//     if (widget.leitoEdicao != null) {
//       final l = widget.leitoEdicao!;
//       _numeroController.text = l.numero ?? '';
//       _idAlaSelecionada = l.idAla;
//       _tipoSelecionado = l.tipo; 
//     }
//     _carregarAlas();
//   }

//   @override
//   void dispose() {
//     _numeroController.dispose();
//     super.dispose();
//   }

//   Future<void> _carregarAlas() async {
//     try {
//       final alas = await widget.alaService.getAllAlas();
//       if (mounted) {
//         setState(() {
//           _alasDisponiveis = alas;
//           if (_idAlaSelecionada == null && alas.isNotEmpty) {
//             _idAlaSelecionada = alas.first.id;
//           }
//           _carregando = false;
//         });
//       }
//     } catch (e) {
//       if (mounted) {
//         setState(() => _carregando = false);
//         ScaffoldMessenger.of(context).showSnackBar(
//           SnackBar(content: Text('ERRO AO CARREGAR ALAS: $e'.toUpperCase()), backgroundColor: Colors.red),
//         );
//       }
//     }
//   }

//   void _salvar() async {
//     if (_formKey.currentState!.validate()) {
//       final novoLeito = Leito(
//         id: widget.leitoEdicao?.id,
//         numero: _numeroController.text,
//         idAla: _idAlaSelecionada,
//         tipo: _tipoSelecionado ?? 'COMUM',
//         situacao: widget.leitoEdicao?.situacao ?? 'VAGO',
//       );

//       try {
//         await widget.leitoService.cadastrarLeito(novoLeito);
//         if (mounted) {
//           ScaffoldMessenger.of(context).showSnackBar(
//             SnackBar(
//               content: Text(widget.leitoEdicao != null ? 'LEITO ATUALIZADO COM SUCESSO!' : 'LEITO CADASTRADO COM SUCESSO!'),
//               backgroundColor: Colors.teal,
//             ),
//           );
//           Navigator.pop(context, true);
//         }
//       } catch (e) {
//         if (mounted) {
//           ScaffoldMessenger.of(context).showSnackBar(
//             SnackBar(content: Text('ERRO AO SALVAR: $e'.toUpperCase()), backgroundColor: Colors.red),
//           );
//         }
//       }
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     final isEdicao = widget.leitoEdicao != null;

//     return Container(
//       height: double.infinity,
//       padding: const EdgeInsets.all(24),
//       decoration: const BoxDecoration(
//         color: Colors.white,
//       ),
//       child: _carregando 
//         ? const Center(child: CircularProgressIndicator(color: Colors.teal))
//         : Form(
//             key: _formKey,
//             child: Column(
//               crossAxisAlignment: CrossAxisAlignment.stretch,
//               children: [
//                 // Cabeçalho
//                 Row(
//                   mainAxisAlignment: MainAxisAlignment.spaceBetween,
//                   children: [
//                     Text(
//                       isEdicao ? "EDITAR LEITO" : "NOVO LEITO",
//                       style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: Colors.teal),
//                     ),
//                     IconButton(
//                       icon: const Icon(Icons.close),
//                       onPressed: () => Navigator.pop(context),
//                     ),
//                   ],
//                 ),
//                 const Divider(),
//                 const SizedBox(height: 16),

//                 // Formulário com Scroll
//                 Expanded(
//                   child: SingleChildScrollView(
//                     child: Column(
//                       children: [
//                         TextFormField(
//                           controller: _numeroController,
//                           decoration: const InputDecoration(
//                             labelText: 'NÚMERO DO LEITO (EX: 101-A) *',
//                             prefixIcon: Icon(Icons.bed),
//                             border: OutlineInputBorder(),
//                           ),
//                           validator: (value) => value!.isEmpty ? 'INFORME O NÚMERO DO LEITO' : null,
//                         ),
//                         const SizedBox(height: 16),
                        
//                         // Dropdown de Tipo de Leito
//                         DropdownButtonFormField<String>(
//                           initialValue: _tipoSelecionado,
//                           decoration: const InputDecoration(
//                             labelText: 'TIPO DO LEITO *',
//                             prefixIcon: Icon(Icons.category),
//                             border: OutlineInputBorder(),
//                           ),
//                           items: _tiposLeito.map((tipo) {
//                             return DropdownMenuItem<String>(
//                               value: tipo,
//                               child: Text(tipo),
//                             );
//                           }).toList(),
//                           onChanged: (val) => setState(() => _tipoSelecionado = val),
//                           validator: (value) => value == null ? 'SELECIONE O TIPO DO LEITO' : null,
//                         ),
//                         const SizedBox(height: 16),
                        
//                         // Dropdown Dinâmico de Ala
//                         DropdownButtonFormField<int>(
//                           initialValue: _idAlaSelecionada,
//                           decoration: const InputDecoration(
//                             labelText: 'SELECIONE A ALA *',
//                             prefixIcon: Icon(Icons.apartment),
//                             border: OutlineInputBorder(),
//                           ),
//                           items: _alasDisponiveis.map((ala) {
//                             return DropdownMenuItem<int>(
//                               value: ala.id,
//                               child: Text("${ala.nomeAla} - ${ala.andar ?? ''}".toUpperCase()),
//                             );
//                           }).toList(),
//                           onChanged: (val) => setState(() => _idAlaSelecionada = val),
//                           validator: (value) => value == null ? 'SELECIONE A ALA' : null,
//                         ),
//                       ],
//                     ),
//                   ),
//                 ),
                
//                 const SizedBox(height: 16),
                
//                 // Botão de Salvar
//                 ElevatedButton.icon(
//                   style: ElevatedButton.styleFrom(
//                     backgroundColor: isEdicao ? Colors.blue : Colors.teal,
//                     foregroundColor: Colors.white,
//                     minimumSize: const Size(double.infinity, 50),
//                   ),
//                   icon: Icon(isEdicao ? Icons.update : Icons.save),
//                   label: Text(isEdicao ? "ATUALIZAR LEITO" : "SALVAR LEITO", style: const TextStyle(fontSize: 16)),
//                   onPressed: _salvar,
//                 ),
//               ],
//             ),
//           ),
//     );
//   }
// }

import 'package:flutter/material.dart';
import '../../domain/entities/ala.dart';
import '../../domain/entities/leito.dart';
import '../../domain/services/ala_service.dart';
import '../../domain/services/leito_service.dart';

class CadastrarLeito extends StatefulWidget {
  final LeitoService leitoService;
  final AlaService alaService;
  final Map<String, dynamic>? leitoParaEdicao;

  const CadastrarLeito({
    super.key,
    required this.leitoService,
    required this.alaService,
    this.leitoParaEdicao,
  });

  @override
  State<CadastrarLeito> createState() => _CadastrarLeitoState();
}

class _CadastrarLeitoState extends State<CadastrarLeito> {
  final _formKey = GlobalKey<FormState>();
  final _numeroCtrl = TextEditingController();
  int? _idAlaSelecionada;
  String _situacao = 'VAGO';
  List<Ala> _alas = [];
  bool _carregando = true;
  bool _salvando = false;

  bool get _isEdicao => widget.leitoParaEdicao != null;

  @override
  void initState() {
    super.initState();
    _carregarAlas();
  }

  @override
  void dispose() {
    _numeroCtrl.dispose();
    super.dispose();
  }

  Future<void> _carregarAlas() async {
    try {
      final alas = await widget.alaService.getAllAlas();
      setState(() {
        _alas = alas;

        if (_isEdicao) {
          final dados = widget.leitoParaEdicao!;
          _numeroCtrl.text = dados['numero_leito']?.toString() ?? dados['numero']?.toString() ?? '';
          _idAlaSelecionada = dados['id_ala'] ?? dados['idAla'];
          _situacao = (dados['status_leito'] ?? dados['situacao'] ?? 'VAGO').toString().toUpperCase();
        } else if (_alas.isNotEmpty) {
          _idAlaSelecionada = _alas.first.id;
        }

        _carregando = false;
      });
    } catch (e) {
      if (mounted) setState(() => _carregando = false);
    }
  }

  Future<void> _salvar() async {
    if (!_formKey.currentState!.validate()) return;
    if (_idAlaSelecionada == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Selecione uma ala.')),
      );
      return;
    }

    setState(() => _salvando = true);

    try {
      final idLeito = _isEdicao
          ? (widget.leitoParaEdicao!['id_leito'] ?? widget.leitoParaEdicao!['idLeito'])
          : null;

      final leito = Leito(
        id: idLeito,
        numero: _numeroCtrl.text.trim(),
        idAla: _idAlaSelecionada,
        situacao: _situacao,
      );

      if (_isEdicao) {
        await widget.leitoService.atualizarLeito(idLeito, leito);
      } else {
        await widget.leitoService.cadastrarLeito(leito);
      }

      if (mounted) Navigator.pop(context, true);
    } catch (e) {
      if (mounted) {
        setState(() => _salvando = false);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro ao salvar leito: $e'), backgroundColor: Colors.red),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_isEdicao ? 'Editar Leito' : 'Novo Leito'),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
      ),
      body: _carregando
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(20.0),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    TextFormField(
                      controller: _numeroCtrl,
                      decoration: const InputDecoration(
                        labelText: 'Número do Leito',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.bed, color: Colors.teal),
                      ),
                      validator: (val) =>
                          val == null || val.trim().isEmpty ? 'Informe o número do leito' : null,
                    ),
                    const SizedBox(height: 16),
                    DropdownButtonFormField<int>(
                      value: _idAlaSelecionada,
                      decoration: const InputDecoration(
                        labelText: 'Ala Hospitalar',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.local_hospital, color: Colors.teal),
                      ),
                      items: _alas.map((ala) {
                        return DropdownMenuItem<int>(
                          value: ala.id,
                          child: Text('${ala.nomeAla} (${ala.andar})'),
                        );
                      }).toList(),
                      onChanged: (val) => setState(() => _idAlaSelecionada = val),
                    ),
                    const SizedBox(height: 16),
                    DropdownButtonFormField<String>(
                      value: _situacao,
                      decoration: const InputDecoration(
                        labelText: 'Situação do Leito',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.info_outline, color: Colors.teal),
                      ),
                      items: const [
                        DropdownMenuItem(value: 'VAGO', child: Text('Vago / Desocupado')),
                        DropdownMenuItem(value: 'HIGIENIZACAO', child: Text('Em Higienização')),
                        DropdownMenuItem(value: 'OCUPADO', child: Text('Ocupado')),
                      ],
                      onChanged: (val) => setState(() => _situacao = val ?? 'VAGO'),
                    ),
                    const Spacer(),
                    ElevatedButton.icon(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.teal,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 14),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                      ),
                      onPressed: _salvando ? null : _salvar,
                      icon: _salvando
                          ? const SizedBox(
                              width: 20,
                              height: 20,
                              child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                            )
                          : const Icon(Icons.save),
                      label: Text(
                        _isEdicao ? 'Atualizar Leito' : 'Cadastrar Leito',
                        style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                    ),
                  ],
                ),
              ),
            ),
    );
  }
}