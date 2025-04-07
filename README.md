# Sistema de Tickets

Este é um projeto de gerenciamento de tickets para uma empresa. O sistema permite que os usuários registrem, visualizem e atualizem tickets associados a empregados. A aplicação foi construída utilizando o ASP.NET Core MVC.

## Como Configurar o Projeto

## Índice

- [Pré-requisitos](#pré-requisitos)
- [Como Configurar o Projeto](#como-configurar-o-projeto)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Banco de Dados](#banco-de-dados)
- [Testes Unitários](#testes-unitários)
- [Swagger](#swagger)

### Pré-requisitos

- **.NET SDK**: Certifique-se de ter o .NET SDK instalado. Você pode baixá-lo em: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

### Passos para Configuração

1. Clone este repositório em sua máquina local:
   
bash
   git clone https://github.com/MatheusBorsa/ticket-project-csharp.git

2. Abra com a IDE VisualStudio ou pode iniciar o sistema pelo terminal:
   
bash
    dotnet run

3. Caso tenha iniciado o sistema pelo terminal, abra o navegador e vá até a URL http://localhost:5247


## Tecnologias Utilizadas

- **ASP.NET Core MVC**: Framework para desenvolvimento da aplicação web.
- **In-Memory Database**: Banco de dados em memória para testes e desenvolvimento rápido.
- **Swagger**: Interface para documentação e testes das APIs.

## Banco de Dados
- **Tabelas**: O sistema cria automaticamente as tabelas necessárias.
- **Persistencia**: Como é um banco de dados que armazena na memoria ram, após parar de rodar o sistema todos os dados registrados serão apagados.
## Testes Unitários
- Pode se rodar os testes unitários pelo terminal utilizando:
  
bash
  dotnet test


## Swagger
- Acessando a url http://localhost:5247/swagger você pode encontrar todos os dois controllers e seus métodos e testá-los
