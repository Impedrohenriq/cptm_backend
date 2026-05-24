# CPTM Backend API

API REST do projeto de controle ambiental de efluentes da CPTM.

Este README e focado apenas na camada backend: arquitetura, ferramentas, configuracao e operacao.

## 1. Objetivo da API

Disponibilizar endpoints para:

- autenticacao de usuarios;
- cadastro e consulta de formularios de efluentes;
- persistencia Oracle com integridade de relacionamento;
- suporte ao fluxo offline-first do frontend por meio de health check e respostas consistentes para sincronizacao.

## 2. Stack e Ferramentas

- ASP.NET Core Web API (net10.0)
- Entity Framework Core 9
- Oracle.EntityFrameworkCore
- Swashbuckle (Swagger/OpenAPI)
- Azure.Storage.Blobs (opcional por feature flag)

Dependencias principais em [CPTM_Backend/CPTM_Backend.csproj](CPTM_Backend/CPTM_Backend.csproj).

Guia dedicado de seguranca:

- [CPTM_Backend/SECURITY.md](CPTM_Backend/SECURITY.md)

## 3. Arquitetura Aplicada

Estrutura principal:

- [CPTM_Backend/Controllers](CPTM_Backend/Controllers): endpoints HTTP
- [CPTM_Backend/DTOs](CPTM_Backend/DTOs): contratos de entrada/saida
- [CPTM_Backend/Models](CPTM_Backend/Models): entidades persistidas
- [CPTM_Backend/Data/AppDbContext.cs](CPTM_Backend/Data/AppDbContext.cs): mapeamento EF Core
- [CPTM_Backend/Security](CPTM_Backend/Security): hash/salt de senha

Ideias de projeto usadas no backend:

- separacao DTO x Model para desacoplamento de contrato API e schema fisico;
- chave primaria ambiental gerada no servidor para padronizacao;
- foto associada em tabela filha com integridade 1:N;
- endpoint de health para o frontend decidir sincronizacao real.

## 4. Endpoints Principais

Base local padrao: http://localhost:5000

Documentacao interativa:

- GET /swagger

Saude da API:

- GET /health

Autenticacao:

- POST /api/auth/register
- POST /api/auth/login

Formulario de efluentes:

- GET /api/formularios-efluente?pagina=1&tamanho=20
- GET /api/formularios-efluente/{chavePrimaria}
- POST /api/formularios-efluente
- PUT /api/formularios-efluente/{chavePrimaria}
- DELETE /api/formularios-efluente/{chavePrimaria}

Regras relevantes:

- o backend gera a chave ambiental no create;
- update substitui o conjunto de fotos do formulario;
- delete remove registro pai e filhos por cascade.

## 5. Configuracao

Arquivo base: [CPTM_Backend/appsettings.Development.json](CPTM_Backend/appsettings.Development.json)

Chaves importantes:

- ConnectionStrings:OracleDB
- Cors:AllowedOrigins
- AzureBlob:Enabled
- AzureBlob:ConnectionString
- AzureBlob:ContainerName

Exemplo de variavel de ambiente no PowerShell:

    $env:ConnectionStrings__OracleDB="User Id=cptmapp;Password=<SENHA>;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));"

## 6. Como Executar

Na raiz do backend:

    dotnet restore
    dotnet build
    dotnet run

Atalho Windows existente no projeto:

    .\run_project.bat

Parar:

    .\stop_project.bat

## 7. Banco de Dados

Os scripts ficam em [CPTM_Backend/Database](CPTM_Backend/Database).

Resumo:

- V0: criacao completa;
- V1: referencia historica;
- V2: migracao incremental para novas colunas BD_01.

Detalhes completos no README de banco em [CPTM_Backend/Database/README.md](CPTM_Backend/Database/README.md).

## 8. Qualidade e Validacao

Comandos recomendados antes de subir alteracoes:

    dotnet build .\CPTM_Backend.sln

Teste de funcionamento rapido:

1. subir API;
2. abrir /swagger;
3. validar GET /health;
4. criar e consultar um formulario;
5. validar persistencia no Oracle.

## 9. Decisoes Tecnicas Importantes

- Oracle como banco transacional principal por aderencia ao ambiente corporativo.
- EF Core para acelerar evolucao de modelo e reduzir SQL manual no dia a dia da API.
- Swagger para reduzir friccao entre frontend e backend durante integracao.
- CORS dinamico (localhost e lista configuravel) para facilitar dev local e homologacao.

## 10. Troubleshooting Rapido

Erro de conexao Oracle:

1. validar service name, host, porta e credenciais.
2. validar permissao do schema nas tabelas.
3. validar se o banco esta acessivel.

Frontend nao sincroniza:

1. validar GET /health.
2. validar CORS.
3. validar URL da API no frontend.
