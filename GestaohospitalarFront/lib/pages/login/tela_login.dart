
import 'package:flutter/material.dart';
import 'package:sqflite/sqflite.dart';

import '../dashboard.dart';
import '../dashboard_medico.dart';
import '../../domain/services/auth_service.dart';

class TelaLogin extends StatefulWidget {
  final Database database;

  const TelaLogin({
    super.key,
    required this.database,
  });

  @override
  State<TelaLogin> createState() => _TelaLoginState();
}

class _TelaLoginState extends State<TelaLogin> {
  final user = TextEditingController();
  final pass = TextEditingController();

  final AuthService _authService = AuthService();

  String tipoAcesso = 'MEDICO';
  bool _carregando = false;

  @override
  void dispose() {
    user.dispose();
    pass.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    if (_carregando) return;

    final usuarioDigitado = user.text.trim();
    final senhaDigitada = pass.text;

    if (usuarioDigitado.isEmpty || senhaDigitada.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Preencha todos os campos!'),
        ),
      );
      return;
    }

    setState(() => _carregando = true);

    try {
      // Administrador e médico são autenticados pela mesma API.
      final resultado = await _authService.autenticar(
        usuarioDigitado,
        senhaDigitada,
      );

      if (!mounted) return;

      if (resultado == null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text(
              'Usuário ou senha incorretos, ou usuário inativo.',
            ),
            backgroundColor: Colors.red,
          ),
        );
        return;
      }

      // Confere se o perfil retornado corresponde à opção escolhida.
      if (resultado.perfil != tipoAcesso) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              'Este usuário possui perfil ${resultado.perfil}. '
              'Selecione a opção correta.',
            ),
            backgroundColor: Colors.orange,
          ),
        );
        return;
      }

      // Guarda o token para utilização nos serviços da API.
      AuthSession.iniciar(resultado);

      if (resultado.perfil == 'ADMIN') {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (_) => Dashboard(
              database: widget.database,
            ),
          ),
        );
      } else if (resultado.perfil == 'MEDICO') {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (_) => DashboardMedico(
              database: widget.database,
            ),
          ),
        );
      }
    } catch (e) {
      if (!mounted) return;

      debugPrint('Erro no login: $e');

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Não foi possível realizar o login. '
            'Verifique a conexão com a API.',
          ),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      if (mounted) {
        setState(() => _carregando = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          Positioned.fill(
            child: Container(
              decoration: BoxDecoration(
                image: DecorationImage(
                  image: const AssetImage('assets/imagens/monge.jpeg'),
                  fit: BoxFit.contain,
                  colorFilter: ColorFilter.mode(
                    Colors.white.withValues(alpha: 0.08),
                    BlendMode.dstATop,
                  ),
                ),
              ),
            ),
          ),
          Center(
            child: SingleChildScrollView(
              child: SizedBox(
                width: 300,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      tipoAcesso == 'ADMIN'
                          ? 'LOGIN ADMINISTRADOR'
                          : 'LOGIN MÉDICO',
                      style: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                        color: Colors.teal,
                      ),
                    ),
                    const SizedBox(height: 20),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Radio<String>(
                          value: 'MEDICO',
                          groupValue: tipoAcesso,
                          onChanged: _carregando
                              ? null
                              : (value) {
                                  if (value != null) {
                                    setState(() => tipoAcesso = value);
                                  }
                                },
                        ),
                        const Text('Médico'),
                        const SizedBox(width: 20),
                        Radio<String>(
                          value: 'ADMIN',
                          groupValue: tipoAcesso,
                          onChanged: _carregando
                              ? null
                              : (value) {
                                  if (value != null) {
                                    setState(() => tipoAcesso = value);
                                  }
                                },
                        ),
                        const Text('Administrador'),
                      ],
                    ),
                    const SizedBox(height: 10),
                    TextField(
                      controller: user,
                      enabled: !_carregando,
                      textInputAction: TextInputAction.next,
                      decoration: InputDecoration(
                        labelText:
                            tipoAcesso == 'ADMIN' ? 'Usuário' : 'CRM',
                        border: const OutlineInputBorder(),
                        filled: true,
                        fillColor: Colors.white.withValues(alpha: 0.8),
                      ),
                    ),
                    const SizedBox(height: 12),
                    TextField(
                      controller: pass,
                      enabled: !_carregando,
                      obscureText: true,
                      textInputAction: TextInputAction.done,
                      onSubmitted: (_) => _login(),
                      decoration: InputDecoration(
                        labelText: 'Senha',
                        border: const OutlineInputBorder(),
                        filled: true,
                        fillColor: Colors.white.withValues(alpha: 0.8),
                      ),
                    ),
                    const SizedBox(height: 20),
                    ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.teal,
                        foregroundColor: Colors.white,
                        minimumSize: const Size(double.infinity, 45),
                      ),
                      onPressed: _carregando ? null : _login,
                      child: _carregando
                          ? const SizedBox(
                              width: 22,
                              height: 22,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: Colors.white,
                              ),
                            )
                          : const Text('Entrar'),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
