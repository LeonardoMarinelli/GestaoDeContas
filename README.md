# Bem vindo ao Sistema de Gestão de Contas 👋

Este projeto foi desenvolvido com o objetivo de ser um sistema versátil de controle financeiro pessoal.

Essa aplicação está sendo desenvolvido utilizando ASP.NET com a versão .NET CORE 8, devido a sua facilidade e agilidade para desenvolvimento em curtos períodos de prazo. Nesse material abaixo, há uma explicação geral sobre algumas das tecnologias utilizadas:

[Microsoft Learn](https://learn.microsoft.com/pt-br/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs)

Aproveite e explore todas as funcionalidades. 

## Começando

### Variáveis de Ambiente
Para rodar a aplicação, é necessário criar um arquivo `.env` em `GestaoDeContas/GestaoDeContas`.

Um arquivo de exemplo `.env.example` está disponível no módulo `GestaoDeContas/GestaoDeContas` para ser **copiado e renomeado** para `.env`, com todas as informações necessárias.

Depois, é preciso **mudar o diretório** para conseguir acessar o projeto principal:
```bash
$ cd .\GestaoDeContas\ 
```

### Docker

No ambiente de desenvolvimento, a aplicação vai usar Docker para criar contêineres de banco de dados. 
Isso dá a cada desenvolvedor um ambiente leve e isolado garantindo compatibilidade, ficando mais fácil de replicar o ambiente.

Há um arquivo `docker-compose.yaml` na raiz do projeto que ajuda a criar um contêiner da base de dados [Postgres](https://www.postgresql.org/).

[Instale o Docker](https://docs.docker.com/get-started/get-docker/)

Após ter o Docker configurado em sua máquina, execute:
```bash
$ docker compose up -d
```

### Migrations

Em `GestaoDeContas.Migrations`, o EntityFramework realiza a criação das `Migrations` para atualização do banco de dados.

Com o .NET 8 instalado, baixe a ferramenta EF Core para realizar a criação das tabelas do banco de dados:
```bash
$ dotnet tool install --global dotnet-ef
```

Após ter instalado o `dotnet-ef`, para criar as tabelas, execute:
```bash
$ dotnet ef migrations add InitialCreate
```

Ou para atualizar as tabelas, execute:
```bash
$ dotnet ef database update
```

Após isso sua aplicação estará em execução.

## Estrutura de Modelagem

O projeto foi dividido em componentes que possuem suas respectivas funções. A árvore/estrutura de diretórios é a seguinte:

```
GestaoDeContas/
├─ Controllers/
├─ Data/
├─ Helpers/
├─ Migrations/
├─ Models/
├─ Views/
│  ├─ ContasPagar/
|  ├─ ContasReceber/
|  ├─ Cartao/
|  ├─ ComprasCartao/
|  ├─ Relatorios/
│  ├─ css/
│  ├─ Shared/
```

## Funcionamento

O sistema tem uma abordagem minimalista e confortável, para usuários de qualquer idade. O sistema está dividido em módulos:
- Contas a Pagar;
- Contas a Receber;
- Compras em Cartão de Crédito;
- Relatórios Financeiros;
- Controle de acesso por usuário.

### Afinal, para que serve cada componente da estrutura?

## Controllers

Assim como são comumente utilizados, os `Controllers` são responsáveis por gerenciar as requisições HTTP da aplicação. São como o "intermediário" entre as `Models` e as `Views`.
Cada ação dentro de um `Controller` geralmente corresponde a uma "rota" e, consequentemente, uma requisição HTTP.

[*Microsoft Learn* - Controllers em ASP.NET](https://learn.microsoft.com/pt-br/aspnet/mvc/overview/older-versions-1/controllers-and-routing/aspnet-mvc-controllers-overview-cs)

## Models

Representam, basicamente, a estrutura de dados e a lógica de negócios da aplicação. Define, ainda, a estrutura dos dados e informações que a aplicação vai utilizar e manipular, como se fossem Entidades.
No caso do ASP.NET, as `Models` também são utilizadas para representar o estado de um formulário enviado por uma `view`.

## Views

As `Views` são responsáveis por renderizar o conteúdo HTML da aplicação, que é o que o usuário final verá no navegador. Elas funcionam como uma interface gráfica e é através delas que obtemos os dados para que os `Controllers` processem as informações (nesse caso, através dos `Models`).
Nessa aplicação, foi utilizado bastante do Razor, que é um motor de *template* do ASP.NET. Ele faz a vinculação dos campos preenchidos a sua respectiva `Model`, gerando assim o HTML final.

## Migrations

São utilizadas para gerenciar automaticamente mudanças no esquema do banco de dados. Sempre que mudamos alguma coisa em algum modelo de dados (adicionar um campo por exemplo) geramos uma `Migration` para aplicar essa mudança, também, no banco de dados.
Para essa aplicação, foi utilizado do *Entity Framework Core* para lidar com as `Migrations`.

## Usuários (Identity)

Para toda a parte de usuários, foi utilizado o sistema do `ASP.NET Identity`, que é responsável por gerenciar de forma prática usuários, senhas e permissões. Ele é uma boa alternativa, já que criptografa as senhas e salva muitas informações valiosas no banco de dados.
Para saber mais, há um artigo no *Microsoft Learn* que introduz bem sobre os conceitos: 

[*Microsoft Learn* - Identity](https://learn.microsoft.com/pt-br/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio)

