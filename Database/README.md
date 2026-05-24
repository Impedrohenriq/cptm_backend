# CPTM Database (Oracle)

Documentacao da camada de banco de dados do projeto de efluentes.

Este README cobre modelagem, scripts, estrategia de evolucao e as ideias usadas no desenho do schema.

## 1. Objetivo do Modelo

Persistir com integridade os dados do formulario FDC-EEA.EF, incluindo:

- dados institucionais e tecnicos do cadastro;
- registro fotografico associado;
- usuarios da aplicacao.

## 2. Tabelas Principais

1. TB_FDC_EEA_EF

- tabela principal do formulario;
- chave primaria: CHAVE_PRIMARIA_MA;
- campos BD_01 ampliados (incluindo novos atributos de status, risco, rastreabilidade e coordenadas SIRGAS2000).

2. TB_FDC_EEA_EF_FOTO

- fotos 1..4 por formulario;
- FK para TB_FDC_EEA_EF;
- coluna de imagem atual: BL_FOTO (BLOB);
- unicidade por formulario e numero da foto.

3. TB_USUARIO_APP

- autenticacao local;
- email unico;
- controle de perfil gestor.

## 3. Scripts Disponiveis

- [CPTM_Backend/Database/V0__create_database_full.sql](CPTM_Backend/Database/V0__create_database_full.sql)
	script principal para criar tudo do zero, com opcao de carga de dados de exemplo.

- [CPTM_Backend/Database/V1__create_tables.sql](CPTM_Backend/Database/V1__create_tables.sql)
	referencia historica da modelagem base.

- [CPTM_Backend/Database/V2__add_missing_bd01_columns.sql](CPTM_Backend/Database/V2__add_missing_bd01_columns.sql)
	migracao incremental idempotente para bancos existentes que ainda nao tenham as novas colunas BD_01.

## 4. Como Aplicar os Scripts

Nova base (recomendado):

1. executar V0.
2. definir V0_LOAD_SAMPLE_DATA = Y apenas se quiser dados de exemplo.

Base ja existente:

1. executar V2 para adicionar colunas faltantes e ajustar tamanho de observacoes.

## 5. Regras de Integridade

Principais garantias do modelo:

- PK em TB_FDC_EEA_EF por chave ambiental;
- FK em TB_FDC_EEA_EF_FOTO com ON DELETE CASCADE;
- CHECK NR_FOTO entre 1 e 4;
- UNIQUE CHAVE_PRIMARIA_MA + NR_FOTO;
- indices para consulta por data, natureza e localizacao.

## 6. Decisoes Tecnicas Utilizadas

1. Oracle como banco principal

- aderencia ao contexto corporativo;
- suporte robusto a constraints e governanca.

2. Fotos em BLOB no estado atual

- simplifica consistencia transacional no desenho atual;
- elimina dependencia externa obrigatoria para salvar imagem.

3. Evolucao por scripts incrementais

- V2 permite atualizar ambiente sem recriar schema;
- abordagem idempotente reduz risco em homologacao/producao.

4. Nomes de colunas alinhados ao negocio

- nomenclatura proxima do BD_01 para rastreabilidade funcional.

## 7. Evolucao Planejada para Midia por URL

Direcao futura:

1. incluir DS_URL_FOTO em TB_FDC_EEA_EF_FOTO;
2. mover upload para Azure Blob via backend;
3. persistir URL no banco em vez de BLOB;
4. migrar legado de BL_FOTO;
5. remover BL_FOTO somente apos validacao completa.

## 8. Validacoes Rapidas Pos-Deploy

Exemplos de consultas de verificacao:

		SELECT table_name FROM user_tables
		WHERE table_name IN ('TB_FDC_EEA_EF', 'TB_FDC_EEA_EF_FOTO', 'TB_USUARIO_APP');

		SELECT sequence_name FROM user_sequences
		WHERE sequence_name = 'SQ_FDC_EEA_EF_FOTO';

		SELECT column_name FROM user_tab_columns
		WHERE table_name = 'TB_FDC_EEA_EF'
			AND column_name IN (
				'DS_STATUS_DESVIO_AMBIENTAL',
				'DS_STATUS_REGISTRO_BD',
				'NR_LATITUDE_SIRGAS2000',
				'NR_LONGITUDE_SIRGAS2000',
				'DS_ANALISE_CPTM_APROVACAO'
			);

## 9. Boas Praticas de Operacao

- versionar mudancas de schema sempre por script incremental;
- aplicar primeiro em dev, depois homolog e producao;
- manter rotina de backup e plano de rollback;
- nao executar alteracoes estruturais diretamente sem versionamento.
