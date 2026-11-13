# GaroAvisa API

API em .NET do **GaroAvisa**, solução CitizenTech da Global Solution 2026/2.

## Índice

- [Sobre o projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Estrutura do projeto](#estrutura-do-projeto)
- [Como executar](#como-executar)
- [Requisitos da Global Solution](#requisitos-da-global-solution)
- [Perfis de acesso](#perfis-de-acesso)
- [Endpoints](#endpoints)
- [Fluxo de teste no Swagger](#fluxo-de-teste-no-swagger)
- [Testes automatizados](#testes-automatizados)
- [Tabelas do banco](#tabelas-do-banco)
- [Integrantes](#integrantes)

---

## Sobre o projeto

### O problema

O tempo interfere diretamente na rotina das pessoas: uma chuva forte no caminho do trabalho, uma onda de calor no dia de uma consulta, um temporal na hora de buscar os filhos na escola. As previsões costumam ser genéricas, para a cidade inteira, e mudam ao longo do dia. A pessoa precisa procurar a informação, interpretar números e códigos e lembrar de conferir de novo.

### A solução

O **GaroAvisa** faz esse trabalho pelo usuário. Ele cadastra os lugares que fazem parte da rotina dele e os compromissos que tem pela frente, e a API acompanha a previsão de cada um, explica o tempo em português e avisa quando existe risco ou quando a previsão muda.

### O que a API faz

- **Conta e acesso seguro:** cadastro e login com token JWT. A senha é guardada protegida por hash, nunca em texto, e cada usuário só vê os próprios dados. O usuário também pode excluir a própria conta, o que apaga todos os dados dele. Contas de administrador não podem ser excluídas pela API, para o sistema nunca ficar sem administrador.
- **Busca de lugares:** o usuário digita o nome de um bairro ou cidade e a API busca na Open-Meteo as coordenadas e o fuso horário. A busca funciona para qualquer lugar do mundo.
- **Meus locais:** o usuário salva seus lugares com uma **etiqueta** (Casa, Trabalho, Escola), escolhe um **local principal** e decide em quais locais quer receber alertas.
- **Previsão explicada:** previsão de 1 a 7 dias para cada local, com o código do tempo traduzido para o português (por exemplo, "Chuva forte" ou "Trovoada"), temperaturas máxima e mínima, chuva em milímetros, chance de chuva e rajada de vento.
- **Regras de alerta:** o administrador define quando um alerta deve ser criado, com cinco tipos de risco (chuva forte, vento forte, calor, frio e tempestade), um limite e uma gravidade (baixa, média ou alta).
- **Alertas automáticos:** a verificação analisa a previsão de hoje e de amanhã de todos os locais, compara com as regras e cria os alertas, sem duplicar. Quando mais de uma regra do mesmo tipo dispara, vale a mais grave. O usuário pode listar os alertas, marcar como lidos e excluir.
- **Aviso de mudança na previsão:** a API guarda a última previsão de cada local e avisa quando ela muda de forma brusca, como 10 mm a mais ou a menos de chuva, 5 °C de diferença na máxima ou um dia sem chuva que passou a ter previsão de garoa ou chuva.
- **Compromissos:** o usuário cadastra um compromisso em um dos seus locais e consulta a previsão e os alertas do dia dele. A previsão fica disponível a partir de 7 dias antes.
- **Resumo do dia:** em uma só consulta, a previsão e os alertas de hoje de todos os locais do usuário.
- **Preferências:** o usuário escolhe se quer alertas do dia, avisos de mudança na previsão, avisos sobre compromissos, resumo diário (com horário) e silêncio noturno. Os alertas do dia e os avisos de mudança respeitam essas escolhas.

### Exemplo de uso

Para entender a ideia na prática, imagine a Ana:

1. A Ana cria a conta dela e busca os lugares da rotina: o bairro onde mora e o bairro onde trabalha. Ela salva os dois com as etiquetas **Casa** e **Trabalho** e marca a Casa como local principal.
2. Ela confere a **previsão** dos próximos dias de cada local, já explicada em português: "Pancadas de chuva fortes, máxima de 24 °C, 80% de chance de chuva".
3. Ela cadastra um **compromisso**: uma consulta médica na sexta-feira, perto do trabalho. Na semana da consulta, ela consulta a previsão daquele dia e vê se há algum alerta.
4. O administrador do GaroAvisa mantém as **regras de alerta**, por exemplo: "chuva acima de 15 mm é risco alto, com a orientação *Evite áreas de alagamento*".
5. Na **verificação**, a API vê que a previsão de amanhã para o Trabalho é de 22 mm de chuva e cria um alerta **"Chuva forte prevista"** para a Ana, com a orientação da regra. Cada alerta traz, além do risco, uma dica do que fazer.
6. No dia seguinte, a previsão muda e a chuva sobe para 35 mm. A API percebe a mudança e cria um aviso **"Previsão mudou"**, explicando o que mudou.
7. De manhã, a Ana abre o **resumo do dia** e vê, em uma só tela, o tempo de hoje e os alertas da Casa e do Trabalho. Ela sai mais cedo e leva guarda-chuva.

Se a Ana não quiser receber avisos de mudança na previsão, ela desliga essa opção nas **preferências**, e a API para de criar esse tipo de aviso para ela.

### Relação com o tema da Global Solution

A Global Solution 2026/2 tem como tema as **CitizenTechs**: tecnologias criadas a partir das necessidades reais das pessoas, que geram um benefício concreto no dia a dia. Entre as áreas sugeridas está **"Clima e emergências: previsão de enchentes, deslizamentos e ondas de calor, com alertas à população"**, e é exatamente aí que o GaroAvisa atua. A API transforma dados públicos e gratuitos de previsão do tempo em informação personalizada: em vez de uma previsão genérica, o usuário recebe alertas sobre os lugares onde ele realmente vai estar e sobre os compromissos que ele realmente tem. Isso o ajuda a se antecipar a chuvas fortes, ventanias e extremos de temperatura e a tomar decisões mais seguras, como sair mais cedo, levar guarda-chuva ou remarcar um compromisso. A solução também segue os critérios de ser **simples e segura**: as explicações são em português claro, os dados de cada usuário ficam protegidos e, se o serviço de previsão ficar fora do ar, o restante da API continua funcionando e a resposta avisa o usuário com uma mensagem clara. O projeto se conecta aos Objetivos de Desenvolvimento Sustentável da ONU **11 (Cidades e comunidades sustentáveis)** e **13 (Ação contra a mudança global do clima)**, ao ajudar as pessoas a se adaptarem aos eventos climáticos do dia a dia.

---

## Tecnologias

| Tecnologia | Uso no projeto |
|---|---|
| .NET 10 / ASP.NET Core Web API | API REST com Controllers |
| Entity Framework Core 10 + Oracle | Acesso ao banco Oracle da FIAP e Migrations |
| JWT Bearer | Autenticação e autorização por perfil (USUARIO e ADMIN) |
| Swagger (Swashbuckle) e Scalar | Documentação interativa (OpenAPI) |
| Rate Limiting do ASP.NET Core | Limite de requisições com resposta 429 |
| Health Checks | Monitoramento da API, do banco e da Open-Meteo |
| HttpClient | Consumo da API pública Open-Meteo (busca de lugares e previsão) |
| xUnit, Moq e WebApplicationFactory | Testes unitários e de integração |

---

## Estrutura do projeto

O projeto é dividido em camadas, cada uma com uma responsabilidade:

```
gs-dotnet/
├── GaroAvisa.Domain           → Entidades e interfaces dos repositórios
├── GaroAvisa.Application      → DTOs, Services (regras de negócio) e Helpers
├── GaroAvisa.Infrastructure   → AppDbContext, Repositories, Migrations, JWT, Open-Meteo e Health Check
├── GaroAvisa.Api              → Controllers, Program.cs, filtro de idempotência, rate limiting e tratamento de erros
├── GaroAvisa.UnitTests        → Testes unitários (Services, Helpers e Controller)
└── GaroAvisa.IntegrationTests → Testes de integração dos endpoints
```

| Camada | O que faz |
|---|---|
| **Domain** | Representa o negócio: `Usuario`, `Local`, `Preferencia`, `RegraAlerta`, `Alerta`, `PrevisaoSalva`, `Compromisso` e `RegistroIdempotencia`. Não depende de nenhuma outra camada. |
| **Application** | Contém as regras: etiqueta de local repetida, data de compromisso no passado, escolha da regra de alerta mais grave, detecção de mudança brusca na previsão etc. |
| **Infrastructure** | Fala com o mundo de fora: banco Oracle (EF Core), Open-Meteo (HttpClient) e geração do token JWT. |
| **Api** | Recebe as requisições HTTP, valida a entrada, aplica autenticação, rate limiting e idempotência, e devolve os códigos HTTP corretos. |

---

## Como executar

### Pré-requisitos

- .NET SDK 10
- Acesso ao banco Oracle da FIAP
- Ferramenta do EF Core: `dotnet tool install --global dotnet-ef`

### 1. Clonar o repositório

```bash
git clone https://github.com/lianalyumi/dotnet-gs2.git
cd dotnet-gs2
```

### 2. Configurar a conexão com o banco

As credenciais **não** ficam no repositório. Copie o arquivo de exemplo:

```bash
cp GaroAvisa.Api/appsettings.Development.exemplo.json GaroAvisa.Api/appsettings.Development.json
```

Abra o `appsettings.Development.json` e preencha com os seus dados:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_RM;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL;"
  },
  "Jwt": {
    "Key": "UMA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES"
  }
}
```

O `appsettings.Development.json` está no `.gitignore`, então a sua senha nunca vai para o GitHub.

### 3. Criar as tabelas no banco

```bash
dotnet ef database update --project GaroAvisa.Infrastructure --startup-project GaroAvisa.Api
```

### 4. Rodar a API

```bash
dotnet run --project GaroAvisa.Api
```

| Página | Endereço |
|---|---|
| Swagger | http://localhost:5080/swagger |
| Scalar | http://localhost:5080/scalar |
| Health (API viva) | http://localhost:5080/health/live |
| Health (banco e Open-Meteo) | http://localhost:5080/health/ready |

### 5. Rodar os testes

```bash
dotnet test
```

Os testes usam banco em memória e uma Open-Meteo falsa, então **não precisam** do Oracle nem de internet.

---

## Requisitos da Global Solution

| # | Requisito | Onde está no projeto | Como verificar |
|---|---|---|---|
| 1 | **API RESTful** com validações, códigos HTTP e **OpenAPI/Swagger** | `Controllers`, DTOs com Data Annotations, `RespostasErro.cs` e `TratamentoErrosHandler.cs` | Swagger em `/swagger`, com resumo, parâmetros e respostas HTTP de cada endpoint |
| 2 | **JWT**: autenticação e autorização | `TokenService.cs`, `AddJwtBearer` no `Program.cs`, `[Authorize]` e `[Authorize(Roles = "ADMIN")]` | Sem token: `401`. Usuário comum em rota de admin: `403` |
| 3 | **Idempotência** | `IdempotenciaFilter.cs`, atributo `[Idempotente]` e tabela `T_GS_IDEMPOTENCIA` | Repetir um POST com o mesmo `Idempotency-Key`: mesma resposta e cabeçalho `Idempotent-Replay: true`. Mesma chave com dados diferentes: `422` |
| 4 | **Rate Limiting** com HTTP 429 | `RateLimitingExtensions.cs` | Mais de 5 logins em 1 minuto: `429` com cabeçalho `Retry-After` |
| 5 | **Testes unitários e de integração** com xUnit | `GaroAvisa.UnitTests` e `GaroAvisa.IntegrationTests` | `dotnet test`: 47 testes aprovados |
| 6 | **Health Checks** da API, do banco e do serviço externo | `Program.cs` e `OpenMeteoHealthCheck.cs` | `/health/live` e `/health/ready` (`oracle` e `open-meteo`) |

### Detalhes

**Validações e erros.** Todas as respostas de erro vêm em português e orientam o que fazer, por exemplo:

```json
{
  "mensagem": "Alguns dados estão inválidos. Confira os campos abaixo.",
  "erros": {
    "Latitude": ["A latitude deve ficar entre -90 e 90."]
  }
}
```

Erros inesperados passam pelo `TratamentoErrosHandler`: banco fora do ar e falha na Open-Meteo devolvem `503`, e qualquer outro erro devolve `500`, sempre com `mensagem`, `status` e `traceId`, sem expor detalhes internos.

**Idempotência.** Os endpoints que criam registros (local, compromisso e regra de alerta) aceitam o cabeçalho opcional `Idempotency-Key`. A API guarda a resposta por 24 horas. Se o mesmo pedido chegar de novo, por exemplo porque a internet caiu e o app reenviou, ela devolve a resposta guardada em vez de criar outro registro.

**Rate Limiting.** Há dois limites, configuráveis no `appsettings.json` (seção `RateLimiting`):

| Política | Limite | Onde |
|---|---|---|
| Global | 100 requisições por minuto, por usuário ou IP | Todos os endpoints |
| Autenticação | 5 requisições por minuto, por IP | Login e cadastro, contra tentativas de adivinhar senha |

Os health checks não entram no limite, para o monitoramento nunca ser bloqueado.

**Health Checks.**

| Endpoint | O que verifica |
|---|---|
| `/health/live` | Se a API está respondendo |
| `/health/ready` | Conexão com o Oracle (`oracle`) e disponibilidade da Open-Meteo (`open-meteo`). Se a Open-Meteo cair, o status fica `Degraded`, porque a API continua funcionando com o que já está salvo |

---

## Perfis de acesso

| Perfil | O que pode fazer |
|---|---|
| **USUARIO** | Gerenciar os próprios locais, compromissos e preferências; excluir a própria conta; ver previsão, alertas e resumo dos seus locais; consultar as regras de alerta |
| **ADMIN** | Tudo o que o usuário faz, mais criar, editar e excluir **regras de alerta** e disparar a **verificação de alertas** |

Toda conta é criada como **USUARIO**. Por segurança, a API não permite que alguém vire administrador sozinho. Para promover uma conta, rode no Oracle:

```sql
UPDATE T_GS_USUARIO SET TIPO = 'ADMIN' WHERE EMAIL = 'email@exemplo.com';
COMMIT;
```

Depois faça login de novo para receber um token com o perfil ADMIN.

Cada usuário só enxerga os próprios dados: buscar o local ou o compromisso de outra pessoa devolve `404`.

---

## Endpoints

Todos os endpoints, exceto cadastro, login e health checks, exigem o token no cabeçalho `Authorization: Bearer {token}`. No Swagger, use o botão **Authorize**.

### Auth
| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/Auth/cadastro` | Cria uma conta e já devolve o token |
| POST | `/api/Auth/login` | Faz login e devolve o token JWT |
| GET | `/api/Auth/me` | Mostra os dados do usuário logado |
| DELETE | `/api/Auth/me` | Exclui a conta do usuário logado e todos os dados dele (contas ADMIN recebem `403`) |

### Lugares
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Lugares?nome=Tatuapé` | Busca lugares na Open-Meteo para depois cadastrar como local |

### Locais
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Locais` | Lista os locais do usuário |
| GET | `/api/Locais/{id}` | Busca um local |
| POST | `/api/Locais` | Cadastra um local (aceita `Idempotency-Key`) |
| PUT | `/api/Locais/{id}` | Atualiza etiqueta, local principal e recebimento de alertas |
| DELETE | `/api/Locais/{id}` | Exclui um local |

### Previsão e Resumo
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Previsao/{localId}?dias=3` | Previsão de 1 a 7 dias de um local, explicada em português |
| GET | `/api/Resumo` | Resumo do dia: previsão e alertas de hoje de cada local |

### Preferências
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Preferencias` | Mostra as notificações escolhidas e o horário do resumo |
| PUT | `/api/Preferencias` | Salva as preferências |
| DELETE | `/api/Preferencias` | Volta para as preferências padrão |

### Regras de alerta
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/RegrasAlerta` | Lista as regras |
| GET | `/api/RegrasAlerta/{id}` | Busca uma regra |
| POST | `/api/RegrasAlerta` | Cria uma regra (**ADMIN**, aceita `Idempotency-Key`) |
| PUT | `/api/RegrasAlerta/{id}` | Atualiza uma regra (**ADMIN**) |
| DELETE | `/api/RegrasAlerta/{id}` | Exclui uma regra (**ADMIN**) |

### Alertas
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Alertas?somenteNaoLidos=false` | Lista os alertas dos locais do usuário |
| GET | `/api/Alertas/{id}` | Busca um alerta |
| PUT | `/api/Alertas/{id}/lido` | Marca um alerta como lido |
| DELETE | `/api/Alertas/{id}` | Exclui um alerta |
| POST | `/api/Alertas/verificar` | Verifica a previsão de todos os locais e cria os alertas (**ADMIN**) |

### Compromissos
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Compromissos?localId=1` | Lista os compromissos (filtro por local opcional) |
| GET | `/api/Compromissos/{id}` | Busca um compromisso |
| GET | `/api/Compromissos/{id}/previsao` | Previsão e alertas do dia do compromisso |
| POST | `/api/Compromissos` | Cadastra um compromisso (aceita `Idempotency-Key`) |
| PUT | `/api/Compromissos/{id}` | Atualiza um compromisso |
| DELETE | `/api/Compromissos/{id}` | Exclui um compromisso |

### Health Checks
| Método | Rota | Descrição |
|---|---|---|
| GET | `/health/live` | API viva |
| GET | `/health/ready` | Banco Oracle e Open-Meteo |

---

## Fluxo de teste no Swagger

**1. Criar conta e fazer login**

`POST /api/Auth/cadastro`
```json
{
  "nome": "Maria",
  "email": "maria@teste.com",
  "senha": "123456"
}
```
Copie o `token` da resposta, clique em **Authorize** e cole.

**2. Buscar o lugar na Open-Meteo**

`GET /api/Lugares` com `nome = Tatuapé`. Use a latitude, a longitude e o fuso retornados no próximo passo.

**3. Cadastrar um local**

`POST /api/Locais`
```json
{
  "etiqueta": "Casa",
  "nome": "Tatuapé",
  "estado": "São Paulo",
  "pais": "Brasil",
  "latitude": -23.53729,
  "longitude": -46.5666,
  "fuso": "America/Sao_Paulo",
  "principal": true,
  "receberAlertas": true
}
```

**4. Ver a previsão**

`GET /api/Previsao/{localId}` com o `id` do local e `dias = 3`.

**5. Cadastrar um compromisso**

`POST /api/Compromissos`
```json
{
  "localId": 1,
  "nome": "Consulta médica",
  "dataHora": "2026-11-20T09:00:00"
}
```
Use o `id` do local criado no passo 3 e troque a data por **um dos próximos dias**: a data não pode estar no passado, e a previsão só fica disponível nos 7 dias antes do compromisso.

Depois, `GET /api/Compromissos/{id}/previsao` mostra o tempo e os alertas do dia do compromisso.

**6. Ajustar as preferências**

`PUT /api/Preferencias`
```json
{
  "resumoDiarioAtivo": true,
  "resumoDiarioHorario": "07:00",
  "alertasDoDia": true,
  "previsaoMudou": true,
  "compromissos": true,
  "silencioNoturno": true
}
```

**7. Cadastrar as regras de alerta (com uma conta ADMIN)**

O banco começa sem regras. Faça login com uma conta ADMIN (veja [Perfis de acesso](#perfis-de-acesso)) e envie cada JSON abaixo em `POST /api/RegrasAlerta`.

Chuva forte:
```json
{
  "tipo": "CHUVA_FORTE",
  "limite": 15,
  "gravidade": "ALTA",
  "titulo": "Chuva forte prevista",
  "descricao": "Evite áreas de alagamento.",
  "ativa": true
}
```

Vento forte:
```json
{
  "tipo": "VENTO_FORTE",
  "limite": 50,
  "gravidade": "MEDIA",
  "titulo": "Ventos fortes",
  "descricao": "Cuidado com árvores e objetos soltos.",
  "ativa": true
}
```

Calor:
```json
{
  "tipo": "CALOR",
  "limite": 34,
  "gravidade": "MEDIA",
  "titulo": "Calor intenso",
  "descricao": "Beba água e evite sol forte.",
  "ativa": true
}
```

Frio:
```json
{
  "tipo": "FRIO",
  "limite": 8,
  "gravidade": "MEDIA",
  "titulo": "Frio intenso",
  "descricao": "Agasalhe-se.",
  "ativa": true
}
```

Tempestade:
```json
{
  "tipo": "TEMPESTADE",
  "limite": 95,
  "gravidade": "ALTA",
  "titulo": "Risco de tempestade",
  "descricao": "Procure abrigo e evite áreas abertas.",
  "ativa": true
}
```

Regra de teste: as regras acima só disparam quando o tempo está ruim de verdade. Para ver um alerta sendo criado em qualquer dia, cadastre também esta regra, que dispara sempre que a máxima passar de 10 °C:
```json
{
  "tipo": "CALOR",
  "limite": 10,
  "gravidade": "BAIXA",
  "titulo": "Teste de calor",
  "descricao": "Regra de teste: máxima acima de 10 °C.",
  "ativa": true
}
```

Como funcionam os campos:

| Tipo | O que o limite mede | Dispara quando |
|---|---|---|
| `CHUVA_FORTE` | Chuva do dia (mm) | Passa do limite |
| `VENTO_FORTE` | Rajada de vento (km/h) | Passa do limite |
| `CALOR` | Temperatura máxima (°C) | Passa do limite |
| `FRIO` | Temperatura mínima (°C) | Fica abaixo do limite |
| `TEMPESTADE` | Código do tempo da Open-Meteo (95 a 99 = tempestade) | Chega no limite |

Gravidades: `BAIXA`, `MEDIA` e `ALTA`. Não pode haver duas regras com o mesmo tipo e a mesma gravidade. Se duas regras do mesmo tipo dispararem, vale a mais grave.

**8. Gerar os alertas (com a conta ADMIN)**

`POST /api/Alertas/verificar` verifica a previsão de hoje e de amanhã de todos os locais de todos os usuários e cria os alertas. Com a regra de teste, `alertasCriados` vem maior que 0.

**9. Ver os alertas (de volta com a conta do passo 1)**

Os alertas pertencem aos locais de quem os cadastrou, e cada usuário só vê os próprios dados. Por isso, faça login de novo com a conta criada no passo 1 e clique em **Authorize** com o novo token.

- `GET /api/Alertas` lista os alertas dos seus locais.
- `PUT /api/Alertas/{id}/lido` marca um alerta como lido.
- `GET /api/Resumo` mostra a previsão e os alertas de hoje de cada local.
- `GET /api/Compromissos/{id}/previsao` mostra a previsão e os alertas do dia do compromisso do passo 5.

---

## Testes automatizados

```bash
dotnet test
```

**47 testes**, todos aprovados:

| Projeto | O que testa |
|---|---|
| **GaroAvisa.UnitTests** | Regras de negócio com Moq: cadastro com senha em hash, e-mail repetido, login, exclusão de conta, etiqueta repetida, local principal, criação de alertas sem duplicar, preferência desligada, Open-Meteo fora do ar, escolha da regra mais grave, mudança brusca na previsão e tradução dos códigos do tempo |
| **GaroAvisa.IntegrationTests** | Endpoints de verdade com `WebApplicationFactory` e banco em memória: cadastro, login, exclusão de conta, `401` sem token, `403` para usuário comum, CRUD de locais, acesso a dados de outro usuário, idempotência (replay e `422`), previsão, verificação de alertas, rate limiting (`429`) e health check |

---

## Tabelas do banco

Criadas pelas Migrations do EF Core no Oracle:

| Tabela | Conteúdo |
|---|---|
| `T_GS_USUARIO` | Contas, com senha em hash e perfil (USUARIO ou ADMIN) |
| `T_GS_LOCAL` | Locais de cada usuário (etiqueta, coordenadas, fuso, principal, receber alertas) |
| `T_GS_PREFERENCIA` | Preferências de notificação de cada usuário |
| `T_GS_REGRA_ALERTA` | Regras que definem quando um alerta é criado |
| `T_GS_ALERTA` | Alertas gerados para cada local |
| `T_GS_PREVISAO` | Última previsão salva de cada local, usada para detectar mudança brusca |
| `T_GS_COMPROMISSO` | Compromissos do usuário em cada local |
| `T_GS_IDEMPOTENCIA` | Respostas guardadas para o controle de idempotência |

---

## Integrantes

| RM | Nome | Turma |
|---|---|---|
| RM561713 | Eduardo Batista Locaspi | 2TDSPI |
| RM563000 | Enzo Vieira Bernardini | 2TDSPI |
| RM565698 | Liana Lyumi Morisita Fujisima | 2TDSPI |
| RM561833 | Victor Alves Lopes | 2TDSPI |
