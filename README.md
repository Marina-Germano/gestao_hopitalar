# gestao_hopitalar
Implementando conexão com banco via API

# Gestão Hospitalar

## 1. Nome do projeto

**Gestão Hospitalar**

Sistema desenvolvido para auxiliar no gerenciamento de informações e processos hospitalares, integrando uma API REST desenvolvida em ASP.NET Core a um aplicativo desenvolvido em Flutter.

---

## 2. Integrantes da equipe

- ELY DEMARQUE JUNIOR 
- GABRIELLA MORENO SILVEIRA LOUZADA
- MARINA APARECIDA JACINTO GERMANO 
- NAIRA CAROLINA SILVEIRA VENANCIO

---

## 3. Descrição do sistema

O projeto Gestão Hospitalar tem como objetivo fornecer uma solução para o gerenciamento de informações hospitalares.

O sistema permite trabalhar com diferentes informações relacionadas ao ambiente hospitalar, incluindo usuários, médicos, pacientes, triagem, convênios, prontuários e internações.

A solução é composta por uma API REST, responsável pelo acesso e gerenciamento dos dados, e por um aplicativo Flutter, responsável pela interface de interação com o usuário.

A API utiliza autenticação baseada em **JWT (JSON Web Token)** para controlar o acesso aos recursos protegidos e utiliza **cache em memória** para melhorar o desempenho de determinadas consultas.

---

## 4. Tecnologias utilizadas

### Backend / API

- **C#**
- **.NET 9**
- **ASP.NET Core**
- **Entity Framework Core**
- **Pomelo EntityFrameworkCore MySQL**
- **MySQL**
- **Swagger**
- **JWT (JSON Web Token)**
- **IMemoryCache**


---

## 5. Estrutura da arquitetura

O projeto utiliza uma arquitetura organizada em camadas, separando responsabilidades entre controladores, serviços, modelos, DTOs e repositórios.

Estrutura principal da API:

Application/: Contém a lógica de aplicação e comunicação.
 DTOs/: Objetos de transferência de dados (Data Transfer Objects).
 Services/: Serviços responsável por orquestrar a lógica de negócio.

Controller/: Controladores que expõem os endpoints HTTP da API.
Core/: Núcleo da aplicação com o modelo de domínio e regras centrais.
Domain/: Regras de negócio e subpasta Repositories/ para acesso a dados.
Infra/: Configurações de infraestrutura e integração externa.
Models/: Entidades e modelos de dados do sistema.

Program.cs: Ficheiro de inicialização da aplicação e registo de serviços.
appsettings.json: Ficheiro com as definições de configuração do ambiente.
GestaoHospitalarApi.csproj: Ficheiro de projeto e dependências do .NET.


---

## 6. Instruções para execução da API

Antes de executar a API, é necessário possuir instalado:

- .NET 9 SDK
- MySQL
- Git
- Editor/IDE compatível com projetos .NET

### Clonando o projeto

No terminal:
git clone URL_DO_REPOSITORIO

Entre na pasta da API:
cd gestao_hopitalar/GestaoHospitalarApi

### Restaurando as dependências
dotnet restore

### Compilando o projeto
dotnet build

### Executando a API
dotnet run

Após a execução, a API estará disponível na porta configurada pelo projeto.

### Swagger

A documentação interativa dos endpoints pode ser acessada em:

http://localhost:5111/swagger


O Swagger permite visualizar os endpoints disponíveis e realizar testes das requisições HTTP.

---

## 7. Instruções para execução do aplicativo Flutter

### Pré-requisitos

É necessário possuir:

- Flutter SDK
- Dart SDK
- Editor compatível com Flutter
- Dispositivo ou emulador configurado

Verifique a instalação do Flutter com:

flutter doctor

Entre na pasta do aplicativo Flutter e execute:
flutter pub get

Para executar o aplicativo:
flutter run

O aplicativo deverá estar configurado para utilizar a API do projeto.


---

## 8. Informações sobre configuração do banco de dados

O sistema utiliza **MySQL** como banco de dados.

O banco utilizado pelo projeto possui o nome:

gestao_hospitalar

Execute o script SQL disponibilizado junto ao projeto no MySQL.

O script cria as tabelas e estruturas necessárias para o funcionamento do sistema.

A conexão com o banco deve ser configurada no arquivo:
appsettings.json

Após configurar o banco e a conexão, execute:

dotnet run

---

## 9. Informações sobre autenticação JWT

A API utiliza **JWT (JSON Web Token)** para autenticação e controle de acesso aos endpoints protegidos.

O login é realizado pelo endpoint:

POST /api/Usuarios/login

O cliente envia o login e a senha:

```json

{

  "login": "admin",

  "senha": "senha_do_usuario"

}

Quando as credenciais são válidas, a API gera e retorna um token JWT:

{

  "sucesso": true,

  "mensagem": "Login realizado com sucesso.",

  "dados": {

    "token": "eyJhbGciOiJIUzI1NiIs..."

  }

}

As senhas não são armazenadas em texto puro. Durante o cadastro e a alteração de senha, é utilizado o PasswordHasher<Usuario> para gerar um hash seguro. No login, a senha informada é validada por meio do VerifyHashedPassword.

O token JWT é gerado pelo serviço JwtService e contém informações do usuário, como identificador, login e perfil. O token utiliza assinatura HMAC-SHA256, além de configurações de emissor (Issuer), audiência (Audience) e tempo de expiração.

Os endpoints protegidos utilizam o atributo:

[Authorize]

Para acessá-los, o token deve ser enviado no cabeçalho da requisição:

Authorization: Bearer SEU_TOKEN_JWT

A API valida a assinatura, o emissor, a audiência e a validade do token. Requisições sem um token válido recebem a resposta HTTP 401 Unauthorized.

---


## 10. Descrição da estratégia de cache utilizada;

Foi implementado **cache em memória utilizando `IMemoryCache`** do ASP.NET Core.

A estratégia foi aplicada aos dados de **Paciente**, com o objetivo de reduzir consultas repetidas ao banco de dados e melhorar o desempenho da API.

Quando uma consulta é realizada:

1. A API verifica se o resultado está disponível no cache.
2. Caso esteja, os dados são retornados diretamente do cache.
3. Caso não esteja, a API consulta o banco de dados.
4. O resultado da consulta é armazenado no cache.
5. Nas próximas consultas compatíveis, o sistema pode reutilizar os dados armazenados.

Nas operações que alteram os dados do paciente, o cache relacionado deve ser atualizado ou invalidado para evitar que informações antigas sejam retornadas.

O cache é configurado na API por meio do serviço:
builder.Services.AddMemoryCache();

E utilizado no Controller por meio da injeção de:
IMemoryCache

O uso do cache busca:
- reduzir consultas desnecessárias ao banco;
- melhorar o tempo de resposta;
- diminuir o processamento de consultas repetidas;
- demonstrar a utilização de uma estratégia de cache na API.

---

## 11. Lista dos principais endpoints

Os endpoints abaixo representam as principais operações disponibilizadas pela API.

### Pacientes

GET    /api/Paciente
GET    /api/Paciente/{id}
POST   /api/Paciente
PUT    /api/Paciente/{id}
DELETE /api/Paciente/{id}

### Usuários

GET    /api/Usuarios
GET    /api/Usuarios/{id}
POST   /api/Usuarios
PUT    /api/Usuarios/{id}
DELETE /api/Usuarios/{id}
POST   /api/Usuarios/login

---

## 12. Exemplos de requisições e respostas

### Cadastro de paciente
Exemplo de requisição:

POST /api/Paciente
Content-Type: application/json

Exemplo de corpo:

  json
{
  "nome": "João da Silva",
  "cpf": "12345678900"
}


Exemplo de resposta:

  json
{
  "mensagem": "Paciente cadastrado com sucesso",
  "idPaciente": 1
}


### Consulta de pacientes

Requisição:
GET /api/Paciente

Exemplo de resposta:

  json
[
  {
    "idPaciente": 1,
    "nome": "João da Silva",
    "cpf": "12345678900"
  }
  ]


### Consulta de paciente por ID

Requisição:
GET /api/Paciente/1

Exemplo de resposta:

  json
{
  "idPaciente": 1,
  "nome": "João da Silva",
  "cpf": "12345678900"
}


---

## 13. Instruções necessárias para reprodução do projeto

Para reproduzir o projeto em outro computador, siga os passos abaixo.

### 1. Clonar o repositório
git clone URL_DO_REPOSITORIO

### 2. Configurar o banco de dados

- Instalar MySQL.
- Criar o banco `gestao_hospitalar`.
- Executar o script SQL do projeto.
- Configurar a `DefaultConnection` no `appsettings.json`.

### 3. Restaurar as dependências da API
cd GestaoHospitalarApi
dotnet restore

### 4. Compilar a API
dotnet build

### 5. Executar 
dotnet run

### 6. Acessar o Swagger
http://localhost:5111/swagger


### 7. Configurar o Flutter
Entrar na pasta do aplicativo:
cd CAMINHO_DO_PROJETO_FLUTTER

Instalar as dependências:
flutter pub get
Executar:
flutter run


Com a API e o aplicativo em execução, verificar se o Flutter está utilizando corretamente o endereço da API.

Informações sensíveis, como senhas do banco de dados e chaves secretas utilizadas na autenticação JWT, não devem ser armazenadas diretamente no repositório público.
