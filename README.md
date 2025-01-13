# E-commerce Máxima (CRUD)

https://via.placeholder.com/900x300.png?text=Cadastro+de+Produtos+CRUD

## Visão Geral
Este projeto implementa uma tela de cadastro de produtos (CRUD) para o módulo de administração de uma plataforma de e-commerce. Ele também inclui o desenvolvimento de uma API que permite o consumo dos dados dos produtos via integração.

## Tecnologias Utilizadas
- ASP.NET Core MVC
- .NET 8.0
- Banco de Dados Oracle

### Pacotes utilizados:

- Oracle.EntityFrameworkCore 9.23.60
- Swashbuckle.AspNetCore.Swagger 7.2.0
- Swashbuckle.AspNetCore.SwaggerGen 7.2.0
- Swashbuckle.AspNetCore.SwaggerUI 7.2.0

## Funcionalidades Planejadas
- Gestão de Produtos (CRUD)
- Integração com serviços RESTful
- Autenticação e autorização
- Otimização para escalabilidade futura

---

## Requisitos Técnicos
- **Frontend**: ASP .NET CORE.
- **Backend**: Dois microsserviços:
  - Web Service: .NET Core MVC.
  - Serviço API: .NET Core 8.0 ou versões anteriores.
- **Banco de Dados**: Oracle para persistência de informações.
- **Adicionais**:
  - Documentação detalhada com scripts de instalação.
  - Endpoints da API:
    - `GET /products`: Obter todos os produtos.
    - `POST /products`: Adicionar ou atualizar produtos.
    - `GET /departments`: Obter lista de departamentos.

---

## Diagrama Arquitetural

```plaintext
+---------------------+          +-----------------+
|       Frontend      |<-------->|   Web Service   |
| (ASP.NET Core MVC)  |          | (.NET Core MVC) |
+---------------------+          +-----------------+
          |                           |
          |                           |
          |     +-----------------+   |
          +---->|   Serviço API   |---+
                | (.NET Core 8.0) |
                +-----------------+
                          |
                          |
                +-----------------+
                |   Banco Oracle  |
                +-----------------+

```

## Instalação

Link para Download -> https://www.oracle.com/br/database/technologies/xe-downloads.html

Versão Oracle Database 21c Express Edition for Windows ou versões anteriores

Instalar a IDE SQL Developer:

Link para Download -> https://www.oracle.com/br/database/sqldeveloper/technologies/download/

Oracle IDE 23.1.1.345.2114 ou versões acima

Recomendo instalar com a JDK incluída: Windows 64-bit with JDK 17 included

Criando o Table Space: 

create tablespace crudecmaxima
 datafile
   'C:\oraclexe\app\oracle\oradata\XE\crudecmaxima.dbf' 
		size 100m autoextend on next 50m maxsize 500m
   online
   permanent
   extent management local autoallocate
   segment space management auto;

Criando as tabelas e sequências:

CREATE TABLE MXSUSUARIOS
(	IDUSUARIO INT NOT NULL PRIMARY KEY,
	NOME VARCHAR2(50) NOT NULL,
	LOGIN VARCHAR2(50) NOT NULL,
	SENHA VARCHAR2(50) NOT NULL,
    	STATUS VARCHAR2(1)
	);

CREATE SEQUENCE SEQ_USU START WITH 1 INCREMENT BY 1 NOMAXVALUE;

CREATE TABLE MXSDEPARTAMENTO
(	IDDEPTO INT NOT NULL,
	CODDEPTO VARCHAR2(50) NOT NULL,
	DESCRICAO VARCHAR2(4000),
	STATUS VARCHAR2(1) NOT NULL,
    CONSTRAINT PK_DEP PRIMARY KEY(IDDEPTO,CODDEPTO),
    CONSTRAINT UQ_COD_DEP UNIQUE(CODDEPTO)
	);

CREATE SEQUENCE SEQ_DEP START WITH 1 INCREMENT BY 1 NOMAXVALUE;

CREATE TABLE MXSPRODUTO
(	IDPROD INT NOT NULL,
	CODPROD VARCHAR2(50) NOT NULL,
	DESCRICAO VARCHAR2(4000),
	CODDEPTO VARCHAR2(50) NOT NULL,
	PRECO NUMBER(10, 6) NOT NULL,
	STATUS VARCHAR2(1) NOT NULL,
	CODOPERACAO NUMBER(1) NOT NULL,
	CONSTRAINT FK_COD_DEPTO FOREIGN KEY(CODDEPTO) REFERENCES MXSDEPARTAMENTO(CODDEPTO),
    CONSTRAINT PK_PROD PRIMARY KEY(IDPROD, CODPROD)
	);

CREATE SEQUENCE SEQ_PRD START WITH 1 INCREMENT BY 1 NOMAXVALUE;

CREATE TABLE MXSPRODUTOLOG
(	CODPRODANT VARCHAR2(50),
    	CODPROD VARCHAR2(50),
        CODDEPTOANT VARCHAR2(50),
    	CODDEPTO VARCHAR2(50),
        PRECOANT NUMBER(10, 6),
    	PRECO NUMBER(10, 6),
        STATUSANT VARCHAR2(1),
    	STATUS VARCHAR2(1),
        CODOPERACAOANT NUMBER(1),
    	CODOPERACAO NUMBER(1),
    	DTATUALIZ DATE
	);

CREATE TABLE MXSLOGERRO
(       NOMETRG VARCHAR2(2000),
        DESCRICAO VARCHAR2(4000),
        DATAERRO DATE
        );

Criando as Triggers:

CREATE OR REPLACE TRIGGER TRG_MXSUSUARIOS 
    BEFORE INSERT ON MXSUSUARIOS 
    FOR EACH ROW  
        BEGIN 
        IF :NEW.IDUSUARIO IS NULL THEN 
            SELECT SEQ_USU.NEXTVAL INTO :NEW.IDUSUARIO FROM DUAL; 
        END IF; 
        :NEW.STATUS := 'A';
    END; 

CREATE OR REPLACE TRIGGER TRG_MXSDEPARTAMENTO 
    BEFORE INSERT ON MXSDEPARTAMENTO 
    FOR EACH ROW  
        BEGIN 
        IF :NEW.IDDEPTO IS NULL THEN 
            SELECT SEQ_DEP.NEXTVAL INTO :NEW.IDDEPTO FROM DUAL; 
        END IF; 
	:NEW.STATUS := 'A';
    END; 

CREATE OR REPLACE TRIGGER TRG_MXSPRODUTO
   BEFORE DELETE OR INSERT OR UPDATE ON MXSPRODUTO
   REFERENCING NEW AS NEW OLD AS OLD
   FOR EACH ROW
DECLARE
   OPER NUMBER;                                  
   ECODE NUMBER;                     
   EMESG VARCHAR2(4000);           
BEGIN
   OPER := 0;
   IF INSERTING
   THEN
        IF :NEW.IDPROD IS NULL THEN 
        SELECT SEQ_PRD.NEXTVAL INTO :NEW.IDPROD FROM DUAL; 
    END IF;
    OPER := 0;
   ELSIF UPDATING
   THEN
      OPER := 1;
   ELSIF DELETING
   THEN
      OPER := 2;
   END IF;

   IF (OPER = 2 AND NVL(:OLD.CODOPERACAO,0) != 2)
       THEN
          :NEW.CODOPERACAO := 2;
          RAISE_APPLICATION_ERROR (-20000, 'Não é permitida deleção física do registro, apenas se já estiver inativo!');
   END IF;

   IF (NVL (:NEW.CODOPERACAO, 0) != 2)
       THEN
          :NEW.CODOPERACAO := OPER;
   END IF;
   
EXCEPTION
   WHEN OTHERS
   THEN
      ECODE := SQLCODE;
      EMESG := SQLERRM;
      IF (OPER = 2 AND NVL(:OLD.CODOPERACAO,0) != 2)
      THEN
         RAISE_APPLICATION_ERROR (-20000, 'Não é permitida deleção física do registro, apenas se já estiver inativo!');
      ELSE
         INSERT INTO MXSLOGERRO (DATAERRO, NOMETRG, DESCRICAO) VALUES (SYSDATE, 'TRG_MXSPRODUTO', SUBSTR(TO_CHAR (ECODE) || '-' || EMESG || ' ** ' || DBMS_UTILITY.FORMAT_CALL_STACK,0,4000));
      END IF;
END;

CREATE OR REPLACE TRIGGER TRG_MXSPRODUTO_LOG 
  AFTER INSERT OR DELETE OR UPDATE ON MXSPRODUTO 
  FOR EACH ROW
  DECLARE
    OPER NUMBER(1);
    ECODE NUMBER; 
    EMESG VARCHAR2(4000); 

BEGIN 
    IF INSERTING THEN 
      OPER := 0;
      INSERT INTO MXSPRODUTOLOG 
                  (CODPRODANT,CODPROD,CODDEPTOANT,CODDEPTO,PRECOANT,PRECO,STATUSANT,STATUS,CODOPERACAOANT,CODOPERACAO,DTATUALIZ) 
      VALUES      (NULL,:NEW.CODPROD,NULL,:NEW.CODDEPTO,NULL,:NEW.PRECO,NULL,:NEW.STATUS,NULL,OPER,SYSDATE);  
    ELSIF UPDATING THEN 
      OPER := 1;
      INSERT INTO MXSPRODUTOLOG 
                  (CODPRODANT,CODPROD,CODDEPTOANT,CODDEPTO,PRECOANT,PRECO,STATUSANT,STATUS,CODOPERACAOANT,CODOPERACAO,DTATUALIZ)  
      VALUES      (:OLD.CODPROD,:NEW.CODPROD,:OLD.CODDEPTO,:NEW.CODDEPTO,:OLD.PRECO,:NEW.PRECO,:OLD.STATUS,:NEW.STATUS,:OLD.CODOPERACAO,OPER,SYSDATE);  
    ELSIF DELETING THEN 
      OPER := 2;
      INSERT INTO MXSPRODUTOLOG 
                  (CODPRODANT,CODPROD,CODDEPTOANT,CODDEPTO,PRECOANT,PRECO,STATUSANT,STATUS,CODOPERACAOANT,CODOPERACAO,DTATUALIZ)  
      VALUES      (:OLD.CODPROD,NULL,:OLD.CODDEPTO,NULL,:OLD.PRECO,NULL,:OLD.STATUS,:NEW.STATUS,:OLD.CODOPERACAO,OPER,SYSDATE);
    END IF; 
    
 EXCEPTION 
   WHEN OTHERS THEN 
      ECODE := SQLCODE; 
      EMESG := SQLERRM; 
      INSERT INTO MXSLOGERRO (DATAERRO, NOMETRG, DESCRICAO) VALUES (SYSDATE,'TRG_MXSPRODUTO_LOG',TO_CHAR (ECODE) || ' - ' || EMESG); 
END; 
