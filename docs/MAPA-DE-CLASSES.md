# Mapa de Classes — Imob

## Objetivo
Este documento é um índice técnico rápido para localizar classes, entender responsabilidades e facilitar evolução da arquitetura.

---

## 1) Camada de UI (WPF)

| Classe/Arquivo | Tipo | Responsabilidade principal | Dependências diretas |
|---|---|---|---|
| `App` (`App.xaml.cs`) | Bootstrap WPF | Inicialização da aplicação | WPF runtime |
| `MainWindow` (`Pages/MainWindow.xaml.cs`) | View + code-behind | Login, autenticação inicial e abertura do sistema | `UsuarioDAO`, `HttpClient` |
| `Sistema` (`Pages/Sistema.xaml.cs`) | View shell + infraestrutura | Cria `SistemaViewModel`, configura `DataContext`, ciclo de token e eventos visuais da janela | `SistemaViewModel`, `SistemaListagemService`, `SistemaCrudService` |
| `Sistema` partial (`Views/ContratosView.cs`) | Partial legado/apoio | Rotinas históricas de contratos mantidas para compatibilidade visual | Controles XAML de contrato, DAOs/DTOs |
| `Sistema` partial (`Views/ImoveisView.cs`) | Partial legado/apoio | Rotinas históricas de imóveis mantidas para compatibilidade visual | Controles XAML de imóvel, DAOs/DTOs |

> Observação: o fluxo principal de negócio foi migrado para o ViewModel; partials em `Views/` são superfície residual/compatibilidade.

---

## 2) ViewModel e infraestrutura de comandos

| Classe/Arquivo | Tipo | Responsabilidade principal | Dependências diretas |
|---|---|---|---|
| `SistemaViewModel` (`ViewModels/SistemaViewModel.cs`) | ViewModel principal | Estado da UI, comandos, validações, regras de fluxo, orquestração de serviços para Clientes/Imóveis/Fotos/Contratos | `ISistemaListagemService`, `ISistemaCrudService`, DAOs/DTOs, `GeradorContratoPdf` |
| `RelayCommand` | Infra comando | Comando síncrono sem parâmetro | `ICommand` |
| `RelayCommandWithParameter` | Infra comando | Comando síncrono com parâmetro | `ICommand` |
| `AsyncRelayCommand` | Infra comando | Comando assíncrono sem parâmetro, controle de execução e captura de exceção | `ICommand`, `Task` |
| `AsyncRelayCommandWithParameter` | Infra comando | Comando assíncrono com parâmetro, controle de execução e captura de exceção | `ICommand`, `Task` |

### `SistemaViewModel` — áreas funcionais
- **Navegação/painéis:** visibilidade dos módulos (Proprietários, Locatários, Fiadores, Imóveis, Contratos, Vistorias).
- **Listagens + filtros:** `ObservableCollection` + `ICollectionView` + `Search*`.
- **Clientes:** criar/editar/inativar/visualizar.
- **Imóveis:** criar/editar/inativar/visualizar + carregamento de catálogos.
- **Fotos:** seleção, preview, remoção, persistência criar/editar.
- **Contratos:** abrir modal com combos, visualizar com preenchimento, salvar criar/editar, inativar, gerar PDF.

---

## 3) Serviços de aplicação

| Classe/Arquivo | Tipo | Responsabilidade principal | Métodos chave |
|---|---|---|---|
| `ISistemaListagemService` | Interface | Contrato de consultas/listagens para ViewModel | `ObterImoveisAsync`, `ObterContratosAsync`, `ObterFotosPorImovelAsync`, `ObterTiposContratoAsync`, etc. |
| `SistemaListagemService` | Implementação | Implementa consultas HTTP para dados de leitura/catálogos | Chamadas DAO `Get*` |
| `ISistemaCrudService` | Interface | Contrato de persistência (create/update/inactivate) | `Cadastrar/Atualizar/Inativar` de `Imovel`, `Contrato`, `Cliente`, `Foto` |
| `SistemaCrudService` | Implementação | Implementa persistência HTTP via DTOs | Métodos CRUD assíncronos |

---

## 4) Modelos de leitura (DAOs)

### Núcleo de domínio
| Classe | Responsabilidade |
|---|---|
| `ClienteDAO` | Representa cliente retornado pela API (proprietário/locatário/fiador) |
| `ImovelDAO` | Representa imóvel retornado pela API |
| `ContratoDAO` | Representa contrato retornado pela API |
| `FotoDAO` | Representa foto associada a imóvel |
| `UsuarioDAO` | Representa usuário autenticado/sessão |
| `VistoriaDAO` | Estrutura de vistoria (módulo em evolução) |

### Catálogos
| Classe | Responsabilidade |
|---|---|
| `TipoClienteDAO` | Catálogo de tipos de cliente |
| `TipoImovelDAO` | Catálogo de tipos de imóvel |
| `IntencaoDAO` | Catálogo de intenção do imóvel |
| `FinalidadeDAO` | Catálogo de finalidade do imóvel |
| `TipoContratoDAO` | Catálogo de tipos de contrato |
| `ModalidadeContratoDAO` | Catálogo de modalidades de contrato |
| `ObjetoContratoDAO` | Catálogo de objetos de contrato |
| `TipoFotoDAO` | Catálogo de tipo de foto |
| `TipoUsuarioDAO` | Catálogo de tipo de usuário |

Padrão comum nos DAOs: métodos estáticos de consulta `Get*` consumindo API e desserialização JSON.

---

## 5) Modelos de escrita (DTOs)

| Classe | Responsabilidade | Operações principais |
|---|---|---|
| `ClienteDTO` | Payload para persistência de clientes | `CadastrarCliente`, `AtualizarCliente`, `InativarCliente` |
| `ImovelDTO` | Payload para persistência de imóveis | `CadastrarImovel`, `AtualizarImovel`, `InativarImovel` |
| `ContratoDTO` | Payload para persistência de contratos | `CadastrarContrato`, `AtualizarContrato`, `InativarContrato` |
| `FotoDTO` | Payload para persistência de fotos | `CadastrarFoto`, `InativarFoto` |

---

## 6) PDF e documentos

| Classe/Arquivo | Responsabilidade |
|---|---|
| `GeradorContratoPdf` (`Services/Pdf`) | Gera contrato em PDF |
| `WindowsFontResolver` (`Services/Pdf`) | Resolve fontes para renderização correta no Windows |
| `ContratoLocacaoPdfModel` (`Models/Documentos/Contratos`) | Modelo de apoio para composição de dados do PDF |

---

## 7) Dependências entre camadas (resumo)

1. `Pages/*` -> interage com `SistemaViewModel` via binding/commands.
2. `SistemaViewModel` -> consome `ISistemaListagemService` e `ISistemaCrudService`.
3. `Services` -> consomem `DAOs/DTOs` e `HttpClient`.
4. `DTOs/DAOs` -> realizam serialização/desserialização e chamadas à API.

Regra arquitetural prática:
- **UI não chama API diretamente para novas funcionalidades**;
- **novos fluxos devem entrar via ViewModel + Service**.

---

## 8) Guia rápido para encontrar onde alterar

- **Novo campo de formulário**: `SistemaViewModel` (propriedade) + `Sistema.xaml` (binding).
- **Nova ação de botão**: `SistemaViewModel` (command + método).
- **Nova regra de validação**: `SistemaViewModel`.
- **Novo endpoint de consulta**: `ISistemaListagemService` + `SistemaListagemService` + DAO.
- **Novo endpoint de persistência**: `ISistemaCrudService` + `SistemaCrudService` + DTO.
- **Ajuste visual puro**: `Sistema.xaml`.

---

## 9) Arquivos mais relevantes para manutenção
- `ViewModels/SistemaViewModel.cs`
- `Pages/Sistema.xaml`
- `Services/ISistemaListagemService.cs`
- `Services/SistemaListagemService.cs`
- `Services/ISistemaCrudService.cs`
- `Services/SistemaCrudService.cs`
- `Models/DAOs/*`
- `Models/DTOs/*`
- `docs/ARQUITETURA-MVVM.md`
