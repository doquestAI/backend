# 📂 DDD Folder Structure - Referência Completa

## Árvore de Diretórios

```
Domain/
│
├── 🔵 Shared/                          ← SHARED KERNEL (Múltiplos BCs usam)
│   │
│   ├── Core/                           ← Base classes DDD
│   │   ├── AggregateRoot.cs            ✅ Orquestra agregado + eventos
│   │   ├── Entity.cs                   ✅ Entidade com identidade única
│   │   ├── ValueObject.cs              ✅ Objeto de valor imutável
│   │   ├── DomainEvent.cs              ✅ Fato do passado
│   │   └── Specification.cs            ✅ Critério de query
│   │
│   └── Repositories/                   ← Interfaces genéricas
│       ├── IRepository.cs              ✅ CRUD genérico para agregados
│       └── IUnitOfWork.cs              ✅ Transação atômica
│
├── 🟢 Agents/                          ← BC: AGENTES IA
│   │
│   ├── Aggregates/
│   │   └── Agent.cs                    ✅ RAIZ AGREGADA
│   │       └─ Contém: AgentId, AgentName, Role, Status, Capabilities, Metrics
│   │
│   ├── ValueObjects/
│   │   ├── AgentId.cs                  ✅ Guid único → New()
│   │   ├── AgentName.cs                ✅ String (1-256 chars)
│   │   ├── AgentDescription.cs         ✅ String (1-2000 chars)
│   │   ├── AgentSystemPrompt.cs        ✅ String (max 32k)
│   │   ├── AgentRole.cs                ✅ Enum: Helper, Specialist, Generator, Evaluator, Orchestrator
│   │   ├── AgentStatus.cs              ✅ Enum: Idle, Running, Done, Error
│   │   ├── AgentCapabilities.cs        ✅ Plugins[], MCPs[], Streaming, RAG
│   │   └── AgentMetrics.cs             ✅ InputTokens, OutputTokens, Invocations, Failures
│   │
│   ├── Events/
│   │   └── AgentCreatedEvent.cs        ✅ agent.created
│   │   └── AgentInvokedEvent.cs        ✅ agent.invoked
│   │   └── AgentDisabledEvent.cs       ✅ agent.disabled
│   │   └── AgentEnabledEvent.cs        ✅ agent.enabled
│   │   └── AgentSystemPromptUpdatedEvent.cs
│   │   └── AgentCapabilitiesUpdatedEvent.cs
│   │
│   ├── Repositories/
│   │   └── IAgentRepository.cs         ✅ GetByName(), GetByRole(), GetEnabled()
│   │
│   └── Specifications/
│       ├── EnabledAgentsSpecification.cs
│       └── AgentsByRoleSpecification.cs
│
├── 🔵 Sessions/                        ← BC: CONVERSAS/MEMÓRIA
│   │
│   ├── Aggregates/
│   │   └── AgentSession.cs             ✅ RAIZ AGREGADA
│   │       └─ Contém: SessionId, AgentId, UserId, State, Memory[], History[]
│   │   └── ExecutionRecord.cs          ✅ Entidade (ação executada)
│   │
│   ├── ValueObjects/
│   │   ├── SessionId.cs                ✅ Guid único → New(), Create()
│   │   ├── SessionState.cs             ✅ Enum: Active, Paused, Closed, Expired
│   │   ├── MemoryRole.cs               ✅ Enum: User, Agent, System
│   │   └── MemoryEntry.cs              ✅ Role + Content + Timestamp (Imutável)
│   │
│   ├── Events/
│   │   └── SessionCreatedEvent.cs      ✅ session.created
│   │   └── MemoryEntryAddedEvent.cs    ✅ session.memory_entry_added
│   │   └── SessionPausedEvent.cs       ✅ session.paused
│   │   └── SessionResumedEvent.cs      ✅ session.resumed
│   │   └── SessionClosedEvent.cs       ✅ session.closed
│   │   └── ExecutionFailedEvent.cs     ✅ session.execution_failed
│   │   └── SessionMemoryClearedEvent.cs
│   │
│   ├── Repositories/
│   │   └── IAgentSessionRepository.cs  ✅ GetBySessionId(), GetByUserId(), GetActive(), GetExpired()
│   │
│   └── Specifications/
│       ├── ActiveSessionsSpecification.cs
│       └── ExpiredSessionsSpecification.cs
│
├── 🔵 Pipelines/                       ← BC: ORQUESTRAÇÃO DE STEPS
│   │
│   ├── Aggregates/
│   │   ├── Pipeline.cs                 ✅ RAIZ AGREGADA
│   │   │   └─ Contém: PipelineId, Name, Status, Steps[], Metrics
│   │   └── PipelineStep.cs             ✅ Entidade (passo dentro da pipeline)
│   │       └─ StepName, Status, Tokens, Duration, ErrorMessage
│   │
│   ├── ValueObjects/
│   │   ├── PipelineId.cs               ✅ Guid único → New()
│   │   ├── PipelineName.cs             ✅ String (1-256 chars)
│   │   ├── StepName.cs                 ✅ String (1-256 chars)
│   │   ├── PipelineStatus.cs           ✅ Enum: Pending, Running, Completed, Failed, Cancelled
│   │   ├── StepStatus.cs               ✅ Enum: Pending, Running, Completed, Failed, Skipped
│   │   └── TokenMetrics.cs             ✅ InputTokens + OutputTokens (Imutável)
│   │
│   ├── Events/
│   │   └── PipelineCreatedEvent.cs     ✅ pipeline.created
│   │   └── PipelineStartedEvent.cs     ✅ pipeline.started
│   │   └── StepCompletedEvent.cs       ✅ pipeline.step_completed
│   │   └── PipelineCompletedEvent.cs   ✅ pipeline.completed
│   │   └── PipelineFailedEvent.cs      ✅ pipeline.failed
│   │   └── PipelineCancelledEvent.cs   ✅ pipeline.cancelled
│   │
│   ├── Repositories/
│   │   └── IPipelineRepository.cs      ✅ GetByName(), GetByStatus(), GetRecent()
│   │
│   └── Specifications/
│       ├── CompletedPipelinesSpecification.cs
│       └── FailedPipelinesSpecification.cs
│
├── 🔵 Capabilities/                    ← BC: PLUGINS & MCPs (⏳ TODO)
│   │
│   ├── Aggregates/
│   │   ├── Plugin.cs
│   │   └── McpConnection.cs
│   │
│   ├── ValueObjects/
│   │   ├── PluginName.cs
│   │   ├── FunctionName.cs
│   │   ├── McpEndpoint.cs
│   │   ├── FunctionCallDescriptor.cs
│   │   └── ParameterDescriptor.cs
│   │
│   ├── Events/
│   │   ├── PluginRegisteredEvent.cs
│   │   └── McpConnectedEvent.cs
│   │
│   ├── Repositories/
│   │   ├── IPluginRepository.cs
│   │   └── IMcpRepository.cs
│   │
│   └── Specifications/
│       └── EnabledPluginsSpecification.cs
│
├── 🔵 Documents/                       ← BC: DOCUMENTOS (⏳ TODO - refatorar)
│   │
│   ├── Aggregates/
│   │   └── Document.cs
│   │
│   ├── ValueObjects/
│   │   ├── DocumentId.cs
│   │   ├── FileName.cs
│   │   ├── FileSize.cs
│   │   ├── DocumentStatus.cs
│   │   ├── EmbeddingStatus.cs
│   │   └── EmbeddingMetadata.cs
│   │
│   ├── Events/
│   │   ├── DocumentUploadedEvent.cs
│   │   ├── DocumentEmbeddingStartedEvent.cs
│   │   ├── DocumentEmbeddingCompletedEvent.cs
│   │   └── DocumentEmbeddingFailedEvent.cs
│   │
│   ├── Repositories/
│   │   └── IDocumentRepository.cs
│   │
│   └── Specifications/
│       ├── EmbeddedDocumentsSpecification.cs
│       └── FailedEmbeddingsSpecification.cs
│
├── 🟡 Configurations/                  ← Compartilhado (Não é BC puro)
│   ├── AppSettings.cs
│   ├── AzureSettings.cs
│   ├── AzureAdSettings.cs
│   ├── AzureKeyvaultSettings.cs
│   ├── DatabaseSettings.cs
│   ├── ServiceBusSettings.cs
│   ├── CacheSettings.cs
│   ├── ObservabilitySettings.cs
│   ├── RateLimitSettings.cs
│   ├── StripeSettings.cs
│   ├── SignedUrlSettings.cs
│   └── PromptProvider.cs
│
├── 🟡 Messages/                        ← Integration Events (Publicadas)
│   ├── DocumentEmbeddingMessage.cs
│   ├── StorageUploadMessage.cs
│   ├── StorageDeleteMessage.cs
│   ├── NotificationEmailMessage.cs
│   └── [Mais...]
│
├── DDD_ARCHITECTURE.md                 📖 Guia completo do DDD
└── DDD_STRUCTURE.md                    📖 Este arquivo (árvore de pastas)
```

---

## Legenda

| Símbolo | Significado |
|---------|------------|
| 🔵 | Bounded Context (Raiz) |
| 🟢 | Bounded Context Implementado ✅ |
| 🟡 | Compartilhado (não é BC puro) |
| ✅ | Implementado/Concluído |
| ⏳ | Em desenvolvimento |

---

## Padrão de Nomenclatura

### Pastas por Bounded Context

```
{BoundedContext}/
├── Aggregates/        → Raízes agregadas (1+ por BC, geralmente 1)
├── ValueObjects/      → VOs do BC (múltiplos)
├── Events/           → Eventos de domínio do BC
├── Repositories/     → Interfaces de repositório
└── Specifications/   → Specifications para queries
```

### Arquivos

| Padrão | Exemplo | Tipo |
|--------|---------|------|
| `{Entity}.cs` | `Agent.cs` | Aggregate Root |
| `{ValueObject}.cs` | `AgentName.cs` | Value Object |
| `{Event}.cs` | `AgentCreatedEvent.cs` | Domain Event |
| `I{Entity}Repository.cs` | `IAgentRepository.cs` | Interface |
| `{Criteria}Specification.cs` | `EnabledAgentsSpecification.cs` | Specification |

---

## Invariantes por BC

### 🟢 AGENTS

- ✓ AgentName é único
- ✓ SystemPrompt ≤ 32k chars
- ✓ Temperature ∈ [0, 2]
- ✓ Role define papel semântico
- ✓ Status: Idle → Running → Done
- ✓ Metrics acumula apenas sucessos

### 🟢 SESSIONS

- ✓ SessionId é único
- ✓ Memória nunca é deletada (soft delete)
- ✓ Apenas Active aceita novas entradas
- ✓ State: Active ↔ Paused → Closed
- ✓ Closed não volta a Active
- ✓ Expira por TTL

### 🟢 PIPELINES

- ✓ PipelineId é único
- ✓ Status: Pending → Running → Completed|Failed|Cancelled
- ✓ Não adiciona steps após Start()
- ✓ Short-circuit em erro
- ✓ Métricas acumulam de todos steps
- ✓ TokenMetrics é imutável

---

## Checklist de Refactoring

### ✅ Concluído

- [x] Criar `Shared/Core/` com bases DDD
- [x] Criar `Shared/Repositories/` com interfaces genéricas
- [x] Refatorar `Agents/` com Aggregate + VOs + Events
- [x] Refatorar `Sessions/` com Aggregate + VOs + Events
- [x] Refatorar `Pipelines/` com Aggregate + VOs + Events
- [x] Criar `DDD_ARCHITECTURE.md`
- [x] Criar `DDD_STRUCTURE.md`

### ⏳ Em Progresso

- [ ] Refatorar `Capabilities/` BC
- [ ] Refatorar `Documents/` BC
- [ ] Implementar `Infrastructure/` repositories
- [ ] Implementar Event Handlers
- [ ] Adicionar Domain Services (multi-agregados)

### 📝 Future

- [ ] Adicionar Bounded Context: `Accounts`
- [ ] Adicionar Bounded Context: `Subscriptions`
- [ ] Adicionar Bounded Context: `Payments` (Stripe)
- [ ] Event Sourcing for Agents
- [ ] CQRS para Queries complexas

---

## Migração do Código Existente

### Arquivos que devem ser MOVIDOS

```
ValueObjects/*                     → {BC}/ValueObjects/
  AgentId.cs                      → Agents/ValueObjects/
  AgentName.cs                    → Agents/ValueObjects/
  SessionId.cs                    → Sessions/ValueObjects/
  
Entities/Abstracts/Entity.cs      → Shared/Core/Entity.cs (refatorado)
Common/ValueObject.cs             → Shared/Core/ValueObject.cs (refatorado)

Interfaces/Repositories/*         → {BC}/Repositories/ (IAgentRepository, etc)
```

### Arquivos que devem ser CRIADOS

```
Shared/Core/AggregateRoot.cs      ← Base para raízes
Agents/Aggregates/Agent.cs        ← Refatorado com Factory
Sessions/Aggregates/AgentSession.cs ← Refatorado
Pipelines/Aggregates/Pipeline.cs  ← Novo
{BC}/Events/*.cs                  ← Domain Events
{BC}/Specifications/*.cs          ← Specifications
```

### Arquivos que podem ficar (Legacy)

```
Configurations/*                  → Manter conforme está
Messages/*                        → Integração, fora de BC
```

---

## Exemplos de Uso

### Criar um Agente

```csharp
// Domain/Agents/Aggregates/Agent.cs
var agent = Agent.Create(
    new AgentName("QuestionGenerator"),
    new AgentDescription("Generates ENEM questions"),
    AgentRole.Generator,
    new AgentSystemPrompt("You are an expert ENEM teacher..."),
    temperature: 0.7,
    maxOutputTokens: 2048
);

if (agent.IsValid)
    await _agentRepository.AddAsync(agent);
```

### Registrar Invocação

```csharp
agent.RecordInvocation(
    inputTokens: 2000,
    outputTokens: 500,
    duration: TimeSpan.FromMilliseconds(2400)
);

// Gera AgentInvokedEvent automaticamente
var evt = agent.UncommittedEvents.First();  // AgentInvokedEvent
```

### Usar Specification

```csharp
var enabledAgents = await _agentRepository.ListAsync(
    new EnabledAgentsSpecification()
);

var generatorAgents = await _agentRepository.ListAsync(
    new AgentsByRoleSpecification(AgentRole.Generator)
);
```

### Criar Sessão

```csharp
var session = AgentSession.Create(
    agent.AgentId,
    userId: currentUser.Id,
    ttl: TimeSpan.FromHours(24)
);

session.AddMemoryEntry(
    MemoryRole.User,
    "Explique fotossíntese",
    name: "João"
);

await _sessionRepository.AddAsync(session);
```

---

## Leitura Recomendada

- 📖 **Domain-Driven Design: Tackling Complexity in the Heart of Software** - Eric Evans
- 📖 **Patterns, Principles, and Practices of Domain-Driven Design** - Scott Millett & Nick Tune
- 🎯 **CQRS and Event Sourcing** - Greg Young
- 🔗 https://www.domainlanguage.com/ddd/

---

**Mantido por:** @yourusername  
**Última atualização:** 2026-07-09  
**Status:** 🟢 Draft Funcional
