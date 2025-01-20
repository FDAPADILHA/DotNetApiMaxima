# E-commerce Máxima (CRUD)

<p align="center">
  <img src="DotNetApiMaxima/DotNetApiMaxima/Frontend/src/assets/img1.png" width="45%" />
  <img src="DotNetApiMaxima/Frontend/src/assets/img2.png" width="45%" />
</p>

## Visão Geral
Este projeto implementa uma tela de cadastro de produtos (CRUD) para o módulo de administração de uma plataforma de e-commerce. Ele também inclui o desenvolvimento de uma API que permite o consumo dos dados dos produtos via integração.

## Tecnologias Utilizadas
- Node.js 18.20.5
- Angular 12.2.18
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
- **Frontend**: Angular 10.9.2 ou versões superiores + Node.js 18.20.5
- **Backend**:
  - Serviço API: .NET Core 8.0 ou versões anteriores.
- **Banco de Dados**: Oracle para persistência de informações.

---

## Diagrama Arquitetural

```plaintext
+-----------------------+          +-----------------+
|       Usuário         |<-------->|     Frontend    |
|     (Navegador/API)   |          | Node.js 18.20.5 |
+-----------------------+          |     +           |
          |                        | Angular 12.2.18 |
          |                        +-----------------+
          |                               |
          |                               |
          |     +-----------------+       |
          +---->|   Serviço API   |-------+
                | (.NET Core 8.0) |
                +-----------------+
                          |
                          |
                +-----------------+
                |   Banco Oracle  |
                +-----------------+

```

## Instalação

### Onde baixar o Visual Studio 2022
Link para Download -> https://visualstudio.microsoft.com/pt-br/downloads/

### Como Instalar e Configurar o ambiente de dados:

Link para Download -> https://www.oracle.com/br/database/technologies/xe-downloads.html

Versão Oracle Database 21c Express Edition for Windows ou versões anteriores

Instalar a IDE SQL Developer:

Link para Download -> https://www.oracle.com/br/database/sqldeveloper/technologies/download/

Oracle IDE 23.1.1.345.2114 ou versões superiores

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
   'C:\oraclexepp\oracle\oradata\XE\crudecmaxima.dbf' 
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

```plaintext
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

-- Usuarios
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Maria Rodrigues', 'renan.lima', 't2g2ct', 'A');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Lúcia Silva', 'lucas.santos', 'swgnvq', 'I');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Ana Alves', 'lúcia.pereira', '9d7sdf', 'A');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Ana Rodrigues', 'carlos.rodrigues', 'ohrnwk', 'I');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Renan Mendes', 'carlos.lima', '4oq9fp', 'I');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Lúcia Pereira', 'julia.rodrigues', 'jsajma', 'I');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('João Lima', 'julia.alves', 'iycxo5', 'A');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Maria Santos', 'lucas.rodrigues', '2cta27', 'A');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('João Mendes', 'marta.santos', 'fiwopf', 'A');
INSERT INTO MXSUSUARIOS (NOME, LOGIN, SENHA, STATUS) VALUES ('Pedro Pereira', 'joão.alves', '7eytkw', 'I');
```

#### 6° Passo Instalação do NODE.JS:
Instalar o Node.js v18.20.5 (versão 23 não suportada pelo Angular)
Link para Download -> https://nodejs.org/pt

Obs: Processo de instação next, next e finish e será instalado a partir de um arquivo que você fez Download e salvou na sua máquina

#### 7° Passo Instalar ou verificar instalação do Angular:
Certifique-se de que o Node.js e o npm estão instalados no sistema.
Para instalar no Windows via comando você pode abrir o Terminal ou Windows Power Shell e escrever o comando:

npm install -g @angular/cli@12

Para conferir se foi instalado:
npm -v

#### 8° Passo: Executar o Backend .NET:
Dentro do Vistual Studio 2022
   Usar o Terminal para realizar o comando:

cd .\DotNetApiMaxima
dotnet run

#### 9° Passo: Executar o Frontend com VS Code:
Open Folder \DotNetApiMaximacd Frontend
npm start

---

## Como Contribuir

Se você deseja contribuir para o projeto, siga as etapas abaixo:

1. Faça um Fork do repositório.
2. Crie uma nova branch (`git checkout -b minha-contribuicao`).
3. Faça as mudanças desejadas e commite (`git commit -am 'Adiciona nova funcionalidade'`).
4. Envie para o repositório remoto (`git push origin minha-contribuicao`).
5. Abra um Pull Request.

**Boas práticas para contribuição:**
- Adicione testes automatizados, caso esteja criando uma nova funcionalidade.
- Certifique-se de que o código esteja bem documentado.
- Mantenha a formatação consistente com o restante do projeto.

## Licença

Este projeto está licenciado sob a Licença MIT - consulte o [LICENSE](LICENSE) para mais detalhes.

---

## Tabela de Conteúdos

1. [Visão Geral](#visão-geral)
2. [Tecnologias Utilizadas](#tecnologias-utilizadas)
3. [Funcionalidades Planejadas](#funcionalidades-planejadas)
4. [Requisitos Técnicos](#requisitos-técnicos)
5. [Instalação](#instalação)
6. [Licença](#licença)
7. [Como Contribuir](#como-contribuir)
