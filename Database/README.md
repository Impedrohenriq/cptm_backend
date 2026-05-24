# Database: Estado Atual e Evolucao Planejada

Este README documenta o estado atual do banco no backend e o plano de evolucao para fotos por URL.

## Estado atual (em uso)

- O backend atual salva fotografias em BLOB na coluna `BL_FOTO` da tabela `TB_FDC_EEA_EF_FOTO`.
- O script alinhado com o backend atual e o `V0__create_database_full.sql`.
- A modelagem base historica permanece no `V1__create_tables.sql`.

Resumo:

- Fotos: `BL_FOTO` (BLOB)
- Tabela de fotos: `TB_FDC_EEA_EF_FOTO`
- Relacao: `TB_FDC_EEA_EF_FOTO.CHAVE_PRIMARIA_MA` -> `TB_FDC_EEA_EF.CHAVE_PRIMARIA_MA`

## Divergencia corrigida

A divergencia entre script e backend foi corrigida para o estado atual:

- `V0__create_database_full.sql` agora esta coerente com persistencia em BLOB.
- O bloco intermediario que migrava para `DS_URL_FOTO` no V0 foi removido para evitar incompatibilidade com a API atual.

## Evolucao planejada (futuro)

Mais para frente, queremos atualizar a aplicacao para salvar fotos por URL no banco, em vez de BLOB.

Direcao planejada:

1. Criar/usar coluna `DS_URL_FOTO` (VARCHAR2(1000)) na `TB_FDC_EEA_EF_FOTO`.
2. Fazer upload de imagem para Azure Blob Storage no backend.
3. Persistir somente URL (publica ou SAS) no banco.
4. Migrar dados historicos de `BL_FOTO` para URL.
5. Remover `BL_FOTO` somente apos validacao completa.

## Quando iniciar a migracao para URL

Passos recomendados:

1. Atualizar modelo/DTO/controller para trafegar URL no lugar de Base64.
2. Integrar de ponta a ponta com `AzureBlobStorageService`.
3. Criar script de migracao para converter BLOB legado em URL.
4. Adicionar testes de criacao, leitura e atualizacao com URL.
5. So entao ajustar script principal para remover BLOB.

## Scripts no diretorio Database

- `V0__create_database_full.sql`: script principal para criacao completa (estado atual BLOB).
- `V1__create_tables.sql`: referencia historica da modelagem base.

## Execucao do V0

1. Estrutura apenas (padrao): manter `DEFINE V0_LOAD_SAMPLE_DATA = N`
2. Estrutura + dados de exemplo/teste: alterar para `DEFINE V0_LOAD_SAMPLE_DATA = Y`
