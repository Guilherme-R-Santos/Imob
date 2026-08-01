# Arquitetura Técnica — Imob (MVVM)

## 1. Objetivo deste documento
Este guia descreve como a aplicação está organizada após a migração para MVVM, com foco em:
- responsabilidades de cada camada;
- classes principais;
- fluxo dos módulos;
- como evoluir o sistema com segurança.

---

## 2. Stack e contexto
- **.NET 10 (Windows)**
- **WPF**
- **MVVM** com comandos (`ICommand`)
- **HTTP API** via `HttpClient`
- **Newtonsoft.Json**
- **PDFsharp** (geração de contrato)

---

## 3. Estrutura de pastas (visão funcional)
- `Pages/`
  - `Sistema.xaml`: composição visual principal (bindings + comandos)
  - `Sistema.xaml.cs`: bootstrap de tela, token/session lifecycle e eventos visuais específicos
- `ViewModels/`
  - `SistemaViewModel.cs`: orquestrador principal dos módulos
  - `Commands/`: infraestrutura de comando (`RelayCommand`, `AsyncRelayCommand`, etc.)
- `Services/`
  - `ISistemaListagemService` / `SistemaListagemService`: consultas/listagens
  - `ISistemaCrudService` / `SistemaCrudService`: criação/atualização/inativação
- `Models/DAOs/`
  - modelos de leitura (dados retornados pela API)
- `Models/DTOs/`
  - modelos de escrita (payloads de criação/edição/inativação)
- `Services/Pdf/`
  - geração de PDF de contrato e resolução de fonte
- `Views/`
  - partials históricos de apoio do `Sistema` (legado/compatibilidade)

---

## 4. Arquitetura em camadas

## 4.1 View (`Sistema.xaml`)
Responsável por:
- exibir estado (coleções, campos, visibilidade);
- encaminhar ações de usuário via `Command`;
- não conter regra de negócio.

Exemplos:
- DataGrids ligados a `*View` e coleções do ViewModel;
- modais ligados a `Visibility` no ViewModel;
- botões ligados a `Abrir*Command`, `Salvar*Command`, `Inativar*Command`.

## 4.2 ViewModel (`SistemaViewModel`)
Responsável por:
- estado da UI (campos, listas, seleção, visibilidade);
- regras de validação e fluxo;
- orquestração de chamadas para serviços;
- mensagens para UI via `ShowErrorAction` / `ShowInfoAction`.

Padrões usados:
- `AsyncRelayCommand` para operações I/O;
- `RelayCommand` para operações síncronas leves;
- `INotifyPropertyChanged` para atualização reativa da UI.

## 4.3 Services
Responsáveis por:
- encapsular acesso a API;
- separar leitura/listagem de escrita/CRUD.

### `ISistemaListagemService`
Consultas centrais:
- imóveis, proprietários, locatários, fiadores, contratos;
- fotos por imóvel;
- catálogos auxiliares (intenção, tipo/finalidade de imóvel);
- catálogos de contrato (tipo, modalidade, objeto).

### `ISistemaCrudService`
Persistência central:
- `Cadastrar/Atualizar/Inativar` de imóvel;
- `Cadastrar/Atualizar/Inativar` de contrato;
- `Cadastrar/Atualizar/Inativar` de cliente;
- `Cadastrar/Inativar` fotos.

## 4.4 Models
- **DAO**: projeção de leitura da API para UI/listagem.
- **DTO**: contrato de escrita para endpoints de persistência.

---

## 5. Responsabilidades por módulo

## 5.1 Clientes (Proprietários, Locatários, Fiadores)
No `SistemaViewModel`:
- coleções (`Proprietarios`, `Locatarios`, `Fiadores`);
- filtros de busca (`Search*` + `ICollectionView`);
- comandos de abrir modal, salvar criar/editar, inativar, visualizar;
- preenchimento/limpeza de formulário de cliente.

## 5.2 Imóveis
No `SistemaViewModel`:
- listagem/filtro de imóveis;
- criação (`SalvarImovelCriarAsync`), edição (`SalvarImovelEditarAsync`), inativação;
- carregamento de catálogos para criação;
- controle de modal de imóvel e seleção de item.

## 5.3 Fotos de imóvel
No `SistemaViewModel`:
- `FotosSelecionadasPreview`;
- buffers internos de binário, ids e removidos;
- adicionar imagem (file picker), remover, salvar criar/editar fotos;
- abertura/fechamento de modal de fotos e sincronização com imóvel selecionado.

## 5.4 Contratos
No `SistemaViewModel`:
- listagem/filtro de contratos;
- abertura de modal criar com carga de combos (`AbrirContratoCriarAsync`);
- visualização/edição com carga e preenchimento (`VisualizarContratoAsync`);
- salvar criar (`SalvarContratoCriarAsync`) e salvar edição (`SalvarContratoVisualizarAsync`);
- validações de prazo, vencimento, valor, obrigatórios e contratantes duplicados;
- regras de modalidade (fiador / seguro-fiança).

---

## 6. Papel do `Sistema.xaml.cs` após MVVM
Permanece focado em infraestrutura de tela:
- criação/injeção do `SistemaViewModel`;
- ações de notificação (`ShowErrorAction` / `ShowInfoAction`);
- ciclo de token/autenticação;
- handlers puramente visuais/comportamentais da janela.

O fluxo de negócio principal (CRUD/listagem/validação de módulos) fica no ViewModel.

---

## 7. Fluxo de execução (alto nível)
1. `Sistema` cria `SistemaViewModel` com `SistemaListagemService` e `SistemaCrudService`.
2. `DataContext` é definido.
3. View dispara comandos.
4. ViewModel valida dados e chama serviço.
5. Serviço chama API (DAO/DTO).
6. ViewModel atualiza propriedades/coleções.
7. UI reage automaticamente por binding.

---

## 8. Convenções para evolução do projeto

## 8.1 Ao adicionar nova funcionalidade
1. **Estado**: crie propriedades no ViewModel.
2. **Ação**: crie comando (`AsyncRelayCommand` para I/O).
3. **Regra**: implemente validação no ViewModel.
4. **Integração API**: exponha método em interface de service e implemente.
5. **Binding**: conecte controles XAML às novas propriedades/comandos.
6. **Feedback**: use `NotificarInfo` / `NotificarErro`.

## 8.2 Onde colocar cada tipo de código
- Regra de negócio/validação: **ViewModel**
- Chamada HTTP: **Service + DTO/DAO**
- Aparência/composição: **XAML**
- Comportamento visual pontual de janela: **code-behind**

## 8.3 Checklist de qualidade para incrementos
- [ ] comando sem dependência de click handler;
- [ ] campos do formulário bindados no XAML;
- [ ] nenhuma regra de negócio nova no code-behind;
- [ ] métodos novos declarados na interface e implementados no service;
- [ ] build ok.

---

## 9. Pontos de extensão recomendados
- Extrair `SistemaViewModel` em ViewModels por módulo (ex.: `ContratosViewModel`, `ImoveisViewModel`) para reduzir tamanho.
- Introduzir testes unitários de validação de contrato/imóvel.
- Padronizar tratamento de erros da API com tipos de exceção específicos.
- Evoluir módulos ainda em maturação (ex.: Vistorias) usando o mesmo padrão.

---

## 10. Resumo rápido
- **UI (XAML)**: exibe e dispara comandos.
- **ViewModel**: estado + regra + orquestração.
- **Services**: fronteira HTTP.
- **DAO/DTO**: contratos de dados.

Com isso, o projeto fica previsível para manutenção e expansão incremental sem acoplamento forte ao code-behind.