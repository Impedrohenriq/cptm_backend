using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPTM_Backend.Models;

/// <summary>
/// Tabela: TB_FDC_EEA_EF
/// Formulário de Cadastramento/Caracterização – FDC (F_01)
/// Programa Ambiental: Efluentes e Emissões Atmosféricas – EEA | Natureza: Efluentes – EF
/// </summary>
[Table("TB_FDC_EEA_EF")]
public class FormularioEfluente
{
    // ===================================================================
    // SEÇÃO 5 – Identificação do E.M.
    // ===================================================================

    /// <summary>BD_01=1 | Chave Primária - Meio Ambiente | Auto | Não Editável
    /// Formato: EEA.EF-A.&lt;ANO&gt;-L.&lt;LINHA&gt;-&lt;SIGLA&gt;-N.&lt;NR&gt;
    /// Ex: "EEA.EF-A.2026-L.07-CPTM-N.000001"</summary>
    [Key]
    [Column("CHAVE_PRIMARIA_MA")]
    [MaxLength(60)]
    public string ChavePrimariaMa { get; set; } = string.Empty;

    /// <summary>BD_01=2 | Elemento de Monitoramento – Número | Editável
    /// Padrão: 6 dígitos. Ex: "000001"</summary>
    [Required]
    [Column("NR_ELEMENTO_MONIT")]
    [MaxLength(6)]
    public string NrElementoMonit { get; set; } = string.Empty;

    /// <summary>BD_01=3 | Elemento de Monitoramento – Nome | Editável
    /// Ex: "Caçamba A", "Plataforma 1 da Estação Brás"</summary>
    [Column("NM_ELEMENTO_MONIT")]
    [MaxLength(100)]
    public string? NmElementoMonit { get; set; }

    // ===================================================================
    // SEÇÃO 1 – Premissas Institucionais / Cabeçalho
    // ===================================================================

    /// <summary>BD_01=52 | Nome (PJ) da Contratada | Editável | Cache
    /// Formato: "Nome - SIGLA" (sigla ≤10 chars, maiúscula, sem espaço)
    /// Ex: "Companhia Paulista de Trens Metropolitanos - CPTM"</summary>
    [Required]
    [Column("NM_CONTRATADA")]
    [MaxLength(100)]
    public string NmContratada { get; set; } = string.Empty;

    /// <summary>BD_01=53 | Nº do Contrato da Contratada | Editável | Cache
    /// Padrão: até 12 chars sem espaços. Ex: "AR01234-56"</summary>
    [Column("NR_CONTRATO")]
    [MaxLength(12)]
    public string? NrContrato { get; set; }

    /// <summary>BD_01=17 | Local do Escopo Contratual (Pseudônimo) | Editável | Cache
    /// Ex: "Túnel da Estação da Luz", "Pátio Capuava"</summary>
    [Column("NM_LOCAL_ESCOPO")]
    [MaxLength(100)]
    public string? NmLocalEscopo { get; set; }

    /// <summary>BD_01=57 | Representante (PF) da Contratada e/ou Área Gestora | Editável | Cache
    /// Máx: 89 chars. Ex: "Representante da Contratada / Representante da CPTM"</summary>
    [Column("NM_REPRESENTANTE")]
    [MaxLength(89)]
    public string? NmRepresentante { get; set; }

    /// <summary>BD_01=4 | Sigla da Área de Meio Ambiente | Editável | Cache
    /// Domínio: GEA_TX_SIGLA_DEPTO_MEIO_AMBIENTE. Ex: "GEA.DEAE", "GEA.DEAO"</summary>
    [Column("SG_AREA_MEIO_AMBIENTE")]
    [MaxLength(20)]
    public string? SgAreaMeioAmbiente { get; set; }

    /// <summary>BD_01=54 | Nome da Área Gestora CPTM | Editável | Cache
    /// Domínio: GEA_TX_NM_AREA_GESTORA_CPTM</summary>
    [Column("NM_AREA_GESTORA_CPTM")]
    [MaxLength(200)]
    public string? NmAreaGestoraCptm { get; set; }

    /// <summary>BD_01=55 | Identificador da Área Gestora CPTM | Auto | Não Editável | Não Visível
    /// Formato: ID.&lt;X&gt;-&lt;X&gt;-&lt;X&gt;-&lt;X&gt;-&lt;X&gt;. Ex: "ID.10-15-5-3-0000"</summary>
    [Column("CD_IDENT_AREA_GESTORA")]
    [MaxLength(30)]
    public string? CdIdentAreaGestora { get; set; }

    /// <summary>BD_01=56 | Sigla da Área Gestora CPTM | Auto | Não Editável | Não Visível
    /// Formato: DO.XXX.XXXX.0000. Ex: "DO.GOF.DOFS.0000"</summary>
    [Column("SG_AREA_GESTORA_CPTM")]
    [MaxLength(20)]
    public string? SgAreaGestoraCptm { get; set; }

    /// <summary>BD_01=58 | Nome (PJ) da Supervisora Ambiental | Editável | Cache
    /// Máx: 89 chars. Ex: "Empresa Supervisora Ambiental Ltda - ESA"</summary>
    [Column("NM_SUPERVISORA_AMBIENTAL")]
    [MaxLength(89)]
    public string? NmSupervisoraAmbiental { get; set; }

    // ===================================================================
    // SEÇÃO 2 – Identificação do Cadastrador e Responsável Técnico
    // ===================================================================

    /// <summary>BD_01=48 | Autor(a) (PF) do Cadastramento | Editável | Cache</summary>
    [Required]
    [Column("NM_AUTOR_CADASTRAMENTO")]
    [MaxLength(89)]
    public string NmAutorCadastramento { get; set; } = string.Empty;

    /// <summary>BD_01=49 | Responsável Técnico – RT pelo Cadastramento | Editável | Cache</summary>
    [Required]
    [Column("NM_RESPONSAVEL_TECNICO")]
    [MaxLength(89)]
    public string NmResponsavelTecnico { get; set; } = string.Empty;

    /// <summary>BD_01=50 | Registro Profissional do RT | Editável | Cache
    /// Ex: "CREA - 123456 - Pessoa 5"</summary>
    [Column("NR_REGISTRO_PROFISSIONAL")]
    [MaxLength(50)]
    public string? NrRegistroProfissional { get; set; }

    /// <summary>BD_01=51 | Documento de Responsabilidade Técnica do RT | Editável | Cache
    /// Ex: "ART nº 123456 - Pessoa 5"</summary>
    [Column("DS_DOC_RESP_TECNICA")]
    [MaxLength(100)]
    public string? DsDocRespTecnica { get; set; }

    // ===================================================================
    // SEÇÃO 3 – Identificação do Formulário
    // ===================================================================

    /// <summary>BD_01=22 | Natureza (do PGA) | Editável | Cache
    /// Domínio: GEA_TX_NATUREZA_DO_PGA. Ex: "Efluente", "Emissões Atmosféricas"</summary>
    [Column("DS_NATUREZA_PGA")]
    [MaxLength(50)]
    public string? DsNaturezaPga { get; set; }

    /// <summary>BD_01=18 | Tipo de Formulário | Auto | Não Editável
    /// Valor fixo: "Formulário de Cadastramento - FDC (FDC-EEA.EF)"</summary>
    [Column("DS_TIPO_FORMULARIO")]
    [MaxLength(100)]
    public string DsTipoFormulario { get; set; } = "Formulário de Cadastramento - FDC (FDC-EEA.EF)";

    /// <summary>BD_01=19 | Data de Emissão do Formulário | Editável | Cache</summary>
    [Required]
    [Column("DT_EMISSAO_FORMULARIO")]
    public DateTime DtEmissaoFormulario { get; set; }

    /// <summary>BD_01=20 | Número do Formulário | Editável | Cache
    /// Padrão: 6 dígitos sequenciais, único. Ex: "000001"</summary>
    [Required]
    [Column("NR_FORMULARIO")]
    [MaxLength(6)]
    public string NrFormulario { get; set; } = string.Empty;

    /// <summary>BD_01=21 | Autor(a) (PF) do Formulário | Editável | Cache</summary>
    [Column("NM_AUTOR_FORMULARIO")]
    [MaxLength(89)]
    public string? NmAutorFormulario { get; set; }

    /// <summary>BD_01=60 | Nome do arquivo FDC relacionado | Auto | Não Editável
    /// Ex: "DeaoCtAr01823-02FdcEeaEfL10ProgaiaN000001"</summary>
    [Column("NM_ARQUIVO_FDC")]
    [MaxLength(200)]
    public string? NmArquivoFdc { get; set; }

    /// <summary>BD_01=61 | Código do arquivo FDC relacionado | Auto | Não Editável
    /// Formato: FDC-EEA.EF-A.&lt;ANO&gt;-L.&lt;LINHA&gt;-&lt;SIGLA&gt;-N.&lt;NR&gt;
    /// Ex: "FDC-EEA.EF-A.2026-L.07-CPTM-N.000001"</summary>
    [Column("CD_ARQUIVO_FDC")]
    [MaxLength(50)]
    public string? CdArquivoFdc { get; set; }

    // ===================================================================
    // SEÇÃO 4 – Data e Hora do Cadastro do E.M.
    // ===================================================================

    /// <summary>BD_01=45 | Data do Cadastramento | Editável | Cache</summary>
    [Required]
    [Column("DT_CADASTRAMENTO")]
    public DateTime DtCadastramento { get; set; }

    /// <summary>BD_01=46 | Hora do Cadastramento | Editável | Cache
    /// Padrão: hh:mm. Ex: "09:00"</summary>
    [Column("HR_CADASTRAMENTO")]
    [MaxLength(5)]
    public string? HrCadastramento { get; set; }

    // ===================================================================
    // SEÇÃO 6 – Localização do E.M.
    // ===================================================================

    /// <summary>BD_01=7 | Nome de Município | Editável | Cache
    /// Domínio: GEA_TX_MUNICIPIO</summary>
    [Column("NM_MUNICIPIO")]
    [MaxLength(100)]
    public string? NmMunicipio { get; set; }

    /// <summary>BD_01=8 | Nome da Linha CPTM | Editável | Cache
    /// Domínio: GEA_TX_LINHA_CPTM. Ex: "Linha 07 - Rubi"</summary>
    [Column("NM_LINHA_CPTM")]
    [MaxLength(50)]
    public string? NmLinhaCptm { get; set; }

    /// <summary>BD_01=12 | Nome da Estação CPTM | Editável | Cache
    /// Domínio: GEA_TX_ESTACAO_CPTM. Ex: "Estação Jardim Helena - Vila Mara"</summary>
    [Column("NM_ESTACAO_CPTM")]
    [MaxLength(100)]
    public string? NmEstacaoCptm { get; set; }

    /// <summary>BD_01=9 | Número da Via da Linha CPTM | Editável | Cache
    /// Domínio: GEA_TX_VIA_CPTM. Ex: "Via 03E - Trecho 2"</summary>
    [Column("NR_VIA_LINHA_CPTM")]
    [MaxLength(30)]
    public string? NrViaLinhaCptm { get; set; }

    /// <summary>BD_01=10 | Trecho e Sentido da Linha CPTM | Editável | Cache
    /// Domínio: GEA_TX_TRECHO_E_SENTIDO_CPTM
    /// Ex: "Estação Antônio Gianetti Neto - Estação Ferraz de Vasconcelos"</summary>
    [Column("DS_TRECHO_SENTIDO_CPTM")]
    [MaxLength(100)]
    public string? DsTrechoSentidoCptm { get; set; }

    /// <summary>BD_01=11 | Número do Quilômetro e Poste | Editável | Cache
    /// Padrão: "00/00" ou "000/000". Ex: "51/02"</summary>
    [Column("NR_KM_POSTE")]
    [MaxLength(10)]
    public string? NrKmPoste { get; set; }

    /// <summary>BD_01=13 | Latitude em Graus – Datum WGS84 | Editável | Cache
    /// Ex: -23.123456</summary>
    [Column("NR_LATITUDE")]
    public decimal? NrLatitude { get; set; }

    /// <summary>BD_01=14 | Longitude em Graus – Datum WGS84 | Editável | Cache
    /// Ex: -46.123456</summary>
    [Column("NR_LONGITUDE")]
    public decimal? NrLongitude { get; set; }

    // ===================================================================
    // SEÇÃO 7.1 – Regulamentação Ambiental
    // ===================================================================

    /// <summary>BD_01=24 | Tipo de Atividade (Listada) | Editável | Cache
    /// Domínio: EF_TX_TIPO_ATIVIDADE_LISTADA. Ex: "Outro(a)(s)"</summary>
    [Column("DS_TIPO_ATIVIDADE_LIST")]
    [MaxLength(100)]
    public string? DsTipoAtividadeList { get; set; }

    /// <summary>BD_01=25 | Tipo de Atividade (Não Listada) | Editável | Cache
    /// Preenchido quando DsTipoAtividadeList = "Outro(a)(s)". Ex: "Transporte"</summary>
    [Column("DS_TIPO_ATIVIDADE_NLIST")]
    [MaxLength(100)]
    public string? DsTipoAtividadeNlist { get; set; }

    /// <summary>BD_01=26 | Tipo de DRA (Listado) | Editável | Cache
    /// Domínio: EF_TX_TIPO_DRA_LISTADO</summary>
    [Column("DS_TIPO_DRA_LIST")]
    [MaxLength(100)]
    public string? DsTipoDraList { get; set; }

    /// <summary>BD_01=27 | Tipo de DRA (Não Listado) | Editável | Cache
    /// Preenchido quando DsTipoDraList = "Outro(a)(s)". Ex: "Teste"</summary>
    [Column("DS_TIPO_DRA_NLIST")]
    [MaxLength(100)]
    public string? DsTipoDraNlist { get; set; }

    /// <summary>BD_01=28 | Código Identificador do DRA | Editável | Cache
    /// Ex: "Nº 1.285.456", "DFR nº 456.123"</summary>
    [Column("CD_IDENTIFICADOR_DRA")]
    [MaxLength(50)]
    public string? CdIdentificadorDra { get; set; }

    /// <summary>BD_01=29 | Data de Validade do DRA | Editável | Cache</summary>
    [Column("DT_VALIDADE_DRA")]
    public DateTime? DtValidadeDra { get; set; }

    // ===================================================================
    // SEÇÃO 7.2 – Detalhamento
    // ===================================================================

    /// <summary>BD_01=31 | Tipo de Atividade na CPTM | Editável | Cache
    /// Domínio: EF_TX_TIPO_ATIVIDADE_CPTM. Ex: "Empreendimento/Obra"</summary>
    [Column("DS_TIPO_ATIVIDADE_CPTM")]
    [MaxLength(100)]
    public string? DsTipoAtividadeCptm { get; set; }

    /// <summary>BD_01=32 | Nome Edificação/Local da CPTM | Editável | Cache
    /// Domínio: EF_TX_NM_LOCAL_ATIV. Ex: "Estação"</summary>
    [Column("NM_LOCAL_EDIFICACAO")]
    [MaxLength(100)]
    public string? NmLocalEdificacao { get; set; }

    /// <summary>BD_01=33 | Nome Edificação/Local (Complemento) | Editável | Cache
    /// Ex: "Brás"</summary>
    [Column("DS_LOCAL_COMPLEMENTO")]
    [MaxLength(100)]
    public string? DsLocalComplemento { get; set; }

    /// <summary>BD_01=34 | Origem do Efluente | Editável | Cache
    /// Domínio: EF_TX_ORIGEM_EFLUENTE. Ex: "Industrial"</summary>
    [Column("DS_ORIGEM_EFLUENTE")]
    [MaxLength(50)]
    public string? DsOrigemEfluente { get; set; }

    /// <summary>BD_01=35 | Fonte Geradora do Efluente | Editável | Cache
    /// Domínio: EF_TX_FONTE_GERADORA. Ex: "Banheiro químico"</summary>
    [Column("DS_FONTE_GERADORA")]
    [MaxLength(100)]
    public string? DsFonteGeradora { get; set; }

    /// <summary>BD_01=36 | Quantidade em Litros | Editável | Cache
    /// Decimal com até 8 casas. Ex: 9.25</summary>
    [Column("NR_QUANTIDADE_LITROS")]
    public decimal? NrQuantidadeLitros { get; set; }

    /// <summary>BD_01=37 | Tipo de Destinação do Efluente | Editável | Cache
    /// Domínio: EF_TX_TIPO_DESTINACAO. Ex: "Interligação em rede coletora"</summary>
    [Column("DS_TIPO_DESTINACAO")]
    [MaxLength(100)]
    public string? DsTipoDestinacao { get; set; }

    /// <summary>BD_01=38 | Tipo de Veículo | Editável | Cache
    /// Domínio: EF_TX_TIPO_VEICULO. Ex: "Caminhão"</summary>
    [Column("DS_TIPO_VEICULO")]
    [MaxLength(50)]
    public string? DsTipoVeiculo { get; set; }

    /// <summary>BD_01=39 | Identificador/Placa do Veículo | Editável | Cache
    /// Ex: "WAD 105D"</summary>
    [Column("CD_PLACA_VEICULO")]
    [MaxLength(20)]
    public string? CdPlacaVeiculo { get; set; }

    /// <summary>BD_01=40 | Código Identificador da Guia de Remessa | Editável | Cache
    /// Ex: "ID nº 10.456"</summary>
    [Column("CD_GUIA_REMESSA")]
    [MaxLength(50)]
    public string? CdGuiaRemessa { get; set; }

    /// <summary>BD_01=41 | Distância da Via CPTM em Metros | Editável | Cache
    /// Decimal. Ex: 7.58</summary>
    [Column("NR_DISTANCIA_VIA_M")]
    public decimal? NrDistanciaViaM { get; set; }

    /// <summary>BD_01=44 | Observações Gerais: Cadastramento | Editável | Cache
    /// Máx: 255 chars.</summary>
    [Column("DS_OBSERVACOES_CADASTRO")]
    [MaxLength(255)]
    public string? DsObservacoesCadastro { get; set; }

    // ===================================================================
    // NAVEGAÇÃO
    // ===================================================================

    /// <summary>Fotografias associadas ao formulário (BD_02=1 | BD_01 70–73).</summary>
    public ICollection<FotoEfluente> Fotos { get; set; } = [];
}
