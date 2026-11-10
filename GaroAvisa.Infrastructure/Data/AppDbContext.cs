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
            modelBuilder.Entity<Local>().Property(l => l.Apelido).HasColumnName("APELIDO");
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
