using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPTM_Backend.Models;

/// <summary>
/// Tabela: TB_FDC_EEA_EF_FOTO  (BD_02 = 1)
/// Armazena as fotografias do Formulário FDC-EEA.EF.
///   NR_FOTO=1 → BD_01=70 (Fotografia 1)
///   NR_FOTO=2 → BD_01=71 (Fotografia 2)
///   NR_FOTO=3 → BD_01=72 (Fotografia 3)
///   NR_FOTO=4 → BD_01=73 (Fotografia 4)
/// Padrão: imagem 3×4, orientação Paisagem/Horizontal.
/// </summary>
[Table("TB_FDC_EEA_EF_FOTO")]
public class FotoEfluente
{
    /// <summary>PK auto-incremental via trigger + SQ_FDC_EEA_EF_FOTO.</summary>
    [Key]
    [Column("ID_FOTO")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdFoto { get; set; }

    /// <summary>FK → TB_FDC_EEA_EF.CHAVE_PRIMARIA_MA</summary>
    [Required]
    [Column("CHAVE_PRIMARIA_MA")]
    [MaxLength(60)]
    public string ChavePrimariaMa { get; set; } = string.Empty;

    /// <summary>Número da fotografia (1–4).
    /// 1=BD_01:70 | 2=BD_01:71 | 3=BD_01:72 | 4=BD_01:73</summary>
    [Required]
    [Column("NR_FOTO")]
    [Range(1, 4)]
    public int NrFoto { get; set; }

    /// <summary>Imagem binária (BLOB). Tamanho 3×4, Paisagem/Horizontal.</summary>
    [Column("BL_FOTO")]
    public byte[]? BlFoto { get; set; }

    /// <summary>Orientação da fotografia. Padrão: "Paisagem/Horizontal".</summary>
    [Column("DS_ORIENTACAO")]
    [MaxLength(30)]
    public string DsOrientacao { get; set; } = "Paisagem/Horizontal";

    // ===================================================================
    // NAVEGAÇÃO
    // ===================================================================

    [ForeignKey(nameof(ChavePrimariaMa))]
    public FormularioEfluente? Formulario { get; set; }
}
