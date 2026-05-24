-- =====================================================================
-- V2__add_missing_bd01_columns.sql
-- Acrescenta colunas novas do BD_01 na TB_FDC_EEA_EF sem recriar tabela.
-- Uso: aplicar em ambientes que ja foram criados com V0/V1 antigos.
-- =====================================================================

DECLARE
  PROCEDURE add_column_if_missing(p_col_name IN VARCHAR2, p_col_def IN VARCHAR2) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*)
      INTO v_count
      FROM USER_TAB_COLUMNS
     WHERE TABLE_NAME = 'TB_FDC_EEA_EF'
       AND COLUMN_NAME = UPPER(p_col_name);

    IF v_count = 0 THEN
      EXECUTE IMMEDIATE 'ALTER TABLE TB_FDC_EEA_EF ADD (' || p_col_name || ' ' || p_col_def || ')';
    END IF;
  END;
BEGIN
  add_column_if_missing('DS_STATUS_DESVIO_AMBIENTAL', 'VARCHAR2(50)');
  add_column_if_missing('DS_STATUS_REGISTRO_BD', 'VARCHAR2(50)');
  add_column_if_missing('NM_EMPRESA_EXECUTORA', 'VARCHAR2(255)');
  add_column_if_missing('NR_CONTRATO_SUPERVISORA', 'VARCHAR2(255)');
  add_column_if_missing('NR_LATITUDE_SIRGAS2000', 'NUMBER(12,3)');
  add_column_if_missing('NR_LONGITUDE_SIRGAS2000', 'NUMBER(12,3)');
  add_column_if_missing('DS_ANALISE_CPTM_APROVACAO', 'VARCHAR2(255)');
  add_column_if_missing('DS_OFERECE_RISCO_SIST_CPTM', 'VARCHAR2(30)');
  add_column_if_missing('DS_DOMINIO_TERRITORIAL', 'VARCHAR2(100)');
  add_column_if_missing('NM_AUTOR_PJ_CADASTRAMENTO', 'VARCHAR2(255)');
  add_column_if_missing('NM_ARQUIVO_RVT_RELACIONADO', 'VARCHAR2(255)');
  add_column_if_missing('CD_ELEMENTO_MONITOR_RVT', 'VARCHAR2(255)');
  add_column_if_missing('NM_ARQUIVO_DAC_RELACIONADO', 'VARCHAR2(255)');
  add_column_if_missing('CD_ELEMENTO_MONITOR_DAC', 'VARCHAR2(255)');
  add_column_if_missing('NM_ARQUIVO_CNC_RELACIONADO', 'VARCHAR2(255)');
  add_column_if_missing('CD_ELEMENTO_MONITOR_CNC', 'VARCHAR2(255)');
  add_column_if_missing('CD_ULTIMO_RRA', 'VARCHAR2(255)');
  add_column_if_missing('CD_CEDOC', 'VARCHAR2(255)');
END;
/

DECLARE
  v_len NUMBER;
BEGIN
  SELECT DATA_LENGTH
    INTO v_len
    FROM USER_TAB_COLUMNS
   WHERE TABLE_NAME = 'TB_FDC_EEA_EF'
     AND COLUMN_NAME = 'DS_OBSERVACOES_CADASTRO';

  IF v_len < 2000 THEN
    EXECUTE IMMEDIATE 'ALTER TABLE TB_FDC_EEA_EF MODIFY (DS_OBSERVACOES_CADASTRO VARCHAR2(2000))';
  END IF;
END;
/

COMMIT;
