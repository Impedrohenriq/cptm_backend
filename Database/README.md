# Database: Integração com Azure Blob Storage

Este README descreve o que é necessário no banco de dados e na infraestrutura para armazenar fotos no Azure Blob Storage e gravar apenas a URL (`DS_URL_FOTO`) na tabela `TB_FDC_EEA_EF_FOTO`.

Resumo rápido:

- As imagens não são mais salvas como BLOBs no banco; em vez disso salvamos o URL público (ou SAS) em `DS_URL_FOTO` (VARCHAR2(1000)).
- Use `V0__create_database_full.sql` para criação completa do zero em um único arquivo (standalone, sem dependências).
- `V1__create_tables.sql` fica apenas como referência histórica da modelagem base.

Requisitos Azure

- Storage Account (standard ou premium) criada na subscription desejada.
- Container dedicado (ex.: `cptm-photos`). Recomenda-se não usar o container `$root`.
- Política de acesso:
  - Opção 1 (simples): container com acesso público `Blob` para permitir URLs diretas.
  - Opção 2 (recomendada em produção): container privado + gerar SAS temporário para acesso público, ou usar CDN / backend proxy para servir imagens.
- CORS: configurar CORS no Storage Account para permitir o domínio do frontend (ex.: `https://app.example.com`) e métodos `GET, POST, PUT` conforme necessário.

Configuração no backend

- Variáveis/keys esperadas (exemplo em `appsettings.json`):

  "AzureBlob": {
  "ConnectionString": "<AZURE_STORAGE_CONNECTION_STRING>",
  "ContainerName": "cptm-photos",
  "BaseUrl": "https://`<account>`.blob.core.windows.net/cptm-photos"  -- opcional
  }
- Em produção, coloque a `ConnectionString` no Key Vault ou em variáveis de ambiente e não commit no código.
- Alternativa segura: configurar Managed Identity para o App Service/VM e autenticar via `DefaultAzureCredential`.

Boas práticas de naming e conteúdo

- Nome sugerido para blobs: `<chave_primaria_ma>/<nr_foto>.<ext>`. Exemplo:
  `EEA.EF-A.2026-L.07-CPTM-N.000999/1.jpg`
- Armazene o `Content-Type` correto ao fazer upload (ex.: `image/jpeg`, `image/png`).
- Use metadados opcionais para registrar `uploadedBy`, `uploadedAt`.

CORS example (portal):

- Allowed origins: `https://seu-frontend` (ou `*` em dev)
- Allowed methods: `GET, PUT, POST` (ajustar conforme o fluxo)
- Allowed headers: `*`
- Exposed headers: `x-ms-*`
- Max age: `3600`

Migração de BLOBs existentes (passos sugeridos)

1. Executar `V0__create_database_full.sql` (ele já inclui as etapas de ajuste para `DS_URL_FOTO`).
2. Criar um utilitário (script C# / Node) que:
   - Faz SELECT nos registros com BLOB existente (`BL_FOTO` ou coluna histórica).
   - Para cada foto, faz upload para o Azure Blob seguindo o padrão de nome.
   - Atualiza `TB_FDC_EEA_EF_FOTO.DS_URL_FOTO` com o URL público/SAS retornado.
   - Registra erros e re-tentativas; não remova os BLOBs até validar tudo.
3. Após validação completa, remover coluna BLOB (opcional) e ajustar triggers/índices.

Notas de integração backend ↔ frontend

- O backend espera `DS_URL_FOTO` preenchido ao ler formulários retornados pela API.
- Quando o frontend envia novas fotos enquanto está online, a aplicação pode:
  - Fazer upload diretamente ao backend que usa `AzureBlobStorageService` para enviar ao Azure;
  - Ou (se arquitetado) enviar diretamente ao Azure (SAS/upload client) e enviar a URL ao backend.
- Se optar por upload direto do cliente, garantir que o backend valide/sanitize a URL antes de persistir.

Segurança

- Não armazene `ConnectionString` em repositório. Use Azure Key Vault ou variáveis de ambiente.
- Revogue SAS ou rotacione chaves quando necessário.
- Controle quem pode apagar blobs (RBAC ou roles no app).

Exemplo rápido de URL esperado

https://`<account>`.blob.core.windows.net/cptm-photos/EEA.EF-A.2026-L.07-CPTM-N.000999/1.jpg

Referências dos scripts no diretório `Database`:

- `V0__create_database_full.sql` — script mestre standalone para criação completa e modelagem do zero
- `V1__create_tables.sql` — referência da modelagem base (não é necessário para executar o V0)

Criação geral em um comando:

1. `V0__create_database_full.sql`

Modo de execução do V0:

1. Estrutura apenas (padrão): manter `DEFINE V0_LOAD_SAMPLE_DATA = N`
2. Estrutura + dados de exemplo/teste: alterar para `DEFINE V0_LOAD_SAMPLE_DATA = Y`
