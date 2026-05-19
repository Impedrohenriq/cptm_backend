# CPTM - Plataforma de Controle de Efluentes

Documentacao unificada do projeto completo:
- Frontend (Vue 3 + Pinia + Vite + PWA)
- Backend (ASP.NET Core + EF Core + Oracle)
- API REST
- Banco Oracle
- Fluxo offline-first com sincronizacao

Este documento foi escrito para servir como base tecnica para desenvolvimento, operacao e apresentacao do projeto.

---

## 1) Visao Geral

A plataforma permite cadastrar, editar e sincronizar formularios ambientais de efluentes da CPTM, com suporte a uso em campo mesmo sem conectividade estavel.

Caracteristicas principais:
- Cadastro completo de formulario FDC-EEA.EF
- Persistencia local no dispositivo (IndexedDB)
- Fila de sincronizacao com retry e tratamento de conflitos
- API REST para CRUD de formularios
- Persistencia no Oracle
- Upload de imagens para Azure Blob (com chave de feature para habilitar/desabilitar)
- Health check para validar disponibilidade real da API

---

## 2) Arquitetura do Sistema

### 2.1 Componentes

- Frontend PWA:
  - Vue 3
  - Pinia
  - Vue Router
  - Vite
  - vite-plugin-pwa (service worker injectManifest)

- Backend API:
  - ASP.NET Core (net10.0)
  - Entity Framework Core
  - Oracle EF Provider
  - Swagger/OpenAPI

- Banco:
  - Oracle
  - Tabelas principais: TB_FDC_EEA_EF, TB_FDC_EEA_EF_FOTO, TB_USUARIO_APP

- Armazenamento de midia:
  - Azure Blob Storage (opcional por ambiente)

### 2.2 Fluxo ponta a ponta

1. Usuario preenche formulario no frontend.
2. Dados e fotos ficam salvos localmente (IndexedDB) durante edicao.
3. Ao enviar, item vai para fila local quando necessario.
4. Sync engine verifica conectividade + health da API.
5. API recebe dados em /api/formularios-efluente.
6. Backend grava no Oracle.
7. Frontend marca status sincronizado apenas apos confirmacao da API.

---

## 3) Estrutura do Projeto

- CPTM_Frontend/
  - Aplicacao web/PWA, interface e logica offline

- CPTM_Backend/
  - API .NET, integracao Oracle, regras de persistencia

- CPTM_Backend/Database/
  - Scripts SQL de estrutura e objetos de banco

---

## 4) Requisitos de Ambiente

## 4.1 Ferramentas obrigatorias

- Node.js 18+ (recomendado 20+)
- npm 9+
- .NET SDK 10.0
- Oracle Database (ex.: XE local ou instancia corporativa)

## 4.2 Ferramentas recomendadas

- Oracle SQL Developer ou SQLcl
- VS Code
- Git

## 4.3 Opcional

- Azurite (desenvolvimento local de Blob) ou conta Azure Storage real

---

## 5) Configuracao do Banco Oracle

Script principal:
- Database/V1__create_tables.sql

Esse script cria:
- TB_FDC_EEA_EF (formulario principal)
- TB_FDC_EEA_EF_FOTO (metadados e binario de foto)
- SQ_FDC_EEA_EF_FOTO + trigger de incremento
- TB_USUARIO_APP (autenticacao local)
- Indices principais

Passo a passo (resumo):
1. Conectar no schema Oracle alvo.
2. Executar o script completo.
3. Validar objetos criados.

Comandos de validacao (exemplo SQL):

```sql
SELECT table_name FROM user_tables
WHERE table_name IN ('TB_FDC_EEA_EF', 'TB_FDC_EEA_EF_FOTO', 'TB_USUARIO_APP');

SELECT sequence_name FROM user_sequences
WHERE sequence_name = 'SQ_FDC_EEA_EF_FOTO';
```

Observacao:
- Em producao, separar scripts por versao/migracao e adotar pipeline de banco.

---

## 6) Configuracao do Backend

Arquivo de configuracao:
- appsettings.Development.json

Campos criticos:
- ConnectionStrings:OracleDB
- AzureBlob:Enabled
- AzureBlob:ConnectionString
- AzureBlob:ContainerName
- AzureBlob:PublicAccess

Exemplo de estrutura (sem segredo real):

```json
{
  "ConnectionStrings": {
    "OracleDB": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));"
  },
  "AzureBlob": {
    "Enabled": false,
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "inspecoes-efluente-dev",
    "PublicAccess": true
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:5173" ]
  }
}
```

Regras importantes:
- Nao versionar senha real de banco.
- Em producao, usar variaveis de ambiente/secret manager.
- Se Azure Blob nao estiver disponivel, manter AzureBlob:Enabled=false.

---

## 7) Configuracao do Frontend

Arquivo:
- CPTM_Frontend/.env

Variavel principal:

```env
VITE_API_BASE_URL=http://localhost:5000
```

Ajuste para homolog/producao:
- apontar para a URL publica da API
- revisar CORS no backend para o dominio do frontend

---

## 8) Como Executar o Projeto

## 8.1 Execucao rapida (Windows)

Na pasta CPTM_Backend, usar scripts utilitarios:

```powershell
.\run_project.bat
```

Para parar:

```powershell
.\stop_project.bat
```

Portas padrao:
- Backend: http://localhost:5000 (e https://localhost:5001 quando profile HTTPS)
- Frontend: http://localhost:5173

## 8.2 Execucao manual

Backend:

```powershell
cd C:\Users\pedro\Desktop\CPTM_Backend
dotnet restore
dotnet build
dotnet run
```

Frontend:

```powershell
cd C:\Users\pedro\Desktop\CPTM_Frontend
npm install
npm run dev -- --host
```

---

## 9) Endpoints da API

Base local:
- http://localhost:5000

Swagger:
- /swagger

Health:
- GET /health

### 9.1 Autenticacao

- POST /api/auth/register
- POST /api/auth/login

Exemplo register:

```json
{
  "fullName": "Usuario Exemplo",
  "email": "usuario@dominio.com",
  "password": "Senha123!",
  "confirmPassword": "Senha123!"
}
```

Exemplo login:

```json
{
  "email": "usuario@dominio.com",
  "password": "Senha123!"
}
```

### 9.2 Formularios de Efluente

- GET /api/formularios-efluente?pagina=1&tamanho=20
- GET /api/formularios-efluente/{chavePrimaria}
- POST /api/formularios-efluente
- PUT /api/formularios-efluente/{chavePrimaria}
- DELETE /api/formularios-efluente/{chavePrimaria}

Observacoes de negocio:
- Chave primaria ambiental e gerada no backend (prefixo EEA.EF-A...)
- PUT substitui conjunto de fotos do formulario
- DELETE remove formulario e fotos associadas (cascade)

---

## 10) Modelo de Dados (Resumo)

### 10.1 TB_FDC_EEA_EF

Tabela principal do formulario com campos de:
- identificacao institucional
- localizacao (municipio, linha, estacao, latitude/longitude)
- dados tecnicos de atividade/efluente
- metadados de cadastro

### 10.2 TB_FDC_EEA_EF_FOTO

Tabela de fotos associadas ao formulario:
- FK para CHAVE_PRIMARIA_MA
- NR_FOTO (1 a 4)
- BLOB de imagem (modelo atual)
- orientacao

### 10.3 TB_USUARIO_APP

Tabela de autenticacao:
- email unico
- hash/salt de senha
- perfil gestor ou nao gestor

---

## 11) Fluxo Offline-First

Estados de sincronizacao usados no frontend:
- rascunho
- pendente_sync
- sincronizando
- sincronizado
- erro_sync

Comportamento:
- Frontend salva rascunhos e fila no IndexedDB.
- Sync nao depende apenas de navigator.onLine.
- Antes de enviar, consulta GET /health.
- Erros retryable (rede, 5xx, 408, 429) entram em retry com backoff.
- Conflito 409 em criacao tenta reconciliacao via update.

---

## 12) Testes e Validacao

## 12.1 Frontend

```powershell
cd C:\Users\pedro\Desktop\CPTM_Frontend
npm test
npm run build
```

## 12.2 Backend

```powershell
cd C:\Users\pedro\Desktop\CPTM_Backend
dotnet build
```

## 12.3 Cenarios funcionais recomendados

1. Cadastrar formulario offline e recarregar app.
2. Confirmar persistencia de fotos offline.
3. Enviar sem API disponivel e validar fila pendente.
4. Reestabelecer API e validar sincronizacao automatica.
5. Testar retry manual no dashboard.

---

## 13) Deploy e Operacao

## 13.1 Backend

- Publicar API em ambiente com HTTPS.
- Configurar CORS com dominios reais do frontend.
- Guardar secrets fora de codigo.
- Habilitar logs e monitoramento centralizado.

## 13.2 Frontend

- Build com npm run build.
- Publicar conteudo estatico em servidor/CDN.
- Ajustar VITE_API_BASE_URL para URL final da API.

## 13.3 Banco

- Aplicar scripts por ambiente (dev/hml/prod).
- Garantir backup/restore e politica de auditoria.

## 13.4 Azure Blob

- Em dev sem Azure, usar AzureBlob:Enabled=false.
- Em producao, configurar Storage Account e container.
- Preferir segredo em vault e rotacao de credenciais.

---

## 14) Troubleshooting

### Problema: frontend nao sincroniza

Checklist:
1. API esta respondendo em /health?
2. CORS permite origem do frontend?
3. VITE_API_BASE_URL aponta para a URL correta?
4. Existem itens em erro_sync com detalhe de erro no frontend?

### Problema: backend nao conecta no Oracle

Checklist:
1. String de conexao valida (host, porta, service name, usuario, senha).
2. Usuario/schema com permissoes nas tabelas.
3. Banco ativo e acessivel na rede.

### Problema: erro com upload de imagem

Checklist:
1. AzureBlob:Enabled esta correto para o ambiente?
2. ConnectionString e ContainerName estao validos?
3. Se UseDevelopmentStorage=true, Azurite esta em execucao?

---

## 15) Checklist para Apresentacao

1. Subir backend e frontend.
2. Mostrar Swagger e endpoint /health.
3. Demonstrar cadastro online.
4. Demonstrar cadastro offline e fila pendente.
5. Reestabelecer conectividade e mostrar sincronizacao.
6. Mostrar registros persistidos no Oracle.
7. (Opcional) Mostrar fluxo de foto no Azure Blob.

---

## 16) Governanca de Configuracao

Recomendacoes:
- Nao commitar segredos de banco/cloud.
- Padronizar arquivos de exemplo (.env.example, appsettings.Example.json).
- Versionar mudancas de schema com scripts incrementais.
- Definir processo de rollback para API e banco.

---

## 17) Licenca e Contato

Definir conforme politica institucional do projeto.

---

Ultima atualizacao: Abril/2026
