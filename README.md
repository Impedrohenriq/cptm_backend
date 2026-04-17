# CPTM - Formulario de Efluentes

Sistema de registro de efluentes composto por dois projetos:

- `CPTM_Frontend/`: Vue 3 + Pinia + Vite PWA
- `CPTM_Backend/`: ASP.NET Core + EF Core + Oracle

## Arquitetura Offline-First

O fluxo do formulario agora separa claramente:

- `rascunho local`: dados ainda em edicao, salvos apenas no dispositivo
- `fila de sincronizacao`: itens prontos para envio, aguardando internet e API saudavel
- `sincronizacao confirmada`: item confirmado pela API .NET e persistido no Oracle

Fluxo ponta a ponta:

`Formulario Vue -> IndexedDB (rascunho/fotos/fila) -> Sync engine -> API .NET (/api/formularios-efluente) -> EF Core -> Oracle`

## Persistencia Local

O frontend deixou de depender de `localStorage` para payload grande.

Agora ficam no IndexedDB:

- formularios locais
- rascunhos em edicao
- fila de sincronizacao pendente
- fotos do formulario em `Blob`
- metadados de sincronizacao

`localStorage` permanece apenas para preferencias simples e autenticacao demo.

## Modelo Local de Inspecao

Cada registro local possui metadados explicitos:

- `localId`
- `chavePrimariaMa`
- `payload`
- `syncStatus`
- `pendingOperation`
- `retryCount`
- `lastError`
- `createdAt`
- `updatedAt`
- `syncedAt`
- `serverConfirmed`

Estados de sincronizacao:

- `rascunho`
- `pendente_sync`
- `sincronizando`
- `sincronizado`
- `erro_sync`

Regra principal: um item so pode virar `sincronizado` depois de resposta positiva da API.

## Retry Automatico e Manual

O frontend possui duas formas de reenvio:

1. `retry automatico`
   - escuta o evento `online`
   - checa `navigator.onLine`
   - consulta `GET /health`
   - processa a fila item a item sem travar tudo por causa de uma falha isolada

2. `retry manual`
   - disponivel na interface do dashboard para itens em `pendente_sync` ou `erro_sync`
   - tambem existe acao para sincronizar a fila inteira

## Verificacao Real da API

O frontend nao assume mais que internet significa backend disponivel.

Antes de sincronizar, ele chama:

- `GET /health`

Se o endpoint nao responder com sucesso:

- o item continua local
- a fila nao e marcada como concluida
- a interface mostra que a API segue indisponivel

## Fotos Offline

As fotos agora seguem este fluxo:

- leitura do arquivo no frontend
- compressao/redimensionamento leve para reduzir quota
- persistencia em `Blob` no IndexedDB
- recriacao de preview ao abrir o formulario novamente
- conversao para Base64 apenas no momento do envio a API

Impacto pratico:

- recarregar o app nao perde fotos anexadas offline
- o envio continua compativel com o backend atual
- o backend envia imagens para Azure Blob Storage e salva apenas a URL no Oracle

## Armazenamento de Fotos no Azure Blob

### Modo atual para testes locais (sem Azure pronto)

Enquanto o ambiente Azure nao estiver pronto, deixe no `appsettings.Development.json`:

- `AzureBlob:Enabled = false`

Com isso:

- a API continua funcionando normalmente para criar/listar/editar formularios
- o envio do formulario nao quebra por falta de Azurite/Azure
- fotos sem URL valida sao ignoradas no backend durante os testes

Quando estiver testando sem upload de imagem, esse modo e o recomendado.

As fotos do formulario agora seguem este fluxo no backend:

1. frontend envia `fotoBase64` e `mimeType`
2. API faz upload no container Azure Blob
3. Oracle grava somente `DS_URL_FOTO` na tabela `TB_FDC_EEA_EF_FOTO`
4. ao consultar o formulario, a API retorna `fotoUrl`

Para visualizar a imagem:

- copie a URL salva em `DS_URL_FOTO`
- abra no navegador

Configuracao esperada em `appsettings`:

- `AzureBlob:Enabled` (`true` para ativar upload; `false` para desativar temporariamente)
- `AzureBlob:ConnectionString`
- `AzureBlob:ContainerName`
- `AzureBlob:PublicAccess` (`true` para URL abrir direto no navegador sem SAS)

### Checklist para ativar quando o Azure estiver pronto

1. Criar Storage Account no Azure.
2. Criar container de imagens (ex.: `inspecoes-efluente`).
3. Configurar `ConnectionString` valida da Storage Account.
4. Definir `AzureBlob:ContainerName` com o nome do container criado.
5. Definir `AzureBlob:Enabled = true`.
6. (Opcional) Definir `AzureBlob:PublicAccess = true` para abrir URL direta.
7. Reiniciar a API.

Resumo de ativacao:

- `appsettings.Development.json` durante testes sem Azure:
   - `Enabled: false`
- quando Azure estiver pronto:
   - `Enabled: true`
   - preencher `ConnectionString` e `ContainerName`

## Backend

### Health Check

O backend expoe um endpoint nativo de saude:

- `GET /health`

Implementacao:

- `Program.cs`
- `builder.Services.AddHealthChecks()`
- `app.MapHealthChecks("/health")`

### Reenvio Seguro

A semantica de API foi preservada:

- `POST /api/formularios-efluente` para criacao
- `PUT /api/formularios-efluente/{chavePrimariaMa}` para atualizacao

A seguranca de retry usa `chavePrimariaMa` como identificador estavel:

- o backend continua rejeitando criacao duplicada com conflito
- o frontend trata `409` de criacao como sinal de possivel registro ja existente e recupera com `PUT`
- isso evita falso positivo de envio e reduz risco de duplicidade indevida

Nenhuma tabela Oracle foi alterada.

## Arquivos Principais

### Frontend

- `src/stores/inspecoes.js`: store Pinia com estados explicitos, fila local e reconciliacao
- `src/services/offlineDb.js`: camada de IndexedDB
- `src/services/offlineMedia.js`: persistencia de fotos em Blob e serializacao
- `src/services/syncEngine.js`: retry item a item
- `src/services/api.js`: health check, DTO e chamadas HTTP
- `src/views/FormularioView.vue`: retomada por `localId`, autosave e envio para fila
- `src/views/DashboardView.vue`: status, retry manual e sync manual
- `src/components/NetworkBar.vue`: conectividade real x saude da API

### Backend

- `Program.cs`: health check
- `Controllers/FormularioEfluenteController.cs`: CRUD preservado
- `Data/AppDbContext.cs`: persistencia Oracle preservada
- `DTOs/FormularioEfluenteDto.cs`: contrato DTO preservado

## Executar

### Backend

```bash
cd CPTM_Backend
dotnet build
dotnet run
```

API padrao:

- `http://localhost:5000`
- `https://localhost:5001`
- `http://localhost:5000/health`

### Frontend

```bash
cd CPTM_Frontend
npm install
npm run build
npm run dev
```

## Cenarios de Teste Recomendados

1. Criar formulario online e confirmar estado `sincronizado`
2. Criar formulario offline, recarregar o app e verificar preservacao do payload
3. Anexar fotos offline, recarregar o app e verificar previews preservados
4. Clicar em enviar offline e validar estado `pendente_sync`
5. Voltar a internet com API saudavel e confirmar sincronizacao automatica
6. Simular internet disponivel com API fora do ar e validar que o item nao vira sincronizado
7. Usar retry manual em item com `erro_sync`
8. Editar item existente, sincronizar de novo e confirmar persistencia via `PUT`
9. Abrir dashboard/gestao e verificar visibilidade dos itens locais pendentes
10. Confirmar que nenhum item vira sincronizado sem resposta positiva da API

## Garantias do Novo Fluxo

- nao ha falso positivo de envio
- o formulario continua utilizavel offline
- fotos nao dependem mais de `localStorage`
- a fila local e real e persistente
- a sincronizacao so fecha quando o backend confirma
- o Oracle continua sendo a persistencia final confirmada
