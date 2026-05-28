-- =====================================================================
-- V3__fix_foto_schema_compat.sql
-- Corrige compatibilidade do schema de fotos para upload em BLOB.
-- Uso: aplicar em ambientes antigos/heterogeneos antes da sincronizacao.
-- =====================================================================

DECLARE
  v_count NUMBER;
BEGIN
  -- 1) Garantir coluna BL_FOTO (BLOB)
  SELECT COUNT(*)
    INTO v_count
    FROM USER_TAB_COLUMNS
   WHERE TABLE_NAME = 'TB_FDC_EEA_EF_FOTO'
     AND COLUMN_NAME = 'BL_FOTO';

  IF v_count = 0 THEN
    EXECUTE IMMEDIATE 'ALTER TABLE TB_FDC_EEA_EF_FOTO ADD (BL_FOTO BLOB)';
  END IF;
END;
/

DECLARE
  v_count NUMBER;
BEGIN
  -- 2) Garantir sequence usada pelo trigger de ID
  SELECT COUNT(*)
    INTO v_count
    FROM USER_SEQUENCES
   WHERE SEQUENCE_NAME = 'SQ_FDC_EEA_EF_FOTO';

  IF v_count = 0 THEN
    EXECUTE IMMEDIATE '
      CREATE SEQUENCE SQ_FDC_EEA_EF_FOTO
      START WITH 1
      INCREMENT BY 1
      NOCACHE
      NOCYCLE';
  END IF;
END;
/

-- 3) Garantir trigger de autoincremento para ID_FOTO
CREATE OR REPLACE TRIGGER TRG_BI_FDC_EEA_EF_FOTO
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

COMMIT;
