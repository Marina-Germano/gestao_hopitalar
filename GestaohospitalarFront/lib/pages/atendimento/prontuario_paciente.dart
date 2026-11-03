import 'package:flutter/material.dart';
import '../../domain/entities/paciente.dart';
import '../../domain/services/prontuario_service.dart'; // Ajuste o caminho conforme a estrutura do seu projeto

DateTime _dataEntrada = DateTime.now();

class ProntuarioForm extends StatefulWidget {
  final Paciente paciente;
  final Map<String, dynamic> triagem; // Dados vindos da tabela triagem
  final dynamic database;

  const ProntuarioForm({super.key, required this.paciente, required this.triagem, required this.database});

  @override
  State<ProntuarioForm> createState() => _ProntuarioFormState();
}

class _ProntuarioFormState extends State<ProntuarioForm> {
  final _evolucaoCtrl = TextEditingController();
  String _internacao = 'NAO'; // Estado do toggle

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Prontuário Médico")),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [


            _buildCard("Data de Entrada", [
              Text(
                "${_dataEntrada.day.toString().padLeft(2, '0')}/${_dataEntrada.month.toString().padLeft(2, '0')}/${_dataEntrada.year} "
                "${_dataEntrada.hour.toString().padLeft(2, '0')}:${_dataEntrada.minute.toString().padLeft(2, '0')}",
                style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
              ),
            ]),

            // 1. DADOS DO PACIENTE
            _buildCard("Dados do Paciente", [
              Text("Nome: ${widget.paciente.nome}"),
              Text("CPF: ${widget.paciente.cpf}"),
              Text("Nascimento: ${widget.paciente.nascimento.toString().split(' ')[0]}"),
            ]),

            // 2. DADOS DA TRIAGEM (Sinais Vitais)
            _buildCard("Sinais Vitais (Triagem)", [
              Text("Pressão: ${widget.triagem['pressao']}"),
              Text("Temperatura: ${widget.triagem['temperatura']}°C"),
              Text("Saturação: ${widget.triagem['saturacao']}%"),
              Text("Queixa: ${widget.triagem['queixa']}"),
            ]),

            // 3. EVOLUÇÃO E CONDUTA
            TextFormField(
              controller: _evolucaoCtrl,
              maxLines: 5,
              decoration: const InputDecoration(labelText: "Evolução/Conduta", border: OutlineInputBorder()),
            ),

            // 4. DEFINIÇÃO DE INTERNAÇÃO
            const SizedBox(height: 20),
            const Text("Necessita de Internação?", style: TextStyle(fontWeight: FontWeight.bold)),
            Row(
              children: [
                Expanded(child: RadioListTile(title: const Text("Não"), value: 'NAO', groupValue: _internacao, onChanged: (v) => setState(() => _internacao = v!))),
                Expanded(child: RadioListTile(title: const Text("Sim"), value: 'SIM', groupValue: _internacao, onChanged: (v) => setState(() => _internacao = v!))),
              ],
            ),

            const SizedBox(height: 20),
            ElevatedButton(
              style: ElevatedButton.styleFrom(minimumSize: const Size(double.infinity, 50)),
              onPressed: _salvarProntuario,
              child: const Text("Salvar Prontuário"),
            )
          ],
        ),
      ),
    );
  }

  Widget _buildCard(String title, List<Widget> children) {
    return Card(
      margin: const EdgeInsets.only(bottom: 15),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
          Text(title, style: const TextStyle(fontWeight: FontWeight.bold, color: Colors.teal)),
          const Divider(),
          ...children
        ]),
      ),
    );
  }

  void _salvarProntuario() async {
    try {
      // 🟢 TRATAMENTO SEGURO DO ID
      final idRaw = widget.triagem['idTriagem'] ?? widget.triagem['id_triagem'] ?? widget.triagem['IdTriagem'];
      final int idTriagem = int.parse(idRaw.toString());

      final dto = {
        'idPaciente': widget.paciente.id,
        'idTriagem': idTriagem,
        'idMedico': 1, // FUTURO: Pegar do seu sistema de login
        'idSala': null, 
        'riscoEvasao': 'BAIXO',
        'evolucao': _evolucaoCtrl.text,
        'statusProntuario': 'ATIVO'
      };

      await ProntuarioService().cadastrarProntuario(dto);

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Prontuário salvo via API!"), backgroundColor: Colors.green),
        );
        Navigator.pop(context);
      }
    } catch (e) {
      debugPrint("ERRO AO SALVAR: $e"); 
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Erro ao salvar prontuário: $e"), backgroundColor: Colors.red),
      );
    }
  }
}