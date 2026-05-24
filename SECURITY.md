# CPTM Security Guide

Guia tecnico de seguranca do projeto CPTM (backend + frontend + banco), com foco academico e rastreabilidade das medidas implementadas.

## 1. Objetivo e Escopo

Este documento consolida:

- ameacas relevantes para o contexto do sistema;
- controles preventivos e detectivos aplicados;
- lacunas residuais e plano de evolucao;
- procedimento de validacao de seguranca.

Escopo de analise:

- API backend em [CPTM_Backend/Program.cs](CPTM_Backend/Program.cs) e [CPTM_Backend/Controllers](CPTM_Backend/Controllers)
- cliente frontend em [CPTM_Frontend/src/services/api.js](CPTM_Frontend/src/services/api.js) e [CPTM_Frontend/src/stores/auth.js](CPTM_Frontend/src/stores/auth.js)
- persistencia Oracle modelada em [CPTM_Backend/Database](CPTM_Backend/Database)

## 2. Visao de Arquitetura de Seguranca

Fluxo atual resumido:

1. Usuario autentica em /api/auth/login ou /api/auth/register.
2. API emite JWT assinado (HMAC SHA-256) com expiração temporal.
3. Frontend persiste token e envia Authorization: Bearer nas rotas protegidas.
4. Backend valida assinatura, issuer, audience e lifetime.
5. Endpoints de formulario exigem autenticacao.

Elementos de seguranca centrais:

- Autenticacao: JWT Bearer.
- Autorizacao: atributo [Authorize] nas rotas sensiveis.
- Transporte: HTTPS + HSTS em producao.
- Superficie de origem: CORS com lista permitida + fallback local restrito a desenvolvimento.
- Persistencia: EF Core/LINQ para reduzir SQL Injection por concatenacao manual.

## 3. Modelo de Ameacas

Ameacas consideradas prioritarias:

1. Bypass de autenticacao

- Risco: acesso anonimo a CRUD de formularios.
- Mitigacao implementada: [Authorize] em [CPTM_Backend/Controllers/FormularioEfluenteController.cs](CPTM_Backend/Controllers/FormularioEfluenteController.cs).

2. Intercepcao de trafego (MITM)

- Risco: captura de token/dados em canais nao cifrados.
- Mitigacoes implementadas:
  - UseHttpsRedirection;
  - HSTS fora de development;
  - frontend apontando para HTTPS local por padrao.

3. SQL Injection

- Risco: manipulacao de query por entrada maliciosa.
- Mitigacao implementada: acesso a dados com EF Core/LINQ (sem SQL raw no fluxo principal).
- Observacao academica: risco residual existe se SQL raw for introduzido no futuro sem parametrizacao.

4. Abuso de upload de imagens

- Risco: payload malformado ou grande causando excecao/DoS.
- Mitigacoes implementadas em [CPTM_Backend/Controllers/FormularioEfluenteController.cs](CPTM_Backend/Controllers/FormularioEfluenteController.cs):
  - limite de quantidade de fotos;
  - bloqueio de duplicidade de NrFoto;
  - validacao de Base64 invalido;
  - limite de tamanho por imagem.

5. Enumeracao e brute force em login

- Estado atual: sem rate limiting dedicado para /api/auth.
- Classificacao: lacuna residual importante.

## 4. Controles Implementados (com rastreabilidade)

### 4.1 Backend

1. JWT Bearer no pipeline

- Arquivo: [CPTM_Backend/Program.cs](CPTM_Backend/Program.cs)
- Pontos tecnicos:
  - AddAuthentication/AddJwtBearer;
  - validacao de issuer, audience, assinatura e tempo de vida;
  - ClockSkew reduzido;
  - RequireHttpsMetadata ativo fora de development.

2. HSTS e HTTPS

- Arquivo: [CPTM_Backend/Program.cs](CPTM_Backend/Program.cs)
- Pontos tecnicos:
  - redirecionamento HTTP para HTTPS;
  - HSTS em ambiente nao-desenvolvimento.

3. CORS controlado

- Arquivo: [CPTM_Backend/Program.cs](CPTM_Backend/Program.cs)
- Pontos tecnicos:
  - origins explicitamente permitidas via configuracao;
  - fallback localhost aceito apenas em development.

4. Endpoints protegidos

- Arquivo: [CPTM_Backend/Controllers/FormularioEfluenteController.cs](CPTM_Backend/Controllers/FormularioEfluenteController.cs)
- Ponto tecnico:
  - [Authorize] no controller para exigir JWT.

5. Emissao de token no fluxo de auth

- Arquivo: [CPTM_Backend/Controllers/AuthController.cs](CPTM_Backend/Controllers/AuthController.cs)
- Pontos tecnicos:
  - token assinado no login e no register;
  - claims de identificacao e perfil;
  - expiracao controlada no servidor.

6. Contrato de auth expandido

- Arquivo: [CPTM_Backend/DTOs/AuthDtos.cs](CPTM_Backend/DTOs/AuthDtos.cs)
- Ponto tecnico:
  - resposta inclui Token e ExpiresAtUtc.

7. Dependencia de seguranca adicionada

- Arquivo: [CPTM_Backend/CPTM_Backend.csproj](CPTM_Backend/CPTM_Backend.csproj)
- Ponto tecnico:
  - Microsoft.AspNetCore.Authentication.JwtBearer.

### 4.2 Frontend

1. Persistencia e uso de token

- Arquivo: [CPTM_Frontend/src/stores/auth.js](CPTM_Frontend/src/stores/auth.js)
- Pontos tecnicos:
  - token salvo em storage local;
  - sessao invalida sem token;
  - limpeza de token no logout.

2. Header Authorization automatizado

- Arquivo: [CPTM_Frontend/src/services/api.js](CPTM_Frontend/src/services/api.js)
- Ponto tecnico:
  - injecao de Authorization: Bearer em chamadas autenticadas.

3. Base HTTPS por padrao

- Arquivo: [CPTM_Frontend/src/services/api.js](CPTM_Frontend/src/services/api.js)
- Ponto tecnico:
  - fallback para https://localhost:5001.

## 5. Configuracoes Criticas

Arquivos:

- [CPTM_Backend/appsettings.json](CPTM_Backend/appsettings.json)
- [CPTM_Backend/appsettings.Development.json](CPTM_Backend/appsettings.Development.json)

Parametros chave:

- Jwt:Issuer
- Jwt:Audience
- Jwt:Key
- Cors:AllowedOrigins

Requisito de seguranca:

- Jwt:Key deve ter no minimo 32 caracteres.

Recomendacao para ambiente real:

- usar secret store/variavel de ambiente para Jwt:Key;
- nao manter segredo real versionado no repositorio;
- rotacionar chave periodicamente.

## 6. Validacao de Seguranca (roteiro de teste)

## 6.1 Testes de autenticacao/autorizacao

1. Acessar GET /api/formularios-efluente sem token.

- Esperado: 401 Unauthorized.

2. Realizar login valido e capturar token.

- Esperado: response com token + expiresAtUtc.

3. Acessar GET /api/formularios-efluente com Bearer token.

- Esperado: 200 OK.

4. Usar token invalido/alterado.

- Esperado: 401 Unauthorized.

## 6.2 Testes de transporte

1. Forcar chamada HTTP em ambiente produtivo.

- Esperado: redirecionamento para HTTPS.

2. Verificar cabecalho Strict-Transport-Security.

- Esperado: presente em ambiente nao-development.

## 6.3 Testes de entrada maliciosa

1. Enviar foto com Base64 invalido.

- Esperado: 400 com mensagem de validacao.

2. Enviar mais de 4 fotos.

- Esperado: 400.

3. Repetir NrFoto no mesmo formulario.

- Esperado: 400.

## 6.4 Testes de resiliencia

1. Simular tentativas repetidas de login com credenciais invalidas.

- Resultado atual: endpoint responde normalmente sem throttling.
- Conclusao: implementar rate limiting.

## 7. Lacunas Residuais e Plano de Evolucao

Prioridade alta:

1. Rate limiting nos endpoints de auth.
2. Rotacao de chave JWT e politica de expiracao/refresh token.
3. Revisao de armazenamento de token no frontend (migracao para cookie HttpOnly se o desenho arquitetural permitir).

Prioridade media:

1. Auditoria estruturada de seguranca (eventos de login, falha e acesso negado).
2. Politica de senha com regras de complexidade e bloqueio progressivo.
3. Monitoramento e alertas para padrao anomalo de autenticacao.

Prioridade baixa:

1. Formalizar baseline de headers de seguranca adicionais.
2. Testes automatizados de regressao para casos de seguranca.

## 8. Checklist de Entrega Segura

Checklist operacional por release:

- [ ] build backend sem erros
- [ ] build frontend sem erros
- [ ] endpoints sensiveis protegidos por [Authorize]
- [ ] Jwt:Key configurada fora do codigo
- [ ] CORS alinhado aos domínios reais
- [ ] teste 401 sem token executado
- [ ] teste 200 com token valido executado
- [ ] teste de upload invalido executado

## 9. Conclusao Academica

O projeto evoluiu de um estado funcional para um estado com controles de seguranca essenciais na camada de aplicacao, com foco em:

- autenticacao forte por token assinado;
- protecao de superficie de API;
- endurecimento de transporte;
- validacao defensiva de entrada.

Do ponto de vista academico, o sistema ja incorpora controles basicos de seguranca em profundidade, mas ainda requer maturacao em controles anti-abuso (rate limiting), governanca de segredos e observabilidade de seguranca para atingir um nivel mais robusto em cenarios de producao.
