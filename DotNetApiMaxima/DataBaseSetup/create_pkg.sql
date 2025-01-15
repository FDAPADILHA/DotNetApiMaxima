--Primeiro passo: Criar o pacote PKG_DB_SETUP
CREATE OR REPLACE PACKAGE PKG_DB_SETUP AS
    PROCEDURE CreateTables;
    PROCEDURE CreateSequences;
    PROCEDURE CreateTriggers;
    PROCEDURE CreateJobs;
    PROCEDURE RunAllSetup;
END PKG_DB_SETUP;

--Segundo passo: Criar o corpo do pacote PKG_DB_SETUP
create or replace PACKAGE BODY PKG_DB_SETUP AS

    PROCEDURE CreateTables IS
        BEGIN
            BEGIN
                EXECUTE IMMEDIATE 'CREATE TABLE MXSUSUARIOS
                (
                    IDUSUARIO INT NOT NULL PRIMARY KEY,
                    NOME VARCHAR2(50) NOT NULL,
                    LOGIN VARCHAR2(50) NOT NULL,
                    SENHA VARCHAR2(50) NOT NULL,
                    STATUS VARCHAR2(1)
                )';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar tabela MXSUSUARIOS: ' || SQLERRM);
            END;

            BEGIN
                EXECUTE IMMEDIATE 'CREATE TABLE MXSDEPARTAMENTO
                (
                    IDDEPTO INT NOT NULL,
                    CODDEPTO VARCHAR2(50) NOT NULL,
                    DESCRICAO VARCHAR2(4000),
                    STATUS VARCHAR2(1) NOT NULL,
                    CONSTRAINT PK_DEP PRIMARY KEY(IDDEPTO, CODDEPTO),
                    CONSTRAINT UQ_COD_DEP UNIQUE(CODDEPTO)
                )';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar tabela MXSDEPARTAMENTO: ' || SQLERRM);
            END;

            BEGIN
                EXECUTE IMMEDIATE 'CREATE TABLE MXSPRODUTO
                (
                    IDPROD INT NOT NULL,
                    CODPROD VARCHAR2(50) NOT NULL,
                    DESCRICAO VARCHAR2(4000),
                    CODDEPTO VARCHAR2(50) NOT NULL,
                    PRECO NUMBER(10, 6) NOT NULL,
                    STATUS VARCHAR2(1) NOT NULL,
                    CODOPERACAO NUMBER(1) NOT NULL,
                    CONSTRAINT FK_COD_DEPTO FOREIGN KEY(CODDEPTO) REFERENCES MXSDEPARTAMENTO(CODDEPTO),
                    CONSTRAINT PK_PROD PRIMARY KEY(IDPROD, CODPROD)
                )';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar tabela MXSPRODUTO: ' || SQLERRM);
            END;

            BEGIN
                EXECUTE IMMEDIATE 'CREATE TABLE MXSPRODUTOLOG
                (
                    CODPRODANT VARCHAR2(50),
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
                )';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar tabela MXSPRODUTOLOG: ' || SQLERRM);
            END;

            BEGIN
                EXECUTE IMMEDIATE 'CREATE TABLE MXSLOGERRO
                (
                    NOMETRG VARCHAR2(2000),
                    DESCRICAO VARCHAR2(4000),
                    DATAERRO DATE
                )';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar tabela MXSLOGERRO: ' || SQLERRM);
            END;
    END CreateTables;

    PROCEDURE CreateSequences IS
        BEGIN
            BEGIN
                EXECUTE IMMEDIATE 'CREATE SEQUENCE SEQ_USU START WITH 1 INCREMENT BY 1 NOMAXVALUE';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar sequência SEQ_USU: ' || SQLERRM);
            END;

            BEGIN
                EXECUTE IMMEDIATE 'CREATE SEQUENCE SEQ_DEP START WITH 1 INCREMENT BY 1 NOMAXVALUE';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar sequência SEQ_DEP: ' || SQLERRM);
            END;

            BEGIN
                EXECUTE IMMEDIATE 'CREATE SEQUENCE SEQ_PRD START WITH 1 INCREMENT BY 1 NOMAXVALUE';
            EXCEPTION
                WHEN OTHERS THEN
                    DBMS_OUTPUT.PUT_LINE('Erro ao criar sequência SEQ_PRD: ' || SQLERRM);
            END;
        END CreateSequences;

        PROCEDURE CreateTriggers IS
            BEGIN

                EXECUTE IMMEDIATE q'[
                    CREATE OR REPLACE TRIGGER TRG_MXSUSUARIOS
                    BEFORE INSERT ON MXSUSUARIOS
                    FOR EACH ROW
                    BEGIN
                        IF :NEW.IDUSUARIO IS NULL THEN
                            SELECT SEQ_USU.NEXTVAL INTO :NEW.IDUSUARIO FROM DUAL;
                        END IF;
                        :NEW.STATUS := 'A';
                    END;
                ]';

                EXECUTE IMMEDIATE q'[
                    CREATE OR REPLACE TRIGGER TRG_MXSDEPARTAMENTO
                    BEFORE INSERT ON MXSDEPARTAMENTO
                    FOR EACH ROW
                    BEGIN
                        IF :NEW.IDDEPTO IS NULL THEN
                            SELECT SEQ_DEP.NEXTVAL INTO :NEW.IDDEPTO FROM DUAL;
                        END IF;
                        :NEW.STATUS := 'A';
                    END;
                ]';

                EXECUTE IMMEDIATE q'[
                    CREATE OR REPLACE TRIGGER TRG_MXSPRODUTO
                    BEFORE INSERT OR UPDATE OR DELETE ON MXSPRODUTO
                    REFERENCING NEW AS NEW OLD AS OLD
                    FOR EACH ROW
                    DECLARE
                        OPER NUMBER;
                        ECODE NUMBER;
                        EMESG VARCHAR2(4000);
                        V_COUNT NUMBER;
                    BEGIN
                        OPER := 0;

                        IF INSERTING THEN
                            IF :NEW.IDPROD IS NULL THEN
                                SELECT SEQ_PRD.NEXTVAL INTO :NEW.IDPROD FROM DUAL;
                            END IF;

                            SELECT COUNT(*) INTO V_COUNT
                            FROM MXSPRODUTO
                            WHERE CODPROD = :NEW.CODPROD
                              AND ROWNUM = 1;

                            IF V_COUNT > 0 THEN
                                RAISE_APPLICATION_ERROR(-20001, 'Já existe um produto com o código ' || :NEW.CODPROD);
                            END IF;

                            OPER := 0;
                        ELSIF UPDATING THEN
                            OPER := 1;
                        ELSIF DELETING THEN
                            IF NVL(:OLD.CODOPERACAO, 0) != 2 THEN
                                RAISE_APPLICATION_ERROR(-20000, 'Não é permitida deleção física do registro, apenas se já estiver inativo!');
                            END IF;
                            OPER := 2;
                        END IF;

                        IF NVL(:NEW.CODOPERACAO, 0) != 2 AND NOT DELETING THEN
                            :NEW.CODOPERACAO := OPER;
                        END IF;

                    EXCEPTION
                        WHEN OTHERS THEN
                            ECODE := SQLCODE;
                            EMESG := SQLERRM;
                            INSERT INTO MXSLOGERRO (DATAERRO, NOMETRG, DESCRICAO)
                            VALUES (SYSDATE, 'TRG_MXSPRODUTO', 
                                    SUBSTR(TO_CHAR(ECODE) || ' - ' || EMESG || ' ** ' || DBMS_UTILITY.FORMAT_CALL_STACK, 0, 4000));
                            RAISE;
                    END;
                ]';

                EXECUTE IMMEDIATE q'[
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
                                (CODPRODANT, CODPROD, CODDEPTOANT, CODDEPTO, PRECOANT, PRECO, STATUSANT, STATUS, CODOPERACAOANT, CODOPERACAO, DTATUALIZ)
                            VALUES (NULL, :NEW.CODPROD, NULL, :NEW.CODDEPTO, NULL, :NEW.PRECO, NULL, :NEW.STATUS, NULL, OPER, SYSDATE);
                        ELSIF UPDATING THEN
                            OPER := 1;
                            INSERT INTO MXSPRODUTOLOG
                                (CODPRODANT, CODPROD, CODDEPTOANT, CODDEPTO, PRECOANT, PRECO, STATUSANT, STATUS, CODOPERACAOANT, CODOPERACAO, DTATUALIZ)
                            VALUES (:OLD.CODPROD, :NEW.CODPROD, :OLD.CODDEPTO, :NEW.CODDEPTO, :OLD.PRECO, :NEW.PRECO, :OLD.STATUS, :NEW.STATUS, :OLD.CODOPERACAO, OPER, SYSDATE);
                        ELSIF DELETING THEN
                            OPER := 2;
                            INSERT INTO MXSPRODUTOLOG
                                (CODPRODANT, CODPROD, CODDEPTOANT, CODDEPTO, PRECOANT, PRECO, STATUSANT, STATUS, CODOPERACAOANT, CODOPERACAO, DTATUALIZ)
                            VALUES (:OLD.CODPROD, NULL, :OLD.CODDEPTO, NULL, :OLD.PRECO, NULL, :OLD.STATUS, :NEW.STATUS, :OLD.CODOPERACAO, OPER, SYSDATE);
                        END IF;

                    EXCEPTION
                        WHEN OTHERS THEN
                            ECODE := SQLCODE;
                            EMESG := SQLERRM;
                            INSERT INTO MXSLOGERRO (DATAERRO, NOMETRG, DESCRICAO)
                            VALUES (SYSDATE, 'TRG_MXSPRODUTO_LOG', 
                                    SUBSTR(TO_CHAR(ECODE) || ' - ' || EMESG || ' ** ' || DBMS_UTILITY.FORMAT_CALL_STACK, 0, 4000));
                            RAISE;
                    END;
                ]';

    END CreateTriggers;

    PROCEDURE CreateJobs IS
                ECODE NUMBER;
                EMESG VARCHAR2(4000); 
            BEGIN
                FOR rec IN (SELECT codprod FROM mxsproduto WHERE status = 'I') LOOP
                    BEGIN
                        UPDATE mxsproduto
                        SET codoperacao = 2
                        WHERE codprod = rec.codprod;
                    EXCEPTION
                        WHEN OTHERS THEN
                            ECODE := SQLCODE;
                            EMESG := SQLERRM;

                            INSERT INTO MXSLOGERRO (
                                DATAERRO, 
                                NOMETRG, 
                                DESCRICAO
                            )
                            VALUES (
                                SYSDATE, 
                                'JOB_UPDATE_STATUS_MXSPRODUTO', 
                                SUBSTR(
                                    TO_CHAR(ECODE) || ' - ' || EMESG || ' ** ' || DBMS_UTILITY.FORMAT_CALL_STACK, 
                                    1, 
                                    4000
                                )
                            );
                    END;
                END LOOP;

                COMMIT;
            EXCEPTION
                WHEN OTHERS THEN
                    ECODE := SQLCODE;
                    EMESG := SQLERRM;

                    INSERT INTO MXSLOGERRO (
                        DATAERRO, 
                        NOMETRG, 
                        DESCRICAO
                    )
                    VALUES (
                        SYSDATE, 
                        'JOB_UPDATE_STATUS_MXSPRODUTO_GENERAL', 
                        SUBSTR(
                            TO_CHAR(ECODE) || ' - ' || EMESG || ' ** ' || DBMS_UTILITY.FORMAT_CALL_STACK, 
                            1, 
                            4000
                        )
                    );

                    RAISE;
    END CreateJobs;


    PROCEDURE RunAllSetup IS
    BEGIN
        CreateTables;
        CreateSequences;
        CreateTriggers;
        CreateJobs;
    END RunAllSetup;

END PKG_DB_SETUP;

--Terceiro passo: Executar o pacote PKG_DB_SETUP
GRANT EXECUTE ON PKG_DB_SETUP TO SYSTEM;
GRANT CREATE TABLE TO SYSTEM;
GRANT CREATE SEQUENCE TO SYSTEM;
GRANT CREATE TRIGGER TO SYSTEM;
GRANT CREATE JOB TO SYSTEM;

BEGIN
    PKG_DB_SETUP.RunAllSetup;
END;