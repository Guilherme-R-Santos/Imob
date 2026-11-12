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
  - até três e-mails de testemunhas para assinatura digital
  - configuração de repasse para até dois destinatários, disponível a administradores
- Regras de interface para habilitar/desabilitar campos conforme modalidade.
- Edição de contrato existente; a API recria as cobranças ativas não pagas ao atualizar o contrato.
- Inativação de contrato.

### 💸 Repasses Pix e TED
- A API autoriza a configuração e as operações de repasse somente para administradores. O cliente também verifica a permissão e mostra erros de acesso negado; se a verificação estiver indisponível, reabra o formulário antes de salvar.
- Em contratos e cobranças, **Configurar repasses** permite até dois destinatários `PIX_CHAVE`, `PIX_CONTA` ou `TED`. O primeiro pode ser pré-preenchido com dados cadastrados do proprietário; complete e revise os campos, inclusive tipo da chave, dígito e tipo da conta. A configuração do contrato é copiada para as cobranças geradas.
- Valores `FIXO` são em reais (50 = R$ 50,00; até duas casas decimais). `PERCENTUAL` usa o valor líquido (50 = 50%; até quatro casas); a soma dos percentuais não pode exceder 100%. A API arredonda os percentuais para centavos e não permite que o total fixo e percentual ultrapasse o líquido.
- O repasse automático requer a opção habilitada e pelo menos um destinatário válido; é iniciado após o processamento do webhook `PAYMENT_RECEIVED` com valor líquido. O manual exige cobrança ativa e recebida no Asaas e webhook processado; permite revisar até dois destinatários antes da confirmação. Há uma solicitação por cobrança Asaas. O retorno 202 significa apenas registro para processamento, não conclusão.
- **Acompanhar repasses** consulta registros locais por cobrança. Cada destinatário é processado separadamente; somente `DONE` indica conclusão. `INCERTO` ou `ENVIANDO` interrompido requer investigação no Asaas e conciliação com o ID da transferência. `BLOQUEADO` local só pode ser retomado após confirmar que não houve envio e corrigir a causa. Estornos, contestações ou problemas em repasse anterior podem impedir o envio. Nunca reenvie automaticamente após falha ou timeout.
- Nesta fase, os repasses operam somente em sandbox e dependem de webhook configurado, migrações aplicadas e `Asaas:RepassesHabilitados=true` na API. Respostas 503 indicam indisponibilidade, inclusive ambiente não sandbox ou funcionalidade desabilitada. Credenciais Asaas permanecem exclusivamente no backend; consulte o README da ImobAPI para os requisitos de implantação.

### 🧾 PDF e assinatura
- Geração inicial de contrato em PDF via `PDFsharp`.
- Configuração de fontes Windows para renderização correta (`WindowsFontResolver`).
- Na visualização do contrato, **Enviar para assinatura** gera o PDF a partir dos dados salvos e envia à rota `Autentique/CriarDocumento`. Salve alterações antes de enviar.
- Proprietário, contratantes e fiadores do contrato são convidados por e-mail. As até três testemunhas cadastradas são incluídas automaticamente pela API; seus e-mails devem ser válidos e distintos dos demais signatários.
- A API limita o PDF a 20 MiB; o limite da Autentique depende do plano (5 MB no gratuito e 20 MB no profissional). O sandbox e a chave são configurados exclusivamente na API.
- A consulta de registros mostra dados salvos na API, não o status atual na Autentique; não há webhook de atualização em tempo real.
- A migração da integração deve ser aplicada no banco da ImobAPI antes do uso. Após isso, é necessário validar o fluxo em sandbox real.
- Em caso de falha ou tempo limite, verifique o painel da Autentique e os logs da API antes de reenviar: o documento pode ter sido criado e o contrato pode estar bloqueado para conciliação.

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
- `Autentique/CriarDocumento`
- `Autentique/DocumentosDoContrato/{contratoId}`

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
