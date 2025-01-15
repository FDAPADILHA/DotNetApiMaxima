# E-commerce Máxima (CRUD)

https://via.placeholder.com/900x300.png?text=Cadastro+de+Produtos+CRUD

## Visão Geral
Este projeto implementa uma tela de cadastro de produtos (CRUD) para o módulo de administração de uma plataforma de e-commerce. Ele também inclui o desenvolvimento de uma API que permite o consumo dos dados dos produtos via integração.

## Tecnologias Utilizadas
- Angular 8.0
- .NET 8.0
- Banco de Dados Oracle

### Pacotes utilizados:

- Oracle.EntityFrameworkCore 9.23.60
- Swashbuckle.AspNetCore.Swagger 7.2.0
- Swashbuckle.AspNetCore.SwaggerGen 7.2.0
- Swashbuckle.AspNetCore.SwaggerUI 7.2.0
- Microsoft.EntityFrameworkC 9.0.0

## Funcionalidades Planejadas
- Gestão de Produtos (CRUD)
- Integração com serviços RESTful
- Autenticação e autorização
- Otimização para escalabilidade futura

---

## Requisitos Técnicos
- **Frontend**: Angular 8.0
- **Backend**: Dois microsserviços:
  - Web Service: Angular 8.0
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
|      Angular 8.0    |          |   Angular 8.0   |
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

# Documentação da API

## Descrição
Esta API permite gerenciar produtos, incluindo funcionalidades de adicionar, listar, consultar e excluir produtos. Este documento descreve os endpoints disponíveis para consumo.

---

## Endpoints Disponíveis


Se houver dúvidas, entre em contato com o suporte da API.



## Instalação

###Onde baixar o Visual Studio 2022
Link para Download -> https://visualstudio.microsoft.com/pt-br/downloads/

###Como Instalar e Configurar o ambiente de dados:

Link para Download -> https://www.oracle.com/br/database/technologies/xe-downloads.html

Versão Oracle Database 21c Express Edition for Windows ou versões anteriores

Instalar a IDE SQL Developer:

Link para Download -> https://www.oracle.com/br/database/sqldeveloper/technologies/download/

Oracle IDE 23.1.1.345.2114 ou versões acima

Recomendo instalar com a JDK incluída: Windows 64-bit with JDK 17 included

Durante a instalação atente-se ao criar o usuário como "SYSTEM" e de guardar a senha que você definiu porque isso será importante para os próximos passos de criação do ambiente do banco.

#### 1° Passo: Realizar o git clone ou download do zip do projeto: 
--Crie um diretório no seu computador para guardar o projeto e utilizar como caminho de execução no Visual Studio 2022
--Comando para Clonar repositório em git
git clone https://github.com/FDAPADILHA/DotNetApiMaxima.git

#### 2° Passo: Abrir o SQL Developer e acessar o banco com usuário SYSTEM

#### 3° Passo: Criar o TS
Criando o Table Space: 

create tablespace crudecmaxima
 datafile
   'C:\oraclexe\app\oracle\oradata\XE\crudecmaxima.dbf' 
		size 100m autoextend on next 50m maxsize 500m
   online
   permanent
   extent management local autoallocate
   segment space management auto;

#### 4° Passo: Abrir o Visual Studio 2022 e selecionar o diretório do projeto. Você deverá ver essa estrutura:
```plaintext
.
├── Config/                 # Configurações e mapeamentos do Entity Framework
│   ├── Contexto.cs         # Contexto do banco de dados
│   ├── DepartamentoMapping.cs  # Mapeamento da entidade Departamento
│   └── ProdutoMapping.cs       # Mapeamento da entidade Produto
├── Controllers/            # Controladores da API
│   └── ProdutoController.cs    # Controlador para produtos
├── DataBaseSetup/          # Scripts relacionados ao banco de dados
│   └── create_pkg.sql          # Script para configuração do banco
├── Models/                 # Modelos de domínio da aplicação
│   ├── Departamento.cs         # Entidade Departamento
│   └── Produto.cs              # Entidade Produto
├── Pages/                  # Páginas do Razor (se aplicável)
├── Properties/             # Configurações do projeto
├── wwwroot/                # Arquivos estáticos da aplicação
├── appsettings.json        # Configurações da aplicação
└── Program.cs              # Configuração e inicialização da aplicação
```

Se não estiver vendo essa estrutura, então dê um duplo clique em "DotNetApiMaxima.sln";

#### 5° Passo: Criar as tabelas, triggers e Jobs do banco de dados Oracle:

No Visual Studio 2022 acesse a pasta 
```plaintext
├── DataBaseSetup/          # Scripts relacionados ao banco de dados
│   └── create_pkg.sql          # Script para configuração do banco
```

Dentro do arquivo "create_pkg.sql" consta outro passo a passo para executar a PKG de criação do ambiente.

#### 5° Passo: Popular os dados nas tabelas:

-- Departamentos
INSERT INTO MXSDEPARTAMENTO (IDDEPTO, CODDEPTO, DESCRICAO, STATUS) VALUES (NULL, 1, 'MOUSES', 'A');
INSERT INTO MXSDEPARTAMENTO (IDDEPTO, CODDEPTO, DESCRICAO, STATUS) VALUES (NULL, 2, 'TECLADOS', 'A');
INSERT INTO MXSDEPARTAMENTO (IDDEPTO, CODDEPTO, DESCRICAO, STATUS) VALUES (NULL, 3, 'MONITORES', 'A');
INSERT INTO MXSDEPARTAMENTO (IDDEPTO, CODDEPTO, DESCRICAO, STATUS) VALUES (NULL, 4, 'HEADSETS', 'A');
INSERT INTO MXSDEPARTAMENTO (IDDEPTO, CODDEPTO, DESCRICAO, STATUS) VALUES (NULL, 5, 'CADEIRAS GAMER', 'A');

-- Produtos
INSERT INTO MXSPRODUTO (IDPROD, CODPROD, DESCRICAO, CODDEPTO, PRECO, STATUS, CODOPERACAO) VALUES (11, 'TECL-GAMER11', 'TECLADO MECANICO AZUL', 2, 250.50, 'A', 0);
INSERT INTO MXSPRODUTO (IDPROD, CODPROD, DESCRICAO, CODDEPTO, PRECO, STATUS, CODOPERACAO) VALUES (12, 'MON-ULTRAWIDE12', 'MONITOR ULTRAWIDE 29"', 3, 1299.99, 'I', 0);
INSERT INTO MXSPRODUTO (IDPROD, CODPROD, DESCRICAO, CODDEPTO, PRECO, STATUS, CODOPERACAO) VALUES (13, 'HEAD-GAMER13', 'HEADSET 7.1 SURROUND', 4, 320.00, 'A', 0);
INSERT INTO MXSPRODUTO (IDPROD, CODPROD, DESCRICAO, CODDEPTO, PRECO, STATUS, CODOPERACAO) VALUES (14, 'CADE-GAMER14', 'CADEIRA ERGONOMICA PRETA', 5, 850.99, 'A', NULL);

   
