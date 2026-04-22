# 🏠 Imob

<p align="center">
  <img alt="Status" src="https://img.shields.io/badge/status-em%20desenvolvimento-orange" />
  <img alt=".NET" src="https://img.shields.io/badge/.NET-10-512BD4" />
  <img alt="WPF" src="https://img.shields.io/badge/UI-WPF-0C54C2" />
  <img alt="C#" src="https://img.shields.io/badge/C%23-14.0-239120" />
  <img alt="Plataforma" src="https://img.shields.io/badge/Windows-Desktop-0078D6" />
</p>

Sistema desktop para gestão imobiliária, focado em **cadastro e administração de imóveis, clientes e contratos**, com interface moderna em **WPF** e integração com API HTTP.

---

## ✨ Visão geral do projeto

O `Imob` é um cliente desktop que centraliza rotinas comuns de imobiliária:

- autenticação de usuário;
- listagem e manutenção de entidades principais;
- inativação lógica de registros;
- gestão de fotos de imóveis (upload e remoção);
- montagem e atualização de contratos de locação;
- geração inicial de contrato em PDF.

> ⚠️ **Importante:** o projeto está em **desenvolvimento ativo**. Há fluxos já funcionais e outros ainda em evolução/refino.

---

## 🧩 Funcionalidades implementadas

### 🔐 Autenticação
- Tela de login (`MainWindow`) com validação via API.
- Recebimento de JWT e controle de expiração.
- Renovação automática de token durante a sessão na tela principal (`Sistema`).

### 👥 Gestão de pessoas
- Módulos separados para:
  - `Proprietários`
  - `Locatários`
  - `Fiadores`
- Ações disponíveis:
  - listar
  - pesquisar
  - cadastrar
  - visualizar/editar
  - inativar

### 🏘️ Gestão de imóveis
- Listagem de imóveis com busca.
- Cadastro completo com:
  - endereço
  - intenção
  - tipo de imóvel
  - dados financeiros (valor, IPTU, condomínio etc.)
- Edição de imóvel.
- Inativação de imóvel.

### 🖼️ Gestão de fotos
- Upload por seletor de arquivos.
- Upload por `drag and drop`.
- Pré-visualização em grade.
- Remoção de fotos selecionadas.
- Persistência das novas fotos e inativação de fotos removidas.

### 📄 Gestão de contratos
- Listagem e busca de contratos.
- Cadastro de contrato com:
  - tipo
  - modalidade
  - objeto
  - proprietário
  - imóvel
  - até 4 contratantes
  - fiador (quando aplicável)
  - dados de vigência e vencimento
  - dados de seguro-fiança (quando aplicável)
- Regras de interface para habilitar/desabilitar campos conforme modalidade.
- Edição de contrato existente.
- Inativação de contrato.

### 🧾 PDF
- Geração inicial de contrato em PDF via `PDFsharp`.
- Configuração de fontes Windows para renderização correta (`WindowsFontResolver`).

---

## 🏗️ Arquitetura e estrutura

Projeto organizado em camadas simples:

- `Pages/`
  - Telas WPF (`MainWindow`, `Sistema`) e code-behind com regras de UI.
- `Models/DAOs/`
  - Modelos de leitura/consulta de entidades recebidas da API.
- `Models/DTOs/`
  - Objetos de envio para criação/atualização/inativação via API.
- `Services/Pdf/`
  - Serviços relacionados à geração de PDF e resolução de fontes.

### Entidades centrais
- `Imovel`
- `Cliente`
- `Contrato`
- `Foto`
- Catálogos auxiliares (`TipoCliente`, `TipoImovel`, `TipoContrato`, `ModalidadeContrato`, `ObjetoContrato`, `Intencao`)

---

## 🔄 Fluxo funcional resumido

1. Usuário faz login.
2. Sistema valida conexão com API.
3. Após autenticação, abre a tela principal com menu lateral.
4. Usuário navega entre módulos (Pessoas, Imóveis, Contratos, Vistorias).
5. Operações de CRUD/inativação são executadas via endpoints HTTP.
6. Em contratos, é possível gerar PDF de contrato.

---

## 🌐 Dependências e integração

### Tecnologias
- `.NET 10` (Windows)
- `WPF`
- `Newtonsoft.Json`
- `PDFsharp`
- `HttpClient` para comunicação com API

### API esperada
O cliente está configurado para consumir API em:

`https://localhost:7251/`

Exemplos de rotas utilizadas:
- `Usuario/Login`
- `Usuario/ObterPorLogin/{login}`
- `Cliente/*`
- `Imovel/*`
- `Contrato/*`
- `Foto/*`

---

## 🚧 Status atual de desenvolvimento

O projeto **não está finalizado**. Pontos observados no estado atual:

- funcionalidades essenciais de cadastro/listagem já implementadas;
- módulo de vistoria ainda sem fluxo completo de CRUD;
- recuperação de senha/novo acesso ainda marcados para implementação;
- geração de PDF de contrato ainda em formato base (template inicial);
- melhorias de robustez e padronização arquitetural ainda podem evoluir.

---

## ▶️ Como executar

### Pré-requisitos
- Windows
- SDK .NET 10 instalado
- API backend do ecossistema `Imob` em execução local

### Passos
1. Clone o repositório.
2. Abra a solução no Visual Studio.
3. Garanta que a API esteja ativa em `https://localhost:7251/`.
4. Execute o projeto WPF (`Imob`).
