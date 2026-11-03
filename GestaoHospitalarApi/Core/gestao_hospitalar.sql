CREATE DATABASE gestao_hospitalar;
USE gestao_hospitalar;

CREATE TABLE pessoa (
    id_pessoa INT not null PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(150) NOT NULL,
    cpf VARCHAR(11) UNIQUE NOT NULL CHECK (length(cpf) = 11),
    nascimento DATE,
    sexo VARCHAR(10) CHECK (sexo IN ('MASCULINO', 'FEMININO', 'OUTRO')),
    telefone VARCHAR(20),
    email VARCHAR(100) UNIQUE,
    rua VARCHAR(150),
    numero_casa VARCHAR(10),
    bairro VARCHAR(100),
    cidade VARCHAR(100),
    estado VARCHAR(2),
    cep VARCHAR(8) CHECK (length(cep) = 8),
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE usuario (
    id_usuario INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_pessoa INT NOT NULL UNIQUE,
    login VARCHAR(50) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    perfil VARCHAR(50) NOT NULL CHECK (perfil IN ('ADMIN', 'MEDICO', 'ENFERMEIRO', 'RECEPCAO', 'FINANCEIRO', 'PACIENTE')),
    ativo INT DEFAULT 1 CHECK (ativo IN (0, 1)),
    FOREIGN KEY (id_pessoa) REFERENCES pessoa(id_pessoa) ON DELETE CASCADE
);


CREATE TABLE convenio (
    id_convenio INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    ativo INT DEFAULT 1 CHECK (ativo IN (0, 1)),
    nome_convenio VARCHAR(100) UNIQUE NOT NULL,
    tipo_leito VARCHAR(20) CHECK (tipo_leito IN ('COMUM', 'PRIVADO', 'PREMIUM')),
    cobre_internacao INT DEFAULT 1 CHECK (cobre_internacao IN (0, 1)),
    cobre_exames INT DEFAULT 1 CHECK (cobre_exames IN (0, 1)),
    cobre_cirurgia INT DEFAULT 1 CHECK (cobre_cirurgia IN (0, 1)),
    limite_medicamento DECIMAL(10, 2),
    percentual_cobertura DECIMAL(5, 2)
);

CREATE TABLE especialidade (
    id_especialidade INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    descricao_especialidade VARCHAR(100) NOT NULL
);

CREATE TABLE medico (
    id_medico INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_usuario INT NOT NULL UNIQUE,
    id_especialidade INT NOT NULL,
    crm VARCHAR(20) UNIQUE NOT NULL,
    honorario DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_especialidade) REFERENCES especialidade(id_especialidade),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario) ON DELETE CASCADE
);


CREATE TABLE escala_medica (
    id_escala INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_medico INT NOT NULL,
    data_escala DATE NOT NULL,
    hora_inicio TIME NOT NULL, -- Formato 'HH:MM'
    hora_fim TIME NOT NULL,    -- Formato 'HH:MM'
    is_plantao INT DEFAULT 0 CHECK (is_plantao IN (0, 1)),
    
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico) ON DELETE CASCADE
);


CREATE TABLE paciente (
    id_paciente INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_pessoa INT NOT NULL UNIQUE,
    ativo INT DEFAULT 1 CHECK (ativo IN (0, 1)),
    alergias TEXT,
    tipo_sanguineo VARCHAR(5),
    historico_clinico TEXT,
    nome_responsavel VARCHAR(150),
    FOREIGN KEY (id_pessoa) REFERENCES pessoa(id_pessoa) ON DELETE CASCADE
);


CREATE TABLE paciente_convenio (
    id_paciente_convenio INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_paciente INT NOT NULL,
    id_convenio INT NOT NULL,
    numero_carteira VARCHAR(50),
    validade DATE NOT NULL,
    ativo INT DEFAULT 1 CHECK (ativo IN (0, 1)),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente) ON DELETE CASCADE, 
    FOREIGN KEY (id_convenio) REFERENCES convenio(id_convenio)
);

CREATE TABLE ala (
	id_ala INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    nome_ala VARCHAR(100) NOT NULL,
    andar VARCHAR(20) NOT NULL
);

CREATE TABLE sala (
    id_sala INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_ala INT NOT NULL,
    tipo VARCHAR(50) DEFAULT 'CONSULTORIO',
    nome VARCHAR(50) NOT NULL,
    status VARCHAR(20) CHECK (status IN ('LIVRE', 'OCUPADA', 'MANUTENCAO'))
);


CREATE TABLE agendamento (
    id_agendamento INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_paciente INT NOT NULL,
    id_medico INT NOT NULL,
    id_sala INT NOT NULL,
    data_hora TIMESTAMP NOT NULL,
    status VARCHAR(20) DEFAULT 'AGENDADO' CHECK (status IN ('AGENDADO', 'CONFIRMADO', 'CANCELADO', 'FINALIZADO')),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente),
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico),
    FOREIGN KEY (id_sala) REFERENCES sala(id_sala)
);


CREATE TABLE triagem (
    id_triagem INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_paciente INT NOT NULL,
    responsavel_triagem VARCHAR(150),
    pressao VARCHAR(10),
    temperatura DECIMAL(4, 1),
    frequencia_cardiaca INT,
    saturacao INT,
    escala_dor INT CHECK (escala_dor BETWEEN 0 AND 10),
    risco VARCHAR(10) CHECK (risco IN ('VERMELHO', 'LARANJA', 'AMARELO', 'VERDE', 'AZUL')),
    queixa TEXT,
    observacoes TEXT,
    internacao VARCHAR(3) CHECK (internacao IN ('SIM', 'NAO')),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente) ON DELETE CASCADE
);


CREATE TABLE prontuario (
    id_prontuario INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_paciente INT NOT NULL,
    id_triagem INT NOT NULL,
    id_medico INT NOT NULL,
    id_sala INT,
    risco_evasao VARCHAR(3) NOT NULL CHECK (risco_evasao IN ('SIM', 'NAO')),
    isolamento VARCHAR(3) NOT NULL CHECK (isolamento IN ('SIM', 'NAO')),
    evolucao TEXT,
    data_abertura TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status_prontuario VARCHAR(10) DEFAULT 'ATIVO' CHECK (status_prontuario IN ('ATIVO', 'ARQUIVADO')),
    FOREIGN KEY (id_paciente) REFERENCES paciente(id_paciente) ON DELETE CASCADE, 
    FOREIGN KEY (id_triagem) REFERENCES triagem(id_triagem) ON DELETE CASCADE,
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico),
    FOREIGN KEY (id_sala) REFERENCES sala(id_sala)
);


CREATE TABLE leito (
    id_leito INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_ala INT NOT NULL,
    numero VARCHAR(10) NOT NULL,
    tipo_leito VARCHAR(20) DEFAULT 'COMUM' CHECK (tipo_leito IN ('COMUM', 'PRIVADO', 'PREMIUM')),
    data_higienizacao TIMESTAMP,
    situacao VARCHAR(20) DEFAULT 'VAGO' CHECK (situacao IN ('VAGO', 'OCUPADO', 'HIGIENIZACAO'))
);


CREATE TABLE internacao (
    id_internacao INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INT NOT NULL,
    id_leito INT NOT NULL,
    data_entrada TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_alta TIMESTAMP NULL DEFAULT NULL,
    status_internacao VARCHAR(15) DEFAULT 'ATIVA' CHECK (status_internacao IN ('ATIVA', 'ALTA', 'TRANSFERIDO')),
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario) ON DELETE CASCADE,
    FOREIGN KEY (id_leito) REFERENCES leito(id_leito)
);


CREATE TABLE almoxarifado (
    id_almoxarifado INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    categoria VARCHAR(20) NOT NULL CHECK (categoria IN ('MEDICAMENTO', 'DESCARTAVEL', 'LIMPEZA', 'EPI', 'INSUMO')),
    descricao TEXT,
    quantidade INT NOT NULL DEFAULT 0,
    unidade VARCHAR(10),
    valor_unitario DECIMAL(10, 2) NOT NULL,
    estoque_minimo INT DEFAULT 0,
    lote VARCHAR(50),
    validade DATE
);


CREATE TABLE medicamento (
    id_medicamento INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_almoxarifado INT NOT NULL,
    principio_ativo VARCHAR(150),
    contraindicacoes TEXT,
    FOREIGN KEY (id_almoxarifado) REFERENCES almoxarifado(id_almoxarifado) ON DELETE CASCADE
);


CREATE TABLE interacao_medicamentosa (
    id_interacao INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_medicamento_1 INT NOT NULL,
    id_medicamento_2 INT NOT NULL,
    gravidade VARCHAR(10) CHECK (gravidade IN ('LEVE', 'MODERADA', 'GRAVE')),
    descricao TEXT,
    FOREIGN KEY (id_medicamento_1) REFERENCES medicamento(id_medicamento) ON DELETE CASCADE,
    FOREIGN KEY (id_medicamento_2) REFERENCES medicamento(id_medicamento) ON DELETE CASCADE
);


CREATE TABLE prescricao (
    id_prescricao INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INT NOT NULL,
    id_medico INT NOT NULL,
    id_medicamento INTEGER NOT NULL,
    dosagem VARCHAR(50),
    aplicacao VARCHAR(50),
    horario VARCHAR(50) NOT NULL,
    observacao TEXT,
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario) ON DELETE CASCADE,
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico),
    FOREIGN KEY (id_medicamento) REFERENCES medicamento(id_medicamento)
);


CREATE TABLE consumo_item (
    id_consumo INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_internacao INT NOT NULL,
    id_almoxarifado INT NOT NULL,
    quantidade INT NOT NULL,
    data_consumo TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    observacao TEXT,
    FOREIGN KEY (id_internacao) REFERENCES internacao(id_internacao) ON DELETE CASCADE,
    FOREIGN KEY (id_almoxarifado) REFERENCES almoxarifado(id_almoxarifado)
);


CREATE TABLE exame (
    id_exame INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    valor DECIMAL(10, 2),
    descricao TEXT
);

CREATE TABLE solicitacao_exame (
    id_solicitacao INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INT NOT NULL,
    id_exame INT NOT NULL,
    id_medico INT NOT NULL,
    data_solicitacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status_exame VARCHAR(20) DEFAULT 'SOLICITADO' CHECK (status_exame IN ('SOLICITADO', 'EM ANDAMENTO', 'CONCLUIDO')),
    resultado TEXT,
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario) ON DELETE CASCADE,
    FOREIGN KEY (id_exame) REFERENCES exame(id_exame),
    FOREIGN KEY (id_medico) REFERENCES medico(id_medico)
);

CREATE TABLE faturamento (
    id_faturamento INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_internacao INT NOT NULL,
    total_medicamentos DECIMAL(10, 2) DEFAULT 0.00,
    total_exames DECIMAL(10, 2) DEFAULT 0.00,
    total_internacao DECIMAL(10, 2) DEFAULT 0.00,
    total_honorarios DECIMAL(10, 2) DEFAULT 0.00,
    total_consumo DECIMAL(10, 2) DEFAULT 0.00,
    valor_total DECIMAL(10, 2),
    status_pagamento VARCHAR(10) DEFAULT 'PENDENTE' CHECK (status_pagamento IN ('PENDENTE', 'PAGO')),
    data_fechamento TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    observacao TEXT,
    FOREIGN KEY (id_internacao) REFERENCES internacao(id_internacao) ON DELETE CASCADE
);


CREATE TABLE auditoria (
    id_auditoria INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_faturamento INT NOT NULL,
    auditor VARCHAR(150),
    data_auditoria TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status_auditoria VARCHAR(10) DEFAULT 'PENDENTE' CHECK (status_auditoria IN ('PENDENTE', 'APROVADO', 'REPROVADO')),
    observacoes TEXT,
    conformidade INT CHECK (conformidade IN (0, 1)),
    FOREIGN KEY (id_faturamento) REFERENCES faturamento(id_faturamento) ON DELETE CASCADE
);

CREATE TABLE log_prontuario (
    id_log INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_prontuario INT NOT NULL,
    responsavel VARCHAR(150),
    data_alteracao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    descricao TEXT,
    FOREIGN KEY (id_prontuario) REFERENCES prontuario(id_prontuario) ON DELETE CASCADE
);

-- ============================================================================
-- 1. PESSOAS (CPF exatamente 11 dígitos, CEP exatamente 8 dígitos)
-- ============================================================================
INSERT INTO pessoa (id_pessoa, nome, cpf, nascimento, sexo, telefone, email, rua, numero_casa, bairro, cidade, estado, cep) VALUES
(1, 'Administrador do Sistema', '00000000000', '1985-01-15', 'MASCULINO', '(32) 99999-9999', 'admin@monge.com', 'Rua Central', '100', 'Centro', 'Muriaé', 'MG', '36880000'),
(2, 'Dr. Carlos Eduardo Silva', '11111111111', '1978-04-20', 'MASCULINO', '(32) 98888-1111', 'carlos.medico@hospital.com', 'Av. Juscelino Kubitschek', '500', 'Barra', 'Muriaé', 'MG', '36880000'),
(3, 'Dra. Ana Paula Mendonça', '22222222222', '1984-09-10', 'FEMININO', '(32) 98888-2222', 'ana.medica@hospital.com', 'Rua São Pedro', '250', 'Centro', 'Muriaé', 'MG', '36880000'),
(4, 'Enf. Maria Oliveira', '33333333333', '1990-03-05', 'FEMININO', '(32) 98888-3333', 'maria.enfermagem@hospital.com', 'Rua Silveira Brum', '42', 'Porto', 'Muriaé', 'MG', '36880000'),
(5, 'Paciente João da Silva', '44444444444', '1992-06-12', 'MASCULINO', '(32) 97777-4444', 'joao.silva@gmail.com', 'Rua Paschoal Bernardino', '120', 'Centro', 'Muriaé', 'MG', '36880000'),
(6, 'Paciente Mariana Souza', '55555555555', '1998-11-25', 'FEMININO', '(32) 97777-5555', 'mariana.souza@gmail.com', 'Av. Altino Rodrigues', '305', 'Dornelas', 'Muriaé', 'MG', '36880000'),
(7, 'Paciente Pedro Henrique', '66666666666', '1965-02-18', 'MASCULINO', '(32) 97777-6666', 'pedro.henrique@gmail.com', 'Rua Benedito Valadares', '88', 'Barra', 'Muriaé', 'MG', '36880000');

-- ============================================================================
-- 2. USUÁRIOS
-- ============================================================================
INSERT INTO usuario (id_usuario, id_pessoa, login, senha, perfil, ativo) VALUES
(1, 1, 'admin', '123456', 'ADMIN', 1),
(2, 2, 'CRM123', '123456', 'MEDICO', 1),
(3, 3, 'CRM456', '123456', 'MEDICO', 1),
(4, 4, 'enfmaria', '123456', 'ENFERMEIRO', 1),
(5, 5, 'pacientejoao', '123456', 'PACIENTE', 1),
(6, 6, 'pacientemariana', '123456', 'PACIENTE', 1),
(7, 7, 'pacientepedro', '123456', 'PACIENTE', 1);

-- ============================================================================
-- 3. ESPECIALIDADES
-- ============================================================================
INSERT INTO especialidade (id_especialidade, descricao_especialidade) VALUES
(1, 'Cardiologia'),
(2, 'Pediatria'),
(3, 'Ortopedia'),
(4, 'Neurologia'),
(5, 'Obstetrícia');

-- ============================================================================
-- 4. MÉDICOS (Vinculados a usuário, especialidade e com honorários definidos)
-- ============================================================================
INSERT INTO medico (id_medico, id_usuario, id_especialidade, crm, honorario) VALUES
(1, 2, 1, 'CRM123', 350.00), -- Dr. Carlos (Cardiologia)
(2, 3, 2, 'CRM456', 300.00); -- Dra. Ana (Pediatria)

-- ============================================================================
-- 6. CONVÊNIOS
-- ============================================================================
INSERT INTO convenio (id_convenio, nome_convenio, tipo_leito, cobre_internacao, cobre_exames, cobre_cirurgia, limite_medicamento, percentual_cobertura, ativo) VALUES
(1, 'Unimed', 'PREMIUM', 1, 1, 1, 5000.00, 100.00, 1),
(2, 'Bradesco Saúde', 'PRIVADO', 1, 1, 1, 3500.00, 80.00, 1),
(3, 'Amil', 'COMUM', 1, 1, 0, 2000.00, 70.00, 1),
(4, 'SulAmérica', 'PREMIUM', 1, 1, 1, 10000.00, 100.00, 1),
(5, 'SUS - Sistema Único de Saúde', 'COMUM', 1, 1, 1, 0.00, 100.00, 1);

-- ============================================================================
-- 7. PACIENTES E PACIENTE_CONVENIO
-- ============================================================================
INSERT INTO paciente (id_paciente, id_pessoa, ativo, alergias, tipo_sanguineo, historico_clinico, nome_responsavel) VALUES
(1, 5, 1, 'Penicilina, Dipirona', 'O+', 'Hipertensão arterial em acompanhamento.', 'Ana da Silva'),
(2, 6, 1, 'Nenhuma conhecida', 'A+', 'Sem comorbidades prévias.', 'Roberto Souza'),
(3, 7, 1, 'Aspirina', 'AB-', 'Diabetes Tipo 2 e Cardiopatia.', 'Lúcia Henrique');

INSERT INTO paciente_convenio (id_paciente_convenio, id_paciente, id_convenio, numero_carteira, validade, ativo) VALUES
(1, 1, 1, 'UNI-99887766-01', '2027-12-31', 1), -- João -> Unimed
(2, 2, 2, 'BRA-11223344-02', '2028-05-15', 1), -- Mariana -> Bradesco
(3, 3, 5, 'SUS-70001234567', '2030-01-01', 1);  -- Pedro -> SUS

-- ============================================================================
-- 8. ALAS, SALAS E LEITOS
-- ============================================================================
INSERT INTO ala (id_ala, nome_ala, andar) VALUES
(1, 'Pronto Socorro', 'Térreo'),
(2, 'UTI Adulto', '1º Andar'),
(3, 'Internação Comum', '1º Andar'),
(4, 'Cardiologia', '2º Andar'),
(5, 'Centro Cirúrgico', '2º Andar'),
(6, 'Ortopedia', '2º Andar'),
(7, 'Maternidade', '3º Andar'),
(8, 'Pediatria', '3º Andar'),
(9, 'UTI Neonatal', '3º Andar'),
(10, 'Internação Premium', '4º Andar');

INSERT INTO sala (id_sala, id_ala, tipo, nome, status) VALUES
(1, 1, 'CONSULTORIO', 'Consultório 01', 'LIVRE'),
(2, 1, 'CONSULTORIO', 'Consultório 02', 'LIVRE'),
(3, 1, 'CONSULTORIO', 'Consultório 03', 'LIVRE'),
(4, 1, 'TRIAGEM', 'Sala de Triagem', 'LIVRE'),
(5, 1, 'SUTURA', 'Sala de Sutura', 'LIVRE'),
(6, 5, 'CIRURGICA', 'Bloco Cirúrgico A', 'LIVRE');

INSERT INTO leito (id_leito, id_ala, numero, tipo_leito, situacao) VALUES
-- Ala 1: Pronto Socorro (5 leitos)
(6, 1, '01', 'COMUM', 'VAGO'),
(7, 1, '02', 'COMUM', 'VAGO'),
(8, 1, '03', 'COMUM', 'VAGO'),
(9, 1, '04', 'COMUM', 'VAGO'),
(10, 1, '05', 'COMUM', 'VAGO'),

-- Ala 2: UTI Adulto (5 leitos - 2 existentes + 3 novos)
(1, 2, '101', 'PREMIUM', 'VAGO'),
(2, 2, '102', 'PRIVADO', 'VAGO'),
(11, 2, '103', 'PREMIUM', 'VAGO'),
(12, 2, '104', 'PREMIUM', 'VAGO'),
(13, 2, '105', 'PRIVADO', 'VAGO'),

-- Ala 3: Internação Comum (5 leitos - 2 existentes + 3 novos)
(3, 3, '201', 'COMUM', 'VAGO'),
(4, 3, '202', 'COMUM', 'VAGO'),
(14, 3, '203', 'COMUM', 'VAGO'),
(15, 3, '204', 'COMUM', 'VAGO'),
(16, 3, '205', 'COMUM', 'VAGO'),

-- Ala 4: Cardiologia (5 leitos)
(17, 4, '401', 'PRIVADO', 'VAGO'),
(18, 4, '402', 'PRIVADO', 'VAGO'),
(19, 4, '403', 'PREMIUM', 'VAGO'),
(20, 4, '404', 'PRIVADO', 'VAGO'),
(21, 4, '405', 'PREMIUM', 'VAGO'),

-- Ala 5: Centro Cirúrgico (5 leitos)
(22, 5, '501', 'PRIVADO', 'VAGO'),
(23, 5, '502', 'PRIVADO', 'VAGO'),
(24, 5, '503', 'PRIVADO', 'VAGO'),
(25, 5, '504', 'PRIVADO', 'VAGO'),
(26, 5, '505', 'PRIVADO', 'VAGO'),

-- Ala 6: Ortopedia (5 leitos)
(27, 6, '601', 'COMUM', 'VAGO'),
(28, 6, '602', 'COMUM', 'VAGO'),
(29, 6, '603', 'COMUM', 'VAGO'),
(30, 6, '604', 'PRIVADO', 'VAGO'),
(31, 6, '605', 'PRIVADO', 'VAGO'),

-- Ala 7: Maternidade (5 leitos)
(32, 7, '701', 'PRIVADO', 'VAGO'),
(33, 7, '702', 'PRIVADO', 'VAGO'),
(34, 7, '703', 'PREMIUM', 'VAGO'),
(35, 7, '704', 'PREMIUM', 'VAGO'),
(36, 7, '705', 'PRIVADO', 'VAGO'),

-- Ala 8: Pediatria (5 leitos)
(37, 8, '801', 'COMUM', 'VAGO'),
(38, 8, '802', 'COMUM', 'VAGO'),
(39, 8, '803', 'COMUM', 'VAGO'),
(40, 8, '804', 'PRIVADO', 'VAGO'),
(41, 8, '805', 'PRIVADO', 'VAGO'),

-- Ala 9: UTI Neonatal (5 leitos)
(42, 9, '901', 'PREMIUM', 'VAGO'),
(43, 9, '902', 'PREMIUM', 'VAGO'),
(44, 9, '903', 'PREMIUM', 'VAGO'),
(45, 9, '904', 'PREMIUM', 'VAGO'),
(46, 9, '905', 'PREMIUM', 'VAGO'),

-- Ala 10: Internação Premium (5 leitos - 1 existente + 4 novos)
(5, 10, '301', 'PREMIUM', 'VAGO'),
(47, 10, '302', 'PREMIUM', 'VAGO'),
(48, 10, '303', 'PREMIUM', 'VAGO'),
(49, 10, '304', 'PREMIUM', 'VAGO'),
(50, 10, '305', 'PREMIUM', 'VAGO');


-- ============================================================================
-- 9. EXAMES
-- ============================================================================
INSERT INTO exame (id_exame, nome, valor, descricao) VALUES
(1, 'Hemograma Completo', 35.00, 'Avaliação de células sanguíneas'),
(2, 'Raio-X de Tórax', 85.00, 'Radiografia da região torácica'),
(3, 'Tomografia Computadorizada de Crânio', 350.00, 'Exame de imagem detalhado'),
(4, 'Eletrocardiograma (ECG)', 60.00, 'Avaliação da atividade elétrica do coração'),
(5, 'Glicemia em Jejum', 20.00, 'Medição da taxa de glicose no sangue');

-- ============================================================================
-- 10. ALMOXARIFADO, MEDICAMENTOS E INTERAÇÕES
-- ============================================================================
INSERT INTO almoxarifado (id_almoxarifado, nome, categoria, descricao, quantidade, unidade, valor_unitario, estoque_minimo, lote, validade) VALUES
(1, 'Dipirona Sódica 500mg/ml', 'MEDICAMENTO', 'Analgésico e antitérmico injetável', 500, 'AMPOLA', 2.50, 50, 'LOTE2026A', '2027-12-31'),
(2, 'Paracetamol 750mg', 'MEDICAMENTO', 'Analgésico em comprimidos', 1000, 'COMPRIMIDO', 0.80, 100, 'LOTE2026B', '2028-06-30'),
(3, 'Warfarina 5mg', 'MEDICAMENTO', 'Anticoagulante oral', 400, 'COMPRIMIDO', 3.20, 40, 'LOTE2026F', '2027-10-20'),
(4, 'Soro Fisiológico 0,9% 500ml', 'INSUMO', 'Solução salina estéril', 300, 'FRASCO', 6.00, 40, 'LOTE2026C', '2027-08-15'),
(5, 'Seringa Descartável 5ml', 'DESCARTAVEL', 'Seringa estéril com agulha', 2000, 'UNIDADE', 0.50, 200, 'LOTE2026D', '2029-01-01'),
(6, 'Luva de Procedimento M', 'EPI', 'Luva de látex estéril', 1500, 'PAR', 0.30, 300, 'LOTE2026E', '2028-10-10');

INSERT INTO medicamento (id_medicamento, id_almoxarifado, principio_ativo, contraindicacoes) VALUES
(1, 1, 'Dipirona Sódica', 'Alergia a pirazolonas, gravidez no primeiro trimestre'),
(2, 2, 'Paracetamol', 'Insuficiência hepática grave, hipersensibilidade'),
(3, 3, 'Warfarina Sódica', 'Risco elevado de hemorragia, gravidez');

INSERT INTO interacao_medicamentosa (id_interacao, id_medicamento_1, id_medicamento_2, gravidade, descricao) VALUES
(1, 1, 3, 'MODERADA', 'Potencialização do efeito anticoagulante com uso prolongado de Dipirona.'),
(2, 2, 3, 'LEVE', 'Pode aumentar o efeito anticoagulante da Warfarina em doses elevadas e contínuas de Paracetamol.');
