CREATE DATABASE gestao_hospitalar;
USE gestao_hospitalar;

CREATE TABLE usuario (
    id_usuario INTEGER PRIMARY KEY AUTO_INCREMENT,
    ativo INTEGER DEFAULT 1 CHECK (ativo IN (0, 1)),
    nome VARCHAR(150) NOT NULL,
    login VARCHAR(50) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    email VARCHAR(100) UNIQUE,
    perfil VARCHAR(50) NOT NULL CHECK (perfil IN ('ADMIN', 'MEDICO', 'ENFERMEIRO', 'RECEPCAO', 'FINANCEIRO')),
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE convenio (
    id_convenio INTEGER PRIMARY KEY AUTO_INCREMENT,
    ativo INTEGER DEFAULT 1 CHECK (ativo IN (0, 1)),
    nome_convenio VARCHAR(100) UNIQUE NOT NULL,
    tipo_leito VARCHAR(20) CHECK (tipo_leito IN ('COMUM', 'PRIVADO', 'PREMIUM')),
    cobre_internacao INTEGER DEFAULT 1 CHECK (cobre_internacao IN (0, 1)),
    cobre_exames INTEGER DEFAULT 1 CHECK (cobre_exames IN (0, 1)),
    cobre_cirurgia INTEGER DEFAULT 1 CHECK (cobre_cirurgia IN (0, 1)),
    limite_medicamento DECIMAL(10, 2),
    percentual_cobertura DECIMAL(5, 2)
);


CREATE TABLE especialidade (
    id_especialidade INTEGER PRIMARY KEY AUTO_INCREMENT,
    descricao_especialidade VARCHAR(100) NOT NULL
);

CREATE TABLE medico (
    id_medico INTEGER PRIMARY KEY AUTO_INCREMENT,
    ativo INTEGER DEFAULT 1 CHECK (ativo IN (0, 1)),
    id_especialidade INTEGER NOT NULL,
    id_usuario INTEGER UNIQUE, -- Vínculo opcional com a tabela usuario
    nome VARCHAR(150) NOT NULL,
    telefone VARCHAR(20),
    email VARCHAR(100),
    crm VARCHAR(20) UNIQUE NOT NULL,
    honorario DECIMAL(10, 2),
    FOREIGN KEY (id_especialidade) REFERENCES especialidade(id_especialidade),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);


CREATE TABLE escala_medica (
    id_escala INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_medico INTEGER NOT NULL,
    data_escala DATE NOT NULL,
    hora_inicio VARCHAR(5) NOT NULL, -- Formato 'HH:MM'
    hora_fim VARCHAR(5) NOT NULL,    -- Formato 'HH:MM'
    is_plantao INTEGER DEFAULT 0 CHECK (is_plantao IN (0, 1)),
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico)
);


CREATE TABLE paciente (
    id_paciente INTEGER PRIMARY KEY AUTO_INCREMENT,
    ativo INTEGER DEFAULT 1 CHECK (ativo IN (0, 1)),
    nome VARCHAR(150) NOT NULL,
    cpf VARCHAR(11) UNIQUE CHECK (length(cpf) = 11),
    sexo VARCHAR(10) CHECK (sexo IN ('MASCULINO', 'FEMININO', 'OUTRO')),
    nascimento DATE,
    alergias TEXT,
    tipo_sanguineo VARCHAR(5),
    historico_clinico TEXT,
    telefone VARCHAR(20),
    rua VARCHAR(150),
    numero_casa INTEGER,
    bairro VARCHAR(100),
    cidade VARCHAR(100),
    estado VARCHAR(2),
    cep VARCHAR(8) CHECK (length(cep) = 8),
    nome_responsavel VARCHAR(150)
);


CREATE TABLE paciente_convenio (
    id_paciente_convenio INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_paciente INTEGER NOT NULL,
    id_convenio INTEGER NOT NULL,
    numero_carteira VARCHAR(50),
    validade DATE NOT NULL,
    ativo INTEGER DEFAULT 1 CHECK (ativo IN (0, 1)),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente),
    FOREIGN KEY (id_convenio) REFERENCES convenio(id_convenio)
);


CREATE TABLE sala (
    id_sala INTEGER PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(50) NOT NULL,
    tipo VARCHAR(50),
    status VARCHAR(20) CHECK (status IN ('LIVRE', 'OCUPADA', 'MANUTENCAO'))
);


CREATE TABLE agendamento (
    id_agendamento INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_paciente INTEGER NOT NULL,
    id_medico INTEGER NOT NULL,
    id_sala INTEGER,
    data_hora TIMESTAMP NOT NULL,
    status VARCHAR(20) DEFAULT 'AGENDADO' CHECK (status IN ('AGENDADO', 'CONFIRMADO', 'CANCELADO', 'FINALIZADO')),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente),
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico),
    FOREIGN KEY (id_sala) REFERENCES sala(id_sala)
);


CREATE TABLE triagem (
    id_triagem INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_paciente INTEGER NOT NULL,
    responsavel_triagem VARCHAR(150),
    pressao VARCHAR(10),
    temperatura DECIMAL(4, 1),
    frequencia_cardiaca INTEGER,
    saturacao INTEGER,
    escala_dor INTEGER CHECK (escala_dor BETWEEN 0 AND 10),
    risco VARCHAR(10) CHECK (risco IN ('VERMELHO', 'LARANJA', 'AMARELO', 'VERDE', 'AZUL')),
    queixa TEXT,
    alergias TEXT,
    observacoes TEXT,
    internacao VARCHAR(3) CHECK (internacao IN ('SIM', 'NAO')),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente)
);


CREATE TABLE prontuario (
    id_prontuario INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_paciente INTEGER NOT NULL,
    id_triagem INTEGER NOT NULL,
    id_medico INTEGER NOT NULL,
    id_sala INTEGER,
    risco_evasao VARCHAR(3) NOT NULL CHECK (risco_evasao IN ('SIM', 'NAO')),
    isolamento VARCHAR(3) NOT NULL CHECK (isolamento IN ('SIM', 'NAO')),
    evolucao TEXT,
    data_abertura TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status_prontuario VARCHAR(10) DEFAULT 'ATIVO' CHECK (status_prontuario IN ('ATIVO', 'ARQUIVADO')),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente),
    FOREIGN KEY (id_triagem) REFERENCES triagem(id_triagem),
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico),
    FOREIGN KEY (id_sala) REFERENCES sala(id_sala)
);


CREATE TABLE leito (
    id_leito INTEGER PRIMARY KEY AUTO_INCREMENT,
    numero VARCHAR(10) NOT NULL,
    ala VARCHAR(50),
    andar VARCHAR(10),
    data_higienizacao TIMESTAMP,
    situacao VARCHAR(20) DEFAULT 'VAGO' CHECK (situacao IN ('VAGO', 'OCUPADO', 'HIGIENIZACAO'))
);


CREATE TABLE internacao (
    id_internacao INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INTEGER NOT NULL,
    id_leito INTEGER NOT NULL,
    data_entrada TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alta TIMESTAMP NULL DEFAULT NULL,
    isolamento VARCHAR(3) CHECK (isolamento IN ('SIM', 'NAO')),
    status_internacao VARCHAR(15) DEFAULT 'ATIVA' CHECK (status_internacao IN ('ATIVA', 'ALTA', 'TRANSFERIDO')),
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario),
    FOREIGN KEY (id_leito) REFERENCES leito(id_leito)
);


CREATE TABLE almoxarifado (
    id_almoxarifado INTEGER PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    categoria VARCHAR(20) NOT NULL CHECK (categoria IN ('MEDICAMENTO', 'DESCARTAVEL', 'LIMPEZA', 'EPI', 'INSUMO')),
    descricao TEXT,
    quantidade INTEGER NOT NULL DEFAULT 0,
    unidade VARCHAR(10),
    valor_unitario DECIMAL(10, 2) NOT NULL,
    estoque_minimo INTEGER DEFAULT 0,
    lote VARCHAR(50),
    validade DATE
);


CREATE TABLE medicamento (
    id_medicamento INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_almoxarifado INTEGER NOT NULL,
    principio_ativo VARCHAR(150),
    contraindicacoes TEXT,
    FOREIGN KEY (id_almoxarifado) REFERENCES almoxarifado(id_almoxarifado)
);


CREATE TABLE interacao_medicamentosa (
    id_interacao INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_medicamento_1 INTEGER NOT NULL,
    id_medicamento_2 INTEGER NOT NULL,
    gravidade VARCHAR(10) CHECK (gravidade IN ('LEVE', 'MODERADA', 'GRAVE')),
    descricao TEXT,
    FOREIGN KEY (id_medicamento_1) REFERENCES medicamento(id_medicamento),
    FOREIGN KEY (id_medicamento_2) REFERENCES medicamento(id_medicamento)
);


CREATE TABLE prescricao (
    id_prescricao INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INTEGER NOT NULL,
    id_medico INTEGER NOT NULL,
    id_medicamento INTEGER NOT NULL,
    dosagem VARCHAR(50),
    aplicacao VARCHAR(50),
    horario VARCHAR(50) NOT NULL,
    observacao TEXT,
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario),
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico),
    FOREIGN KEY (id_medicamento) REFERENCES medicamento(id_medicamento)
);


CREATE TABLE consumo_item (
    id_consumo INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_internacao INTEGER NOT NULL,
    id_almoxarifado INTEGER NOT NULL,
    quantidade INTEGER NOT NULL,
    data_consumo TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    observacao TEXT,
    FOREIGN KEY (id_internacao) REFERENCES internacao(id_internacao),
    FOREIGN KEY (id_almoxarifado) REFERENCES almoxarifado(id_almoxarifado)
);


CREATE TABLE exame (
    id_exame INTEGER PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    valor DECIMAL(10, 2),
    descricao TEXT
);


CREATE TABLE solicitacao_exame (
    id_solicitacao INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INTEGER NOT NULL,
    id_exame INTEGER NOT NULL,
    id_medico INTEGER NOT NULL,
    data_solicitacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status_exame VARCHAR(20) DEFAULT 'SOLICITADO' CHECK (status_exame IN ('SOLICITADO', 'EM ANDAMENTO', 'CONCLUIDO')),
    resultado TEXT,
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario),
    FOREIGN KEY (id_exame) REFERENCES exame(id_exame),
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico)
);


CREATE TABLE faturamento (
    id_faturamento INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_internacao INTEGER NOT NULL,
    valor_medicamentos DECIMAL(10, 2) DEFAULT 0.00,
    valor_exames DECIMAL(10, 2) DEFAULT 0.00,
    valor_internacao DECIMAL(10, 2) DEFAULT 0.00,
    valor_honorarios DECIMAL(10, 2) DEFAULT 0.00,
    valor_consumo DECIMAL(10, 2) DEFAULT 0.00,
    valor_total DECIMAL(10, 2),
    status_pagamento VARCHAR(10) DEFAULT 'PENDENTE' CHECK (status_pagamento IN ('PENDENTE', 'PAGO')),
    data_fechamento TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    observacao TEXT,
    FOREIGN KEY (id_internacao) REFERENCES internacao(id_internacao)
);


CREATE TABLE auditoria (
    id_auditoria INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_faturamento INTEGER,
    auditor VARCHAR(150),
    data_auditoria TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status_auditoria VARCHAR(10) DEFAULT 'PENDENTE' CHECK (status_auditoria IN ('PENDENTE', 'APROVADO', 'REPROVADO')),
    observacoes TEXT,
    conformidade INTEGER CHECK (conformidade IN (0, 1)),
    FOREIGN KEY (id_faturamento) REFERENCES faturamento(id_faturamento)
);


CREATE TABLE log_prontuario (
    id_log INTEGER PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INTEGER NOT NULL,
    responsavel VARCHAR(150),
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    descricao TEXT,
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario)
);