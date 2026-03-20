using System.ComponentModel.DataAnnotations;

namespace CPTM_Backend.DTOs;

// ============================================================
//  DTO principal – espelha exatamente as 51 colunas do CSV
//  (seções 1–7.2) + lista de fotos (seção 7.3)
// ============================================================
public class FormularioEfluenteDto
{
    // ── Seção 5: Identificação do E.M. ──────────────────────
    /// <summary>BD_01=1 | Chave Primária - Meio Ambiente | Auto</summary>
    public string ChavePrimariaMa { get; set; } = string.Empty;

    /// <summary>BD_01=2 | Elemento de Monitoramento – Número (000001–999999)</summary>
    [Required] [MaxLength(6)]
    public string NrElementoMonit { get; set; } = string.Empty;

    /// <summary>BD_01=3 | Elemento de Monitoramento – Nome</summary>
    [MaxLength(100)]
    public string? NmElementoMonit { get; set; }

    // ── Seção 1: Premissas Institucionais / Cabeçalho ───────
    /// <summary>BD_01=52 | Nome (PJ) da Contratada</summary>
    [Required] [MaxLength(100)]
    public string NmContratada { get; set; } = string.Empty;

    /// <summary>BD_01=53 | Nº do Contrato da Contratada (≤12 chars)</summary>
    [MaxLength(12)]
    public string? NrContrato { get; set; }

    /// <summary>BD_01=17 | Local do Escopo Contratual (Pseudônimo)</summary>
    [MaxLength(100)]
    public string? NmLocalEscopo { get; set; }

    /// <summary>BD_01=57 | Representante (PF) da Contratada e/ou Área Gestora (≤89 chars)</summary>
    [MaxLength(89)]
    public string? NmRepresentante { get; set; }

    /// <summary>BD_01=4 | Sigla da Área de Meio Ambiente | Domínio: GEA_TX_SIGLA_DEPTO_MEIO_AMBIENTE</summary>
    [MaxLength(20)]
    public string? SgAreaMeioAmbiente { get; set; }

    /// <summary>BD_01=54 | Nome da Área Gestora CPTM | Domínio: GEA_TX_NM_AREA_GESTORA_CPTM</summary>
    [MaxLength(200)]
    public string? NmAreaGestoraCptm { get; set; }

    /// <summary>BD_01=55 | Identificador da Área Gestora CPTM | Auto | Não Editável</summary>
    [MaxLength(30)]
    public string? CdIdentAreaGestora { get; set; }

    /// <summary>BD_01=56 | Sigla da Área Gestora CPTM | Auto | Não Editável</summary>
    [MaxLength(20)]
    public string? SgAreaGestoraCptm { get; set; }

    /// <summary>BD_01=58 | Nome (PJ) da Supervisora Ambiental (≤89 chars)</summary>
    [MaxLength(89)]
    public string? NmSupervisoraAmbiental { get; set; }

    // ── Seção 2: Identificação do Cadastrador e RT ──────────
    /// <summary>BD_01=48 | Autor(a) (PF) do Cadastramento</summary>
    [Required] [MaxLength(89)]
    public string NmAutorCadastramento { get; set; } = string.Empty;

    /// <summary>BD_01=49 | Responsável Técnico – RT</summary>
    [Required] [MaxLength(89)]
    public string NmResponsavelTecnico { get; set; } = string.Empty;

    /// <summary>BD_01=50 | Registro Profissional do RT</summary>
    [MaxLength(50)]
    public string? NrRegistroProfissional { get; set; }

    /// <summary>BD_01=51 | Documento de Responsabilidade Técnica do RT</summary>
    [MaxLength(100)]
    public string? DsDocRespTecnica { get; set; }

    // ── Seção 3: Identificação do Formulário ────────────────
    /// <summary>BD_01=22 | Natureza (do PGA) | Domínio: GEA_TX_NATUREZA_DO_PGA</summary>
    [MaxLength(50)]
    public string? DsNaturezaPga { get; set; }

    /// <summary>BD_01=18 | Tipo de Formulário | Auto | Não Editável</summary>
    [MaxLength(100)]
    public string? DsTipoFormulario { get; set; }

    /// <summary>BD_01=19 | Data de Emissão do Formulário</summary>
    [Required]
    public DateTime DtEmissaoFormulario { get; set; }

    /// <summary>BD_01=20 | Número do Formulário (000001–999999)</summary>
    [Required] [MaxLength(6)]
    public string NrFormulario { get; set; } = string.Empty;

    /// <summary>BD_01=21 | Autor(a) (PF) do Formulário</summary>
    [MaxLength(89)]
    public string? NmAutorFormulario { get; set; }

    /// <summary>BD_01=60 | Nome do arquivo FDC | Auto | Não Editável</summary>
    [MaxLength(200)]
    public string? NmArquivoFdc { get; set; }

    /// <summary>BD_01=61 | Código do arquivo FDC | Auto | Não Editável</summary>
    [MaxLength(50)]
    public string? CdArquivoFdc { get; set; }

    // ── Seção 4: Data e Hora do Cadastro do E.M. ────────────
    /// <summary>BD_01=45 | Data do Cadastramento</summary>
    [Required]
    public DateTime DtCadastramento { get; set; }

    /// <summary>BD_01=46 | Hora do Cadastramento (hh:mm)</summary>
    [MaxLength(5)]
    public string? HrCadastramento { get; set; }

    // ── Seção 6: Localização do E.M. ────────────────────────
    /// <summary>BD_01=7 | Nome de Município | Domínio: GEA_TX_MUNICIPIO</summary>
    [MaxLength(100)]
    public string? NmMunicipio { get; set; }

    /// <summary>BD_01=8 | Nome da Linha CPTM | Domínio: GEA_TX_LINHA_CPTM</summary>
    [MaxLength(50)]
    public string? NmLinhaCptm { get; set; }

    /// <summary>BD_01=12 | Nome da Estação CPTM | Domínio: GEA_TX_ESTACAO_CPTM</summary>
    [MaxLength(100)]
    public string? NmEstacaoCptm { get; set; }

    /// <summary>BD_01=9 | Número da Via da Linha CPTM | Domínio: GEA_TX_VIA_CPTM</summary>
    [MaxLength(30)]
    public string? NrViaLinhaCptm { get; set; }

    /// <summary>BD_01=10 | Trecho e Sentido da Linha CPTM | Domínio: GEA_TX_TRECHO_E_SENTIDO_CPTM</summary>
    [MaxLength(100)]
    public string? DsTrechoSentidoCptm { get; set; }

    /// <summary>BD_01=11 | Número do Quilômetro e Poste (ex: "51/02")</summary>
    [MaxLength(10)]
    public string? NrKmPoste { get; set; }

    /// <summary>BD_01=13 | Latitude – Datum WGS84 (ex: -23.123456)</summary>
    public decimal? NrLatitude { get; set; }

    /// <summary>BD_01=14 | Longitude – Datum WGS84 (ex: -46.123456)</summary>
    public decimal? NrLongitude { get; set; }

    // ── Seção 7.1: Regulamentação Ambiental ─────────────────
    /// <summary>BD_01=24 | Tipo de Atividade (Listada) | Domínio: EF_TX_TIPO_ATIVIDADE_LISTADA</summary>
    [MaxLength(100)]
    public string? DsTipoAtividadeList { get; set; }

    /// <summary>BD_01=25 | Tipo de Atividade (Não Listada)</summary>
    [MaxLength(100)]
    public string? DsTipoAtividadeNlist { get; set; }

    /// <summary>BD_01=26 | Tipo de DRA (Listado) | Domínio: EF_TX_TIPO_DRA_LISTADO</summary>
    [MaxLength(100)]
    public string? DsTipoDraList { get; set; }

    /// <summary>BD_01=27 | Tipo de DRA (Não Listado)</summary>
    [MaxLength(100)]
    public string? DsTipoDraNlist { get; set; }

    /// <summary>BD_01=28 | Código Identificador do DRA</summary>
    [MaxLength(50)]
    public string? CdIdentificadorDra { get; set; }

    /// <summary>BD_01=29 | Data de Validade do DRA</summary>
    public DateTime? DtValidadeDra { get; set; }

    // ── Seção 7.2: Detalhamento ──────────────────────────────
    /// <summary>BD_01=31 | Tipo de Atividade na CPTM | Domínio: EF_TX_TIPO_ATIVIDADE_CPTM</summary>
    [MaxLength(100)]
    public string? DsTipoAtividadeCptm { get; set; }

    /// <summary>BD_01=32 | Nome Edificação/Local da CPTM | Domínio: EF_TX_NM_LOCAL_ATIV</summary>
    [MaxLength(100)]
    public string? NmLocalEdificacao { get; set; }

    /// <summary>BD_01=33 | Nome Edificação/Local (Complemento)</summary>
    [MaxLength(100)]
    public string? DsLocalComplemento { get; set; }

    /// <summary>BD_01=34 | Origem do Efluente | Domínio: EF_TX_ORIGEM_EFLUENTE</summary>
    [MaxLength(50)]
    public string? DsOrigemEfluente { get; set; }

    /// <summary>BD_01=35 | Fonte Geradora do Efluente | Domínio: EF_TX_FONTE_GERADORA</summary>
    [MaxLength(100)]
    public string? DsFonteGeradora { get; set; }

    /// <summary>BD_01=36 | Quantidade (Litros) – decimal até 8 casas</summary>
    public decimal? NrQuantidadeLitros { get; set; }

    /// <summary>BD_01=37 | Tipo de Destinação do Efluente | Domínio: EF_TX_TIPO_DESTINACAO</summary>
    [MaxLength(100)]
    public string? DsTipoDestinacao { get; set; }

    /// <summary>BD_01=38 | Tipo de Veículo | Domínio: EF_TX_TIPO_VEICULO</summary>
    [MaxLength(50)]
    public string? DsTipoVeiculo { get; set; }

    /// <summary>BD_01=39 | Identificador/Placa do Veículo</summary>
    [MaxLength(20)]
    public string? CdPlacaVeiculo { get; set; }

    /// <summary>BD_01=40 | Código Identificador da Guia de Remessa</summary>
    [MaxLength(50)]
    public string? CdGuiaRemessa { get; set; }

    /// <summary>BD_01=41 | Distância da Via CPTM (Metros)</summary>
    public decimal? NrDistanciaViaM { get; set; }

    /// <summary>BD_01=44 | Observações Gerais: Cadastramento (≤255 chars)</summary>
    [MaxLength(255)]
    public string? DsObservacoesCadastro { get; set; }

    // ── Seção 7.3: Registro Fotográfico (BD_02=1) ───────────
    /// <summary>Fotografias 1–4 (BD_01 70–73). Imagem como Base64.</summary>
    public List<FotoDto>? Fotos { get; set; }
}

// ============================================================
//  DTO de Fotografia
// ============================================================
public class FotoDto
{
    /// <summary>PK gerada pelo banco (null ao criar).</summary>
    public long? IdFoto { get; set; }

    /// <summary>Número da fotografia (1–4).
    /// 1=BD_01:70 | 2=BD_01:71 | 3=BD_01:72 | 4=BD_01:73</summary>
    [Required] [Range(1, 4)]
    public int NrFoto { get; set; }

    /// <summary>Imagem codificada em Base64 (null ao retornar sem conteúdo).</summary>
    public string? FotoBase64 { get; set; }

    /// <summary>Orientação da foto. Padrão: "Paisagem/Horizontal".</summary>
    [MaxLength(30)]
    public string? DsOrientacao { get; set; }
}
