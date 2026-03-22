-- =====================================================================
-- CPTM – Companhia Paulista de Trens Metropolitanos
-- Gerência de Meio Ambiente – GEA
-- Programa Ambiental: Efluentes e Emissões Atmosféricas – EEA
-- Natureza: Efluentes – EF
-- Formulário de Cadastramento/Caracterização – FDC  (Formulário F_01)
--
-- Script Oracle: Criação de tabelas, sequences, triggers e índices
-- Versão : 1.0
-- Data   : 2026-03-20
-- =====================================================================


-- -----------------------------------------------------------------------
-- 1. TABELA PRINCIPAL  –  TB_FDC_EEA_EF
--
--    Armazena todos os 51 campos do formulário F_01 (FDC-EEA.EF).
--
--    Mapeamento BD_01 → coluna:
--       BD_01= 1 → CHAVE_PRIMARIA_MA       BD_01= 2 → NR_ELEMENTO_MONIT
--       BD_01= 3 → NM_ELEMENTO_MONIT       BD_01= 4 → SG_AREA_MEIO_AMBIENTE
--       BD_01= 7 → NM_MUNICIPIO            BD_01= 8 → NM_LINHA_CPTM
--       BD_01= 9 → NR_VIA_LINHA_CPTM       BD_01=10 → DS_TRECHO_SENTIDO_CPTM
--       BD_01=11 → NR_KM_POSTE             BD_01=12 → NM_ESTACAO_CPTM
--       BD_01=13 → NR_LATITUDE             BD_01=14 → NR_LONGITUDE
--       BD_01=17 → NM_LOCAL_ESCOPO         BD_01=18 → DS_TIPO_FORMULARIO
--       BD_01=19 → DT_EMISSAO_FORMULARIO   BD_01=20 → NR_FORMULARIO
--       BD_01=21 → NM_AUTOR_FORMULARIO     BD_01=22 → DS_NATUREZA_PGA
--       BD_01=24 → DS_TIPO_ATIVIDADE_LIST  BD_01=25 → DS_TIPO_ATIVIDADE_NLIST
--       BD_01=26 → DS_TIPO_DRA_LIST        BD_01=27 → DS_TIPO_DRA_NLIST
--       BD_01=28 → CD_IDENTIFICADOR_DRA    BD_01=29 → DT_VALIDADE_DRA
--       BD_01=31 → DS_TIPO_ATIVIDADE_CPTM  BD_01=32 → NM_LOCAL_EDIFICACAO
--       BD_01=33 → DS_LOCAL_COMPLEMENTO    BD_01=34 → DS_ORIGEM_EFLUENTE
--       BD_01=35 → DS_FONTE_GERADORA       BD_01=36 → NR_QUANTIDADE_LITROS
--       BD_01=37 → DS_TIPO_DESTINACAO      BD_01=38 → DS_TIPO_VEICULO
--       BD_01=39 → CD_PLACA_VEICULO        BD_01=40 → CD_GUIA_REMESSA
--       BD_01=41 → NR_DISTANCIA_VIA_M      BD_01=44 → DS_OBSERVACOES_CADASTRO
--       BD_01=45 → DT_CADASTRAMENTO        BD_01=46 → HR_CADASTRAMENTO
--       BD_01=48 → NM_AUTOR_CADASTRAMENTO  BD_01=49 → NM_RESPONSAVEL_TECNICO
--       BD_01=50 → NR_REGISTRO_PROFISSIONAL BD_01=51 → DS_DOC_RESP_TECNICA
--       BD_01=52 → NM_CONTRATADA           BD_01=53 → NR_CONTRATO
--       BD_01=54 → NM_AREA_GESTORA_CPTM    BD_01=55 → CD_IDENT_AREA_GESTORA
--       BD_01=56 → SG_AREA_GESTORA_CPTM    BD_01=57 → NM_REPRESENTANTE
--       BD_01=58 → NM_SUPERVISORA_AMBIENTAL BD_01=60 → NM_ARQUIVO_FDC
--       BD_01=61 → CD_ARQUIVO_FDC
-- -----------------------------------------------------------------------
CREATE TABLE TB_FDC_EEA_EF (

    -- ===================================================================
    -- SEÇÃO 5 – Identificação do E.M.
    -- ===================================================================

    -- BD_01=1 | Chave Primária - Meio Ambiente | Auto | Não Editável
    --   Formato: EEA.EF-A.<ANO>-L.<LINHA>-<SIGLA>-N.<NR>
    --   Exemplo: EEA.EF-A.2026-L.07-CPTM-N.000001
    CHAVE_PRIMARIA_MA           VARCHAR2(60)    NOT NULL,

    -- BD_01=2 | Elemento de Monitoramento – Número | Editável
    --   Padrão: 6 dígitos sequenciais  (000001 – 999999)
    NR_ELEMENTO_MONIT           VARCHAR2(6)     NOT NULL,

    -- BD_01=3 | Elemento de Monitoramento – Nome | Editável
    --   Exemplos: "Caçamba A", "Plataforma 1 da Estação Brás", "Banheiro Químico B"
    NM_ELEMENTO_MONIT           VARCHAR2(100),

    -- ===================================================================
    -- SEÇÃO 1 – Premissas Institucionais / Cabeçalho
    -- ===================================================================

    -- BD_01=52 | Nome (PJ) da Contratada | Editável | Cache
    --   Formato: "Nome Completo - SIGLA"  (sigla ≤ 10 chars, maiúscula, sem espaço)
    --   Exemplo: "Companhia Paulista de Trens Metropolitanos - CPTM"
    NM_CONTRATADA               VARCHAR2(100)   NOT NULL,

    -- BD_01=53 | Nº do Contrato da Contratada | Editável | Cache
    --   Padrão: até 12 chars sem espaços.  Ex: "001001001001", "AR01234-56"
    NR_CONTRATO                 VARCHAR2(12),

    -- BD_01=17 | Local do Escopo Contratual (Pseudônimo) | Editável | Cache
    --   Exemplos: "Túnel da Estação da Luz", "Pátio Capuava", "Linha 07 - Rubi"
    NM_LOCAL_ESCOPO             VARCHAR2(100),

    -- BD_01=57 | Representante (PF) da Contratada e/ou Área Gestora | Editável | Cache
    --   Máximo: 89 chars.  Ex: "Representante da Contratada / Representante da CPTM"
    NM_REPRESENTANTE            VARCHAR2(89),

    -- BD_01=4  | Sigla da Área de Meio Ambiente | Editável | Cache
    --   Domínio: GEA_TX_SIGLA_DEPTO_MEIO_AMBIENTE
    --   Exemplos: "GEA.DEAE", "GEA.DEAO"
    SG_AREA_MEIO_AMBIENTE       VARCHAR2(20),

    -- BD_01=54 | Nome da Área Gestora CPTM | Editável | Cache
    --   Domínio: GEA_TX_NM_AREA_GESTORA_CPTM
    NM_AREA_GESTORA_CPTM        VARCHAR2(200),

    -- BD_01=55 | Identificador da Área Gestora CPTM | Auto | Não Editável | Não Visível
    --   Formato: ID.<X>-<X>-<X>-<X>-<X>  Ex: "ID.10-15-5-3-0000"
    CD_IDENT_AREA_GESTORA       VARCHAR2(30),

    -- BD_01=56 | Sigla da Área Gestora CPTM | Auto | Não Editável | Não Visível
    --   Formato: DO.XXX.XXXX.0000  Ex: "DO.GOF.DOFS.0000"
    SG_AREA_GESTORA_CPTM        VARCHAR2(20),

    -- BD_01=58 | Nome (PJ) da Supervisora Ambiental | Editável | Cache
    --   Máximo: 89 chars.
    --   Exemplos: "Empresa Supervisora Ambiental Ltda - ESA", "GEA.DEAO"
    NM_SUPERVISORA_AMBIENTAL    VARCHAR2(89),

    -- ===================================================================
    -- SEÇÃO 2 – Identificação do Cadastrador e Responsável Técnico
    -- ===================================================================

    -- BD_01=48 | Autor(a) (PF) do Cadastramento | Editável | Cache
    NM_AUTOR_CADASTRAMENTO      VARCHAR2(89)    NOT NULL,

    -- BD_01=49 | Responsável Técnico – RT pelo Cadastramento | Editável | Cache
    NM_RESPONSAVEL_TECNICO      VARCHAR2(89)    NOT NULL,

    -- BD_01=50 | Registro Profissional do RT | Editável | Cache
    --   Ex: "CREA - 123456 - Pessoa 5"
    NR_REGISTRO_PROFISSIONAL    VARCHAR2(50),

    -- BD_01=51 | Documento de Responsabilidade Técnica do RT | Editável | Cache
    --   Ex: "ART nº 123456 - Pessoa 5"
    DS_DOC_RESP_TECNICA         VARCHAR2(100),

    -- ===================================================================
    -- SEÇÃO 3 – Identificação do Formulário
    -- ===================================================================

    -- BD_01=22 | Natureza (do PGA) | Editável | Cache
    --   Domínio: GEA_TX_NATUREZA_DO_PGA
    --   Exemplo: "Efluente", "Emissões Atmosféricas"
    DS_NATUREZA_PGA             VARCHAR2(50),

    -- BD_01=18 | Tipo de Formulário | Auto | Não Editável
    --   Valor fixo: "Formulário de Cadastramento - FDC (FDC-EEA.EF)"
    DS_TIPO_FORMULARIO          VARCHAR2(100)   DEFAULT 'Formulário de Cadastramento - FDC (FDC-EEA.EF)',

    -- BD_01=19 | Data de Emissão do Formulário | Editável | Cache  (dd/mm/aaaa)
    DT_EMISSAO_FORMULARIO       DATE            NOT NULL,

    -- BD_01=20 | Número do Formulário | Editável | Cache
    --   Padrão: 6 dígitos sequenciais, não replicável.  000001 – 999999
    NR_FORMULARIO               VARCHAR2(6)     NOT NULL,

    -- BD_01=21 | Autor(a) (PF) do Formulário | Editável | Cache
    NM_AUTOR_FORMULARIO         VARCHAR2(89),

    -- BD_01=60 | Nome do arquivo FDC relacionado | Auto | Não Editável
    --   Ex: "DeaoCtAr01823-02FdcEeaEfL10ProgaiaN000001"
    NM_ARQUIVO_FDC              VARCHAR2(200),

    -- BD_01=61 | Código do arquivo FDC relacionado | Auto | Não Editável
    --   Formato: FDC-EEA.EF-A.<ANO>-L.<LINHA>-<SIGLA>-N.<NR>
    --   Ex: "FDC-EEA.EF-A.2026-L.07-CPTM-N.000001"
    CD_ARQUIVO_FDC              VARCHAR2(50),

    -- ===================================================================
    -- SEÇÃO 4 – Data e Hora do Cadastro do E.M.
    -- ===================================================================

    -- BD_01=45 | Data do Cadastramento | Editável | Cache  (dd/mm/aaaa)
    DT_CADASTRAMENTO            DATE            NOT NULL,

    -- BD_01=46 | Hora do Cadastramento | Editável | Cache  (hh:mm)
    HR_CADASTRAMENTO            VARCHAR2(5),

    -- ===================================================================
    -- SEÇÃO 6 – Localização do E.M.
    -- ===================================================================

    -- BD_01=7  | Nome de Município | Editável | Cache
    --   Domínio: GEA_TX_MUNICIPIO
    NM_MUNICIPIO                VARCHAR2(100),

    -- BD_01=8  | Nome da Linha CPTM | Editável | Cache
    --   Domínio: GEA_TX_LINHA_CPTM.  Ex: "Linha 07 - Rubi"
    NM_LINHA_CPTM               VARCHAR2(50),

    -- BD_01=12 | Nome da Estação CPTM | Editável | Cache
    --   Domínio: GEA_TX_ESTACAO_CPTM.  Ex: "Estação Jardim Helena - Vila Mara"
    NM_ESTACAO_CPTM             VARCHAR2(100),

    -- BD_01=9  | Número da Via da Linha CPTM | Editável | Cache
    --   Domínio: GEA_TX_VIA_CPTM.  Ex: "Via 03E - Trecho 2"
    NR_VIA_LINHA_CPTM           VARCHAR2(30),

    -- BD_01=10 | Trecho e Sentido da Linha CPTM | Editável | Cache
    --   Domínio: GEA_TX_TRECHO_E_SENTIDO_CPTM
    --   Ex: "Estação Antônio Gianetti Neto - Estação Ferraz de Vasconcelos"
    DS_TRECHO_SENTIDO_CPTM      VARCHAR2(100),

    -- BD_01=11 | Número do Quilômetro e Poste | Editável | Cache
    --   Padrão: "00/00" ou "000/000".  Ex: "51/02", "210/125"
    NR_KM_POSTE                 VARCHAR2(10),

    -- BD_01=13 | Latitude em Graus – Datum WGS84 | Editável | Cache
    --   Ex: -23.123456
    NR_LATITUDE                 NUMBER(12, 6),

    -- BD_01=14 | Longitude em Graus – Datum WGS84 | Editável | Cache
    --   Ex: -46.123456
    NR_LONGITUDE                NUMBER(12, 6),

    -- ===================================================================
    -- SEÇÃO 7.1 – Regulamentação Ambiental
    -- ===================================================================

    -- BD_01=24 | Tipo de Atividade (Listada) | Editável | Cache
    --   Domínio: EF_TX_TIPO_ATIVIDADE_LISTADA.  Ex: "Outro(a)(s)"
    DS_TIPO_ATIVIDADE_LIST      VARCHAR2(100),

    -- BD_01=25 | Tipo de Atividade (Não Listada) | Editável | Cache
    --   Preenchido quando DS_TIPO_ATIVIDADE_LIST = "Outro(a)(s)".  Ex: "Transporte"
    DS_TIPO_ATIVIDADE_NLIST     VARCHAR2(100),

    -- BD_01=26 | Tipo de DRA (Listado) | Editável | Cache
    --   Domínio: EF_TX_TIPO_DRA_LISTADO
    DS_TIPO_DRA_LIST            VARCHAR2(100),

    -- BD_01=27 | Tipo de DRA (Não Listado) | Editável | Cache
    --   Preenchido quando DS_TIPO_DRA_LIST = "Outro(a)(s)".  Ex: "Teste"
    DS_TIPO_DRA_NLIST           VARCHAR2(100),

    -- BD_01=28 | Código Identificador do DRA | Editável | Cache
    --   Ex: "Nº 1.285.456", "DFR nº 456.123"
    CD_IDENTIFICADOR_DRA        VARCHAR2(50),

    -- BD_01=29 | Data de Validade do DRA | Editável | Cache  (dd/mm/aaaa)
    DT_VALIDADE_DRA             DATE,

    -- ===================================================================
    -- SEÇÃO 7.2 – Detalhamento
    -- ===================================================================

    -- BD_01=31 | Tipo de Atividade na CPTM | Editável | Cache
    --   Domínio: EF_TX_TIPO_ATIVIDADE_CPTM.  Ex: "Empreendimento/Obra"
    DS_TIPO_ATIVIDADE_CPTM      VARCHAR2(100),

    -- BD_01=32 | Nome Edificação/Local da CPTM | Editável | Cache
    --   Domínio: EF_TX_NM_LOCAL_ATIV.  Ex: "Estação"
    NM_LOCAL_EDIFICACAO         VARCHAR2(100),

    -- BD_01=33 | Nome Edificação/Local (Complemento) | Editável | Cache
    --   Ex: "Brás"
    DS_LOCAL_COMPLEMENTO        VARCHAR2(100),

    -- BD_01=34 | Origem do Efluente | Editável | Cache
    --   Domínio: EF_TX_ORIGEM_EFLUENTE.  Ex: "Industrial"
    DS_ORIGEM_EFLUENTE          VARCHAR2(50),

    -- BD_01=35 | Fonte Geradora do Efluente | Editável | Cache
    --   Domínio: EF_TX_FONTE_GERADORA.  Ex: "Banheiro químico"
    DS_FONTE_GERADORA           VARCHAR2(100),

    -- BD_01=36 | Quantidade em Litros | Editável | Cache
    --   Número decimal com até 8 casas.  Ex: 9.25000000
    NR_QUANTIDADE_LITROS        NUMBER(16, 8),

    -- BD_01=37 | Tipo de Destinação do Efluente | Editável | Cache
    --   Domínio: EF_TX_TIPO_DESTINACAO.  Ex: "Interligação em rede coletora"
    DS_TIPO_DESTINACAO          VARCHAR2(100),

    -- BD_01=38 | Tipo de Veículo | Editável | Cache
    --   Domínio: EF_TX_TIPO_VEICULO.  Ex: "Caminhão"
    DS_TIPO_VEICULO             VARCHAR2(50),

    -- BD_01=39 | Identificador/Placa do Veículo | Editável | Cache
    --   Ex: "WAD 105D"
    CD_PLACA_VEICULO            VARCHAR2(20),

    -- BD_01=40 | Código Identificador da Guia de Remessa | Editável | Cache
    --   Ex: "ID nº 10.456"
    CD_GUIA_REMESSA             VARCHAR2(50),

    -- BD_01=41 | Distância da Via CPTM (Metros) | Editável | Cache
    --   Número decimal.  Padrão: "... ,00".  Ex: 7.58
    NR_DISTANCIA_VIA_M          NUMBER(10, 2),

    -- BD_01=44 | Observações Gerais: Cadastramento | Editável | Cache
    --   Máximo: 255 caracteres
    DS_OBSERVACOES_CADASTRO     VARCHAR2(255),

    -- ==== CONSTRAINTS ====
    CONSTRAINT PK_FDC_EEA_EF PRIMARY KEY (CHAVE_PRIMARIA_MA)
);


-- -----------------------------------------------------------------------
-- 2. SEQUENCE + TABELA DE FOTOGRAFIAS  –  TB_FDC_EEA_EF_FOTO
--
--    BD_02 = 1
--      BD_01=70 → NR_FOTO=1 (Fotografia 1)
--      BD_01=71 → NR_FOTO=2 (Fotografia 2)
--      BD_01=72 → NR_FOTO=3 (Fotografia 3)
--      BD_01=73 → NR_FOTO=4 (Fotografia 4)
--    Tamanho padrão: 3×4, orientação Paisagem/Horizontal
-- -----------------------------------------------------------------------
CREATE SEQUENCE SQ_FDC_EEA_EF_FOTO
    START WITH 1
    INCREMENT BY 1
    NOCACHE
    NOCYCLE;

CREATE TABLE TB_FDC_EEA_EF_FOTO (

    -- PK auto-incremental via trigger + SQ_FDC_EEA_EF_FOTO
    ID_FOTO               NUMBER          NOT NULL,

    -- FK → TB_FDC_EEA_EF
    CHAVE_PRIMARIA_MA     VARCHAR2(60)    NOT NULL,

    -- Número da fotografia do formulário (1=BD_01:70  2=71  3=72  4=73)
    NR_FOTO               NUMBER(1)       NOT NULL,

    -- Imagem binária (Fotografia 3x4, Paisagem/Horizontal)
    BL_FOTO               BLOB,

    -- Orientação padrão do formulário: "Paisagem/Horizontal"
    DS_ORIENTACAO         VARCHAR2(30)    DEFAULT 'Paisagem/Horizontal',

    CONSTRAINT PK_FDC_EEA_EF_FOTO    PRIMARY KEY (ID_FOTO),
    CONSTRAINT FK_FOTO_FORMULARIO     FOREIGN KEY (CHAVE_PRIMARIA_MA)
        REFERENCES TB_FDC_EEA_EF (CHAVE_PRIMARIA_MA) ON DELETE CASCADE,
    CONSTRAINT CK_NR_FOTO_RANGE       CHECK (NR_FOTO BETWEEN 1 AND 4),
    CONSTRAINT UQ_FOTO_POR_FORMULARIO UNIQUE (CHAVE_PRIMARIA_MA, NR_FOTO)
);

-- Trigger: popula ID_FOTO automaticamente via sequence
CREATE OR REPLACE TRIGGER TRG_FDC_EEA_EF_FOTO_BI
    BEFORE INSERT ON TB_FDC_EEA_EF_FOTO
    FOR EACH ROW
BEGIN
    IF :NEW.ID_FOTO IS NULL THEN
        SELECT SQ_FDC_EEA_EF_FOTO.NEXTVAL
          INTO :NEW.ID_FOTO
          FROM DUAL;
    END IF;
END;
/


-- -----------------------------------------------------------------------
-- 3. ÍNDICES
-- -----------------------------------------------------------------------

-- Índice na FK da tabela de fotos
CREATE INDEX IX_FOTO_CHAVE_MA          ON TB_FDC_EEA_EF_FOTO (CHAVE_PRIMARIA_MA);

-- Índices de busca frequente na tabela principal
CREATE INDEX IX_EF_DT_CADASTRAMENTO    ON TB_FDC_EEA_EF (DT_CADASTRAMENTO);
CREATE INDEX IX_EF_DT_EMISSAO          ON TB_FDC_EEA_EF (DT_EMISSAO_FORMULARIO);
CREATE INDEX IX_EF_NM_CONTRATADA       ON TB_FDC_EEA_EF (NM_CONTRATADA);
CREATE INDEX IX_EF_NM_MUNICIPIO        ON TB_FDC_EEA_EF (NM_MUNICIPIO);
CREATE INDEX IX_EF_NM_LINHA_CPTM       ON TB_FDC_EEA_EF (NM_LINHA_CPTM);
CREATE INDEX IX_EF_NM_ESTACAO_CPTM     ON TB_FDC_EEA_EF (NM_ESTACAO_CPTM);
CREATE INDEX IX_EF_NR_FORMULARIO       ON TB_FDC_EEA_EF (NR_FORMULARIO);
CREATE INDEX IX_EF_DS_NATUREZA_PGA     ON TB_FDC_EEA_EF (DS_NATUREZA_PGA);


-- -----------------------------------------------------------------------
-- 4. AUTENTICACAO
-- -----------------------------------------------------------------------
CREATE TABLE TB_USUARIO_APP (
    ID                VARCHAR2(36)    NOT NULL,
    EMAIL             VARCHAR2(180)   NOT NULL,
    PASSWORD_HASH     VARCHAR2(128)   NOT NULL,
    PASSWORD_SALT     VARCHAR2(64)    NOT NULL,
    IS_GESTOR         NUMBER(1)       DEFAULT 0 NOT NULL,
    NOME_EXIBICAO     VARCHAR2(120)   NOT NULL,
    CARGO             VARCHAR2(60)    NOT NULL,
    LINHA             VARCHAR2(80)    NOT NULL,
    CRIADO_EM_UTC     TIMESTAMP(6)    DEFAULT SYSTIMESTAMP NOT NULL,

    CONSTRAINT PK_USUARIO_APP PRIMARY KEY (ID),
    CONSTRAINT CK_USUARIO_APP_IS_GESTOR CHECK (IS_GESTOR IN (0, 1))
);

CREATE UNIQUE INDEX UQ_USUARIO_APP_EMAIL ON TB_USUARIO_APP (EMAIL);

COMMIT;
