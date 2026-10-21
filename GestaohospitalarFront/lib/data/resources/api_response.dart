class ApiResponse<T> {
  final bool sucesso;
  final String? mensagem;
  final T? dados;

  ApiResponse({required this.sucesso, this.mensagem, this.dados});

  factory ApiResponse.fromJson(Map<String, dynamic> json, T Function(dynamic) fromJsonT) {
    return ApiResponse(
      sucesso: json['sucesso'] ?? false,
      mensagem: json['mensagem'],
      dados: json['dados'] != null ? fromJsonT(json['dados']) : null,
    );
  }
}