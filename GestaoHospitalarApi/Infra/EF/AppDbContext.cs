using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Infra.EF;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agendamento> Agendamentos { get; set; }

    public virtual DbSet<Almoxarifado> Almoxarifados { get; set; }

    public virtual DbSet<Auditorium> Auditoria { get; set; }

    public virtual DbSet<ConsumoItem> ConsumoItems { get; set; }

    public virtual DbSet<Convenio> Convenios { get; set; }

    public virtual DbSet<EscalaMedica> EscalaMedicas { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<Exame> Exames { get; set; }

    public virtual DbSet<Faturamento> Faturamentos { get; set; }

    public virtual DbSet<InteracaoMedicamentosa> InteracaoMedicamentosas { get; set; }

    public virtual DbSet<Internacao> Internacaos { get; set; }

    public virtual DbSet<Leito> Leitos { get; set; }

    public virtual DbSet<LogProntuario> LogProntuarios { get; set; }

    public virtual DbSet<Medicamento> Medicamentos { get; set; }

    public virtual DbSet<Medico> Medicos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<PacienteConvenio> PacienteConvenios { get; set; }

    public virtual DbSet<Pessoa> Pessoas { get; set; }

    public virtual DbSet<Prescricao> Prescricaos { get; set; }

    public virtual DbSet<Prontuario> Prontuarios { get; set; }

    public virtual DbSet<Sala> Salas { get; set; }

    public virtual DbSet<SolicitacaoExame> SolicitacaoExames { get; set; }

    public virtual DbSet<Triagem> Triagems { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.HasKey(e => e.IdAgendamento).HasName("PRIMARY");

            entity.ToTable("agendamento");

            entity.HasIndex(e => e.IdMedico, "id_medico");

            entity.HasIndex(e => e.IdPaciente, "id_paciente");

            entity.HasIndex(e => e.IdSala, "id_sala");

            entity.Property(e => e.IdAgendamento)
                .HasColumnType("int(11)")
                .HasColumnName("id_agendamento");
            entity.Property(e => e.DataHora)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_hora");
            entity.Property(e => e.IdMedico)
                .HasColumnType("int(11)")
                .HasColumnName("id_medico");
            entity.Property(e => e.IdPaciente)
                .HasColumnType("int(11)")
                .HasColumnName("id_paciente");
            entity.Property(e => e.IdSala)
                .HasColumnType("int(11)")
                .HasColumnName("id_sala");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'AGENDADO'")
                .HasColumnName("status");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Agendamentos)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("agendamento_ibfk_2");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Agendamentos)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("agendamento_ibfk_1");

            entity.HasOne(d => d.IdSalaNavigation).WithMany(p => p.Agendamentos)
                .HasForeignKey(d => d.IdSala)
                .HasConstraintName("agendamento_ibfk_3");
        });

        modelBuilder.Entity<Almoxarifado>(entity =>
        {
            entity.HasKey(e => e.IdAlmoxarifado).HasName("PRIMARY");

            entity.ToTable("almoxarifado");

            entity.Property(e => e.IdAlmoxarifado)
                .HasColumnType("int(11)")
                .HasColumnName("id_almoxarifado");
            entity.Property(e => e.Categoria)
                .HasMaxLength(20)
                .HasColumnName("categoria");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.EstoqueMinimo)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("estoque_minimo");
            entity.Property(e => e.Lote)
                .HasMaxLength(50)
                .HasColumnName("lote");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Quantidade)
                .HasColumnType("int(11)")
                .HasColumnName("quantidade");
            entity.Property(e => e.Unidade)
                .HasMaxLength(10)
                .HasColumnName("unidade");
            entity.Property(e => e.Validade).HasColumnName("validade");
            entity.Property(e => e.ValorUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("valor_unitario");
        });

        modelBuilder.Entity<Auditorium>(entity =>
        {
            entity.HasKey(e => e.IdAuditoria).HasName("PRIMARY");

            entity.ToTable("auditoria");

            entity.HasIndex(e => e.IdFaturamento, "id_faturamento");

            entity.Property(e => e.IdAuditoria)
                .HasColumnType("int(11)")
                .HasColumnName("id_auditoria");
            entity.Property(e => e.Auditor)
                .HasMaxLength(150)
                .HasColumnName("auditor");
            entity.Property(e => e.Conformidade)
                .HasColumnType("int(11)")
                .HasColumnName("conformidade");
            entity.Property(e => e.DataAuditoria)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_auditoria");
            entity.Property(e => e.IdFaturamento)
                .HasColumnType("int(11)")
                .HasColumnName("id_faturamento");
            entity.Property(e => e.Observacoes)
                .HasColumnType("text")
                .HasColumnName("observacoes");
            entity.Property(e => e.StatusAuditoria)
                .HasMaxLength(10)
                .HasDefaultValueSql("'PENDENTE'")
                .HasColumnName("status_auditoria");

            entity.HasOne(d => d.IdFaturamentoNavigation).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.IdFaturamento)
                .HasConstraintName("auditoria_ibfk_1");
        });

        modelBuilder.Entity<ConsumoItem>(entity =>
        {
            entity.HasKey(e => e.IdConsumo).HasName("PRIMARY");

            entity.ToTable("consumo_item");

            entity.HasIndex(e => e.IdAlmoxarifado, "id_almoxarifado");

            entity.HasIndex(e => e.IdInternacao, "id_internacao");

            entity.Property(e => e.IdConsumo)
                .HasColumnType("int(11)")
                .HasColumnName("id_consumo");
            entity.Property(e => e.DataConsumo)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_consumo");
            entity.Property(e => e.IdAlmoxarifado)
                .HasColumnType("int(11)")
                .HasColumnName("id_almoxarifado");
            entity.Property(e => e.IdInternacao)
                .HasColumnType("int(11)")
                .HasColumnName("id_internacao");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.Quantidade)
                .HasColumnType("int(11)")
                .HasColumnName("quantidade");

            entity.HasOne(d => d.IdAlmoxarifadoNavigation).WithMany(p => p.ConsumoItems)
                .HasForeignKey(d => d.IdAlmoxarifado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("consumo_item_ibfk_2");

            entity.HasOne(d => d.IdInternacaoNavigation).WithMany(p => p.ConsumoItems)
                .HasForeignKey(d => d.IdInternacao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("consumo_item_ibfk_1");
        });

        modelBuilder.Entity<Convenio>(entity =>
        {
            entity.HasKey(e => e.IdConvenio).HasName("PRIMARY");

            entity.ToTable("convenio");

            entity.HasIndex(e => e.NomeConvenio, "nome_convenio").IsUnique();

            entity.Property(e => e.IdConvenio)
                .HasColumnType("int(11)")
                .HasColumnName("id_convenio");
            entity.Property(e => e.Ativo)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("ativo");
            entity.Property(e => e.CobreCirurgia)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("cobre_cirurgia");
            entity.Property(e => e.CobreExames)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("cobre_exames");
            entity.Property(e => e.CobreInternacao)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("cobre_internacao");
            entity.Property(e => e.LimiteMedicamento)
                .HasPrecision(10, 2)
                .HasColumnName("limite_medicamento");
            entity.Property(e => e.NomeConvenio)
                .HasMaxLength(100)
                .HasColumnName("nome_convenio");
            entity.Property(e => e.PercentualCobertura)
                .HasPrecision(5, 2)
                .HasColumnName("percentual_cobertura");
            entity.Property(e => e.TipoLeito)
                .HasMaxLength(20)
                .HasColumnName("tipo_leito");
        });

        modelBuilder.Entity<EscalaMedica>(entity =>
        {
            entity.HasKey(e => e.IdEscala).HasName("PRIMARY");

            entity.ToTable("escala_medica");

            entity.HasIndex(e => e.IdMedico, "id_medico");

            entity.Property(e => e.IdEscala)
                .HasColumnType("int(11)")
                .HasColumnName("id_escala");
            entity.Property(e => e.DataEscala).HasColumnName("data_escala");
            entity.Property(e => e.HoraFim)
                .HasMaxLength(5)
                .HasColumnName("hora_fim");
            entity.Property(e => e.HoraInicio)
                .HasMaxLength(5)
                .HasColumnName("hora_inicio");
            entity.Property(e => e.IdMedico)
                .HasColumnType("int(11)")
                .HasColumnName("id_medico");
            entity.Property(e => e.IsPlantao)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("is_plantao");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.EscalaMedicas)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("escala_medica_ibfk_1");
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.IdEspecialidade).HasName("PRIMARY");

            entity.ToTable("especialidade");

            entity.Property(e => e.IdEspecialidade)
                .HasColumnType("int(11)")
                .HasColumnName("id_especialidade");
            entity.Property(e => e.DescricaoEspecialidade)
                .HasMaxLength(100)
                .HasColumnName("descricao_especialidade");
        });

        modelBuilder.Entity<Exame>(entity =>
        {
            entity.HasKey(e => e.IdExame).HasName("PRIMARY");

            entity.ToTable("exame");

            entity.Property(e => e.IdExame)
                .HasColumnType("int(11)")
                .HasColumnName("id_exame");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Valor)
                .HasPrecision(10, 2)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<Faturamento>(entity =>
        {
            entity.HasKey(e => e.IdFaturamento).HasName("PRIMARY");

            entity.ToTable("faturamento");

            entity.HasIndex(e => e.IdInternacao, "id_internacao");

            entity.Property(e => e.IdFaturamento)
                .HasColumnType("int(11)")
                .HasColumnName("id_faturamento");
            entity.Property(e => e.DataFechamento)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_fechamento");
            entity.Property(e => e.IdInternacao)
                .HasColumnType("int(11)")
                .HasColumnName("id_internacao");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");
            entity.Property(e => e.StatusPagamento)
                .HasMaxLength(10)
                .HasDefaultValueSql("'PENDENTE'")
                .HasColumnName("status_pagamento");
            entity.Property(e => e.ValorConsumo)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("valor_consumo");
            entity.Property(e => e.ValorExames)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("valor_exames");
            entity.Property(e => e.ValorHonorarios)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("valor_honorarios");
            entity.Property(e => e.ValorInternacao)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("valor_internacao");
            entity.Property(e => e.ValorMedicamentos)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("valor_medicamentos");
            entity.Property(e => e.ValorTotal)
                .HasPrecision(10, 2)
                .HasColumnName("valor_total");

            entity.HasOne(d => d.IdInternacaoNavigation).WithMany(p => p.Faturamentos)
                .HasForeignKey(d => d.IdInternacao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("faturamento_ibfk_1");
        });

        modelBuilder.Entity<InteracaoMedicamentosa>(entity =>
        {
            entity.HasKey(e => e.IdInteracao).HasName("PRIMARY");

            entity.ToTable("interacao_medicamentosa");

            entity.HasIndex(e => e.IdMedicamento1, "id_medicamento_1");

            entity.HasIndex(e => e.IdMedicamento2, "id_medicamento_2");

            entity.Property(e => e.IdInteracao)
                .HasColumnType("int(11)")
                .HasColumnName("id_interacao");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.Gravidade)
                .HasMaxLength(10)
                .HasColumnName("gravidade");
            entity.Property(e => e.IdMedicamento1)
                .HasColumnType("int(11)")
                .HasColumnName("id_medicamento_1");
            entity.Property(e => e.IdMedicamento2)
                .HasColumnType("int(11)")
                .HasColumnName("id_medicamento_2");

            entity.HasOne(d => d.IdMedicamento1Navigation).WithMany(p => p.InteracaoMedicamentosaIdMedicamento1Navigations)
                .HasForeignKey(d => d.IdMedicamento1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interacao_medicamentosa_ibfk_1");

            entity.HasOne(d => d.IdMedicamento2Navigation).WithMany(p => p.InteracaoMedicamentosaIdMedicamento2Navigations)
                .HasForeignKey(d => d.IdMedicamento2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interacao_medicamentosa_ibfk_2");
        });

        modelBuilder.Entity<Internacao>(entity =>
        {
            entity.HasKey(e => e.IdInternacao).HasName("PRIMARY");

            entity.ToTable("internacao");

            entity.HasIndex(e => e.IdLeito, "id_leito");

            entity.HasIndex(e => e.IdProntuario, "id_prontuario");

            entity.Property(e => e.IdInternacao)
                .HasColumnType("int(11)")
                .HasColumnName("id_internacao");
            entity.Property(e => e.DataAlta)
                .HasColumnType("timestamp")
                .HasColumnName("data_alta");
            entity.Property(e => e.DataEntrada)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_entrada");
            entity.Property(e => e.IdLeito)
                .HasColumnType("int(11)")
                .HasColumnName("id_leito");
            entity.Property(e => e.IdProntuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_prontuario");
            entity.Property(e => e.Isolamento)
                .HasMaxLength(3)
                .HasColumnName("isolamento");
            entity.Property(e => e.StatusInternacao)
                .HasMaxLength(15)
                .HasDefaultValueSql("'ATIVA'")
                .HasColumnName("status_internacao");

            entity.HasOne(d => d.IdLeitoNavigation).WithMany(p => p.Internacaos)
                .HasForeignKey(d => d.IdLeito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("internacao_ibfk_2");

            entity.HasOne(d => d.IdProntuarioNavigation).WithMany(p => p.Internacaos)
                .HasForeignKey(d => d.IdProntuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("internacao_ibfk_1");
        });

        modelBuilder.Entity<Leito>(entity =>
        {
            entity.HasKey(e => e.IdLeito).HasName("PRIMARY");

            entity.ToTable("leito");

            entity.Property(e => e.IdLeito)
                .HasColumnType("int(11)")
                .HasColumnName("id_leito");
            entity.Property(e => e.Ala)
                .HasMaxLength(50)
                .HasColumnName("ala");
            entity.Property(e => e.Andar)
                .HasMaxLength(10)
                .HasColumnName("andar");
            entity.Property(e => e.DataHigienizacao)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_higienizacao");
            entity.Property(e => e.Numero)
                .HasMaxLength(10)
                .HasColumnName("numero");
            entity.Property(e => e.Situacao)
                .HasMaxLength(20)
                .HasDefaultValueSql("'VAGO'")
                .HasColumnName("situacao");
        });

        modelBuilder.Entity<LogProntuario>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PRIMARY");

            entity.ToTable("log_prontuario");

            entity.HasIndex(e => e.IdProntuario, "id_prontuario");

            entity.Property(e => e.IdLog)
                .HasColumnType("int(11)")
                .HasColumnName("id_log");
            entity.Property(e => e.DataAlteracao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_alteracao");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.IdProntuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_prontuario");
            entity.Property(e => e.Responsavel)
                .HasMaxLength(150)
                .HasColumnName("responsavel");

            entity.HasOne(d => d.IdProntuarioNavigation).WithMany(p => p.LogProntuarios)
                .HasForeignKey(d => d.IdProntuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("log_prontuario_ibfk_1");
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.HasKey(e => e.IdMedicamento).HasName("PRIMARY");

            entity.ToTable("medicamento");

            entity.HasIndex(e => e.IdAlmoxarifado, "id_almoxarifado");

            entity.Property(e => e.IdMedicamento)
                .HasColumnType("int(11)")
                .HasColumnName("id_medicamento");
            entity.Property(e => e.Contraindicacoes)
                .HasColumnType("text")
                .HasColumnName("contraindicacoes");
            entity.Property(e => e.IdAlmoxarifado)
                .HasColumnType("int(11)")
                .HasColumnName("id_almoxarifado");
            entity.Property(e => e.PrincipioAtivo)
                .HasMaxLength(150)
                .HasColumnName("principio_ativo");

            entity.HasOne(d => d.IdAlmoxarifadoNavigation).WithMany(p => p.Medicamentos)
                .HasForeignKey(d => d.IdAlmoxarifado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("medicamento_ibfk_1");
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.IdMedico).HasName("PRIMARY");

            entity.ToTable("medico");

            entity.HasIndex(e => e.Crm, "crm").IsUnique();

            entity.HasIndex(e => e.IdEspecialidade, "id_especialidade");

            entity.HasIndex(e => e.IdUsuario, "id_usuario").IsUnique();

            entity.Property(e => e.IdMedico)
                .HasColumnType("int(11)")
                .HasColumnName("id_medico");
            entity.Property(e => e.Crm)
                .HasMaxLength(20)
                .HasColumnName("crm");
            entity.Property(e => e.Honorario)
                .HasPrecision(10, 2)
                .HasColumnName("honorario");
            entity.Property(e => e.IdEspecialidade)
                .HasColumnType("int(11)")
                .HasColumnName("id_especialidade");
            entity.Property(e => e.IdUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_usuario");

            entity.HasOne(d => d.IdEspecialidadeNavigation).WithMany(p => p.Medicos)
                .HasForeignKey(d => d.IdEspecialidade)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("medico_ibfk_1");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Medico)
                .HasForeignKey<Medico>(d => d.IdUsuario)
                .HasConstraintName("medico_ibfk_2");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.IdPaciente).HasName("PRIMARY");

            entity.ToTable("paciente");

            entity.HasIndex(e => e.IdPessoa, "id_pessoa").IsUnique();

            entity.Property(e => e.IdPaciente)
                .HasColumnType("int(11)")
                .HasColumnName("id_paciente");
            entity.Property(e => e.Alergias)
                .HasColumnType("text")
                .HasColumnName("alergias");
            entity.Property(e => e.Ativo)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("ativo");
            entity.Property(e => e.HistoricoClinico)
                .HasColumnType("text")
                .HasColumnName("historico_clinico");
            entity.Property(e => e.IdPessoa)
                .HasColumnType("int(11)")
                .HasColumnName("id_pessoa");
            entity.Property(e => e.NomeResponsavel)
                .HasMaxLength(150)
                .HasColumnName("nome_responsavel");
            entity.Property(e => e.TipoSanguineo)
                .HasMaxLength(5)
                .HasColumnName("tipo_sanguineo");

            entity.HasOne(d => d.IdPessoaNavigation).WithOne(p => p.Paciente)
                .HasForeignKey<Paciente>(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("paciente_ibfk_1");
        });

        modelBuilder.Entity<PacienteConvenio>(entity =>
        {
            entity.HasKey(e => e.IdPacienteConvenio).HasName("PRIMARY");

            entity.ToTable("paciente_convenio");

            entity.HasIndex(e => e.IdConvenio, "id_convenio");

            entity.HasIndex(e => e.IdPaciente, "id_paciente");

            entity.Property(e => e.IdPacienteConvenio)
                .HasColumnType("int(11)")
                .HasColumnName("id_paciente_convenio");
            entity.Property(e => e.Ativo)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("ativo");
            entity.Property(e => e.IdConvenio)
                .HasColumnType("int(11)")
                .HasColumnName("id_convenio");
            entity.Property(e => e.IdPaciente)
                .HasColumnType("int(11)")
                .HasColumnName("id_paciente");
            entity.Property(e => e.NumeroCarteira)
                .HasMaxLength(50)
                .HasColumnName("numero_carteira");
            entity.Property(e => e.Validade).HasColumnName("validade");

            entity.HasOne(d => d.IdConvenioNavigation).WithMany(p => p.PacienteConvenios)
                .HasForeignKey(d => d.IdConvenio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("paciente_convenio_ibfk_2");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.PacienteConvenios)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("paciente_convenio_ibfk_1");
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.IdPessoa).HasName("PRIMARY");

            entity.ToTable("pessoa");

            entity.HasIndex(e => e.Cpf, "cpf").IsUnique();

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.IdPessoa)
                .HasColumnType("int(11)")
                .HasColumnName("id_pessoa");
            entity.Property(e => e.Bairro)
                .HasMaxLength(100)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(100)
                .HasColumnName("cidade");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .HasColumnName("cpf");
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_criacao");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .HasColumnName("estado");
            entity.Property(e => e.Nascimento).HasColumnName("nascimento");
            entity.Property(e => e.Nome)
                .HasMaxLength(150)
                .HasColumnName("nome");
            entity.Property(e => e.NumeroCasa)
                .HasColumnType("int(11)")
                .HasColumnName("numero_casa");
            entity.Property(e => e.Rua)
                .HasMaxLength(150)
                .HasColumnName("rua");
            entity.Property(e => e.Sexo)
                .HasMaxLength(10)
                .HasColumnName("sexo");
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Prescricao>(entity =>
        {
            entity.HasKey(e => e.IdPrescricao).HasName("PRIMARY");

            entity.ToTable("prescricao");

            entity.HasIndex(e => e.IdMedicamento, "id_medicamento");

            entity.HasIndex(e => e.IdMedico, "id_medico");

            entity.HasIndex(e => e.IdProntuario, "id_prontuario");

            entity.Property(e => e.IdPrescricao)
                .HasColumnType("int(11)")
                .HasColumnName("id_prescricao");
            entity.Property(e => e.Aplicacao)
                .HasMaxLength(50)
                .HasColumnName("aplicacao");
            entity.Property(e => e.Dosagem)
                .HasMaxLength(50)
                .HasColumnName("dosagem");
            entity.Property(e => e.Horario)
                .HasMaxLength(50)
                .HasColumnName("horario");
            entity.Property(e => e.IdMedicamento)
                .HasColumnType("int(11)")
                .HasColumnName("id_medicamento");
            entity.Property(e => e.IdMedico)
                .HasColumnType("int(11)")
                .HasColumnName("id_medico");
            entity.Property(e => e.IdProntuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_prontuario");
            entity.Property(e => e.Observacao)
                .HasColumnType("text")
                .HasColumnName("observacao");

            entity.HasOne(d => d.IdMedicamentoNavigation).WithMany(p => p.Prescricaos)
                .HasForeignKey(d => d.IdMedicamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prescricao_ibfk_3");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Prescricaos)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prescricao_ibfk_2");

            entity.HasOne(d => d.IdProntuarioNavigation).WithMany(p => p.Prescricaos)
                .HasForeignKey(d => d.IdProntuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prescricao_ibfk_1");
        });

        modelBuilder.Entity<Prontuario>(entity =>
        {
            entity.HasKey(e => e.IdProntuario).HasName("PRIMARY");

            entity.ToTable("prontuario");

            entity.HasIndex(e => e.IdMedico, "id_medico");

            entity.HasIndex(e => e.IdPaciente, "id_paciente");

            entity.HasIndex(e => e.IdSala, "id_sala");

            entity.HasIndex(e => e.IdTriagem, "id_triagem");

            entity.Property(e => e.IdProntuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_prontuario");
            entity.Property(e => e.DataAbertura)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_abertura");
            entity.Property(e => e.Evolucao)
                .HasColumnType("text")
                .HasColumnName("evolucao");
            entity.Property(e => e.IdMedico)
                .HasColumnType("int(11)")
                .HasColumnName("id_medico");
            entity.Property(e => e.IdPaciente)
                .HasColumnType("int(11)")
                .HasColumnName("id_paciente");
            entity.Property(e => e.IdSala)
                .HasColumnType("int(11)")
                .HasColumnName("id_sala");
            entity.Property(e => e.IdTriagem)
                .HasColumnType("int(11)")
                .HasColumnName("id_triagem");
            entity.Property(e => e.Isolamento)
                .HasMaxLength(3)
                .HasColumnName("isolamento");
            entity.Property(e => e.RiscoEvasao)
                .HasMaxLength(3)
                .HasColumnName("risco_evasao");
            entity.Property(e => e.StatusProntuario)
                .HasMaxLength(10)
                .HasDefaultValueSql("'ATIVO'")
                .HasColumnName("status_prontuario");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Prontuarios)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prontuario_ibfk_3");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Prontuarios)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prontuario_ibfk_1");

            entity.HasOne(d => d.IdSalaNavigation).WithMany(p => p.Prontuarios)
                .HasForeignKey(d => d.IdSala)
                .HasConstraintName("prontuario_ibfk_4");

            entity.HasOne(d => d.IdTriagemNavigation).WithMany(p => p.Prontuarios)
                .HasForeignKey(d => d.IdTriagem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prontuario_ibfk_2");
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.HasKey(e => e.IdSala).HasName("PRIMARY");

            entity.ToTable("sala");

            entity.Property(e => e.IdSala)
                .HasColumnType("int(11)")
                .HasColumnName("id_sala");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<SolicitacaoExame>(entity =>
        {
            entity.HasKey(e => e.IdSolicitacao).HasName("PRIMARY");

            entity.ToTable("solicitacao_exame");

            entity.HasIndex(e => e.IdExame, "id_exame");

            entity.HasIndex(e => e.IdMedico, "id_medico");

            entity.HasIndex(e => e.IdProntuario, "id_prontuario");

            entity.Property(e => e.IdSolicitacao)
                .HasColumnType("int(11)")
                .HasColumnName("id_solicitacao");
            entity.Property(e => e.DataSolicitacao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("data_solicitacao");
            entity.Property(e => e.IdExame)
                .HasColumnType("int(11)")
                .HasColumnName("id_exame");
            entity.Property(e => e.IdMedico)
                .HasColumnType("int(11)")
                .HasColumnName("id_medico");
            entity.Property(e => e.IdProntuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_prontuario");
            entity.Property(e => e.Resultado)
                .HasColumnType("text")
                .HasColumnName("resultado");
            entity.Property(e => e.StatusExame)
                .HasMaxLength(20)
                .HasDefaultValueSql("'SOLICITADO'")
                .HasColumnName("status_exame");

            entity.HasOne(d => d.IdExameNavigation).WithMany(p => p.SolicitacaoExames)
                .HasForeignKey(d => d.IdExame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("solicitacao_exame_ibfk_2");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.SolicitacaoExames)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("solicitacao_exame_ibfk_3");

            entity.HasOne(d => d.IdProntuarioNavigation).WithMany(p => p.SolicitacaoExames)
                .HasForeignKey(d => d.IdProntuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("solicitacao_exame_ibfk_1");
        });

        modelBuilder.Entity<Triagem>(entity =>
        {
            entity.HasKey(e => e.IdTriagem).HasName("PRIMARY");

            entity.ToTable("triagem");

            entity.HasIndex(e => e.IdPaciente, "id_paciente");

            entity.Property(e => e.IdTriagem)
                .HasColumnType("int(11)")
                .HasColumnName("id_triagem");
            entity.Property(e => e.Alergias)
                .HasColumnType("text")
                .HasColumnName("alergias");
            entity.Property(e => e.EscalaDor)
                .HasColumnType("int(11)")
                .HasColumnName("escala_dor");
            entity.Property(e => e.FrequenciaCardiaca)
                .HasColumnType("int(11)")
                .HasColumnName("frequencia_cardiaca");
            entity.Property(e => e.IdPaciente)
                .HasColumnType("int(11)")
                .HasColumnName("id_paciente");
            entity.Property(e => e.Internacao)
                .HasMaxLength(3)
                .HasColumnName("internacao");
            entity.Property(e => e.Observacoes)
                .HasColumnType("text")
                .HasColumnName("observacoes");
            entity.Property(e => e.Pressao)
                .HasMaxLength(10)
                .HasColumnName("pressao");
            entity.Property(e => e.Queixa)
                .HasColumnType("text")
                .HasColumnName("queixa");
            entity.Property(e => e.ResponsavelTriagem)
                .HasMaxLength(150)
                .HasColumnName("responsavel_triagem");
            entity.Property(e => e.Risco)
                .HasMaxLength(10)
                .HasColumnName("risco");
            entity.Property(e => e.Saturacao)
                .HasColumnType("int(11)")
                .HasColumnName("saturacao");
            entity.Property(e => e.Temperatura)
                .HasPrecision(4, 1)
                .HasColumnName("temperatura");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Triagems)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("triagem_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.IdPessoa, "id_pessoa").IsUnique();

            entity.HasIndex(e => e.Login, "login").IsUnique();

            entity.Property(e => e.IdUsuario)
                .HasColumnType("int(11)")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Ativo)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("ativo");
            entity.Property(e => e.IdPessoa)
                .HasColumnType("int(11)")
                .HasColumnName("id_pessoa");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Perfil)
                .HasMaxLength(50)
                .HasColumnName("perfil");
            entity.Property(e => e.Senha)
                .HasMaxLength(255)
                .HasColumnName("senha");

            entity.HasOne(d => d.IdPessoaNavigation).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
