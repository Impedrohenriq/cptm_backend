using Microsoft.EntityFrameworkCore;
using CPTM_Backend.Models;

namespace CPTM_Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FormularioEfluente> Formularios { get; set; }
    public DbSet<FotoEfluente>       Fotos       { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── TB_FDC_EEA_EF ────────────────────────────────────────────────
        modelBuilder.Entity<FormularioEfluente>(entity =>
        {
            entity.ToTable("TB_FDC_EEA_EF");

            entity.HasKey(e => e.ChavePrimariaMa);

            // Precisão das colunas numéricas
            entity.Property(e => e.NrLatitude)
                  .HasColumnType("NUMBER(12,6)");

            entity.Property(e => e.NrLongitude)
                  .HasColumnType("NUMBER(12,6)");

            entity.Property(e => e.NrQuantidadeLitros)
                  .HasColumnType("NUMBER(16,8)");

            entity.Property(e => e.NrDistanciaViaM)
                  .HasColumnType("NUMBER(10,2)");

            // Valor padrão do tipo de formulário (BD_01=18)
            entity.Property(e => e.DsTipoFormulario)
                  .HasDefaultValue("Formulário de Cadastramento - FDC (FDC-EEA.EF)");

            // Relacionamento 1:N com fotos (BD_02=1)
            entity.HasMany(e => e.Fotos)
                  .WithOne(f => f.Formulario)
                  .HasForeignKey(f => f.ChavePrimariaMa)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── TB_FDC_EEA_EF_FOTO ───────────────────────────────────────────
        modelBuilder.Entity<FotoEfluente>(entity =>
        {
            entity.ToTable("TB_FDC_EEA_EF_FOTO");

            entity.HasKey(e => e.IdFoto);

            // ID_FOTO é gerado pelo trigger Oracle (SQ_FDC_EEA_EF_FOTO)
            entity.Property(e => e.IdFoto)
                  .ValueGeneratedOnAdd();

            // Unicidade: um formulário só pode ter uma foto por número (BD_01 70–73)
            entity.HasIndex(e => new { e.ChavePrimariaMa, e.NrFoto })
                  .IsUnique()
                  .HasDatabaseName("UQ_FOTO_POR_FORMULARIO");

            entity.Property(e => e.DsOrientacao)
                  .HasDefaultValue("Paisagem/Horizontal");
        });
    }
}
