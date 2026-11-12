using GaroAvisa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Local> Locais { get; set; }
        public DbSet<Preferencia> Preferencias { get; set; }
        public DbSet<RegraAlerta> RegrasAlerta { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<PrevisaoSalva> PrevisoesSalvas { get; set; }
        public DbSet<Compromisso> Compromissos { get; set; }
        public DbSet<RegistroIdempotencia> RegistrosIdempotencia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("T_GS_USUARIO");
            modelBuilder.Entity<Usuario>().Property(u => u.Id).HasColumnName("ID");
            modelBuilder.Entity<Usuario>().Property(u => u.Nome).HasColumnName("NOME");
            modelBuilder.Entity<Usuario>().Property(u => u.Email).HasColumnName("EMAIL");
            modelBuilder.Entity<Usuario>().Property(u => u.SenhaHash).HasColumnName("SENHA_HASH");
            modelBuilder.Entity<Usuario>().Property(u => u.Tipo).HasColumnName("TIPO");
            modelBuilder.Entity<Usuario>().Property(u => u.CriadoEm).HasColumnName("DT_CRIACAO");

            modelBuilder.Entity<Local>().ToTable("T_GS_LOCAL");
            modelBuilder.Entity<Local>().Property(l => l.Id).HasColumnName("ID");
            modelBuilder.Entity<Local>().Property(l => l.UsuarioId).HasColumnName("ID_USUARIO");
            modelBuilder.Entity<Local>().Property(l => l.Etiqueta).HasColumnName("ETIQUETA");
            modelBuilder.Entity<Local>().Property(l => l.Nome).HasColumnName("NOME");
            modelBuilder.Entity<Local>().Property(l => l.Estado).HasColumnName("ESTADO");
            modelBuilder.Entity<Local>().Property(l => l.Pais).HasColumnName("PAIS");
            modelBuilder.Entity<Local>().Property(l => l.Latitude).HasColumnName("LATITUDE");
            modelBuilder.Entity<Local>().Property(l => l.Longitude).HasColumnName("LONGITUDE");
            modelBuilder.Entity<Local>().Property(l => l.Fuso).HasColumnName("FUSO");
            modelBuilder.Entity<Local>().Property(l => l.Principal).HasColumnName("PRINCIPAL");
            modelBuilder.Entity<Local>().Property(l => l.ReceberAlertas).HasColumnName("RECEBER_ALERTAS");
            modelBuilder.Entity<Local>().Property(l => l.CriadoEm).HasColumnName("DT_CRIACAO");
            modelBuilder.Entity<Preferencia>().ToTable("T_GS_PREFERENCIA");
            modelBuilder.Entity<Preferencia>().Property(p => p.Id).HasColumnName("ID");
            modelBuilder.Entity<Preferencia>().Property(p => p.UsuarioId).HasColumnName("ID_USUARIO");
            modelBuilder.Entity<Preferencia>().Property(p => p.ResumoDiarioAtivo).HasColumnName("RESUMO_DIARIO_ATIVO");
            modelBuilder.Entity<Preferencia>().Property(p => p.ResumoDiarioHorario).HasColumnName("RESUMO_DIARIO_HORARIO");
            modelBuilder.Entity<Preferencia>().Property(p => p.AlertasDoDia).HasColumnName("ALERTAS_DO_DIA");
            modelBuilder.Entity<Preferencia>().Property(p => p.PrevisaoMudou).HasColumnName("PREVISAO_MUDOU");
            modelBuilder.Entity<Preferencia>().Property(p => p.Compromissos).HasColumnName("COMPROMISSOS");
            modelBuilder.Entity<Preferencia>().Property(p => p.SilencioNoturno).HasColumnName("SILENCIO_NOTURNO");
            modelBuilder.Entity<Preferencia>().Property(p => p.AtualizadoEm).HasColumnName("DT_ATUALIZACAO");
            modelBuilder.Entity<RegraAlerta>().ToTable("T_GS_REGRA_ALERTA");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.Id).HasColumnName("ID");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.Tipo).HasColumnName("TIPO");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.Limite).HasColumnName("LIMITE");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.Gravidade).HasColumnName("GRAVIDADE");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.Titulo).HasColumnName("TITULO");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.Descricao).HasColumnName("DESCRICAO");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.Ativa).HasColumnName("ATIVA");
            modelBuilder.Entity<RegraAlerta>().Property(r => r.CriadoEm).HasColumnName("DT_CRIACAO");
            modelBuilder.Entity<Alerta>().ToTable("T_GS_ALERTA");
            modelBuilder.Entity<Alerta>().Property(a => a.Id).HasColumnName("ID");
            modelBuilder.Entity<Alerta>().Property(a => a.LocalId).HasColumnName("ID_LOCAL");
            modelBuilder.Entity<Alerta>().Property(a => a.DataPrevisao).HasColumnName("DT_PREVISAO");
            modelBuilder.Entity<Alerta>().Property(a => a.Tipo).HasColumnName("TIPO");
            modelBuilder.Entity<Alerta>().Property(a => a.Titulo).HasColumnName("TITULO");
            modelBuilder.Entity<Alerta>().Property(a => a.Descricao).HasColumnName("DESCRICAO");
            modelBuilder.Entity<Alerta>().Property(a => a.Gravidade).HasColumnName("GRAVIDADE");
            modelBuilder.Entity<Alerta>().Property(a => a.ValorPrevisto).HasColumnName("VALOR_PREVISTO");
            modelBuilder.Entity<Alerta>().Property(a => a.Lido).HasColumnName("LIDO");
            modelBuilder.Entity<Alerta>().Property(a => a.CriadoEm).HasColumnName("DT_CRIACAO");

            modelBuilder.Entity<PrevisaoSalva>().ToTable("T_GS_PREVISAO");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.Id).HasColumnName("ID");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.LocalId).HasColumnName("ID_LOCAL");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.DataPrevisao).HasColumnName("DT_PREVISAO");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.CodigoTempo).HasColumnName("CODIGO_TEMPO");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.TemperaturaMaxima).HasColumnName("TEMP_MAXIMA");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.TemperaturaMinima).HasColumnName("TEMP_MINIMA");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.ChuvaMm).HasColumnName("CHUVA_MM");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.ProbabilidadeChuva).HasColumnName("PROB_CHUVA");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.RajadaVentoKmh).HasColumnName("RAJADA_VENTO");
            modelBuilder.Entity<PrevisaoSalva>().Property(p => p.AtualizadoEm).HasColumnName("DT_ATUALIZACAO");

            modelBuilder.Entity<Compromisso>().ToTable("T_GS_COMPROMISSO");
            modelBuilder.Entity<Compromisso>().Property(c => c.Id).HasColumnName("ID");
            modelBuilder.Entity<Compromisso>().Property(c => c.LocalId).HasColumnName("ID_LOCAL");
            modelBuilder.Entity<Compromisso>().Property(c => c.Nome).HasColumnName("NOME");
            modelBuilder.Entity<Compromisso>().Property(c => c.DataHora).HasColumnName("DT_COMPROMISSO");
            modelBuilder.Entity<Compromisso>().Property(c => c.CriadoEm).HasColumnName("DT_CRIACAO");

            modelBuilder.Entity<RegistroIdempotencia>().ToTable("T_GS_IDEMPOTENCIA");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.Id).HasColumnName("ID");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.Chave).HasColumnName("CHAVE");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.UsuarioId).HasColumnName("ID_USUARIO");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.Rota).HasColumnName("ROTA");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.HashRequisicao).HasColumnName("HASH_REQUISICAO");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.StatusCode).HasColumnName("STATUS_CODE");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.RespostaJson).HasColumnName("RESPOSTA_JSON");
            modelBuilder.Entity<RegistroIdempotencia>().Property(r => r.CriadoEm).HasColumnName("DT_CRIACAO");

            base.OnModelCreating(modelBuilder);
        }
    }
}
