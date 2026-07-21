# 🏛️ Domain-Driven Design (DDD) Architecture

> Guia completo da estrutura 100% DDD do Domain no DoquEST

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Estrutura de Pastas](#estrutura-de-pastas)
3. [Bounded Contexts](#bounded-contexts)
4. [Padrões DDD](#padrões-ddd)
5. [Como Usar](#como-usar)

---

## Visão Geral

A Domain está organizada em **5 Bounded Contexts** independentes + **1 Shared Kernel**:

```
Domain/
├── Shared/                    ← Shared Kernel (Core DDD)
│
├── Agents/                    ← BOUNDED CONTEXT: Agentes IA
├── Sessions/                  ← BOUNDED CONTEXT: Conversas
├── Pipelines/                 ← BOUNDED CONTEXT: Orquestração
├── Capabilities/              ← BOUNDED CONTEXT: Plugins/MCPs
├── Documents/                 ← BOUNDED CONTEXT: Documentos
│
└── Configurations/            ← Shared Configs (Não é BC puro)
```

---

## Estrutura de Pastas

### 📁 `Shared/Core/` (Shared Kernel)

**Bases abstratas compartilhadas por todos Bounded Contexts:**

```
Shared/Core/
├── AggregateRoot.cs          ← Raiz agregada abstrata
├── Entity.cs                 ← Entidade abstrata
├── ValueObject.cs            ← Value Object abstrato
├── DomainEvent.cs            ← Evento de domínio
└── Specification.cs          ← Specification Pattern
```

**Responsabilidades:**
- `AggregateRoot` - Orquestra aggregado + eventos não commitados
- `Entity` - Identidade única, soft delete, comparação por ID
- `ValueObject` - Imutabilidade, comparação por valor, sem identidade
- `DomainEvent` - Eventos que representam fatos do passado
- `Specification` - Encapsula critérios de query reutilizáveis

### 📁 `Shared/Repositories/` 

```
Shared/Repositories/
├── IRepository.cs            ← Interface genérica para todos BCs
└── IUnitOfWork.cs            ← Unit of Work Pattern (transações)
```

**Como usar:**
```csharp
public interface IRepository<T> where T : AggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<T>> ListAsync(Specification<T> spec, CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, CancellationToken ct);
}
```

---

## Bounded Contexts

### 1. 🤖 **AGENTS Bounded Context**

Gerencia configuração, capacidades e métricas de agentes IA.

```
Agents/
├── Aggregates/
│   └── Agent.cs              ← AGGREGATE ROOT
│       ├─ AgentId (VO)
│       ├─ AgentName (VO)
│       ├─ SystemPrompt (VO)
│       ├─ Role (Enum VO)
│       ├─ Status (Enum VO)
│       ├─ Capabilities (VO)
│       └─ Metrics (VO)
│
├── ValueObjects/
│   ├── AgentId.cs            ← Guid único
│   ├── AgentName.cs          ← String validado (1-256 chars)
│   ├── AgentDescription.cs   ← String validado (1-2000 chars)
│   ├── AgentSystemPrompt.cs  ← String validado (max 32k)
│   ├── AgentRole.cs          ← Enum: Helper, Specialist, Generator, etc
│   ├── AgentStatus.cs        ← Enum: Idle, Running, Done, Error
│   ├── AgentCapabilities.cs  ← Plugins, MCPs, Streaming, RAG
│   └── AgentMetrics.cs       ← Tokens, invocações, falhas
│
├── Events/
│   ├── AgentCreatedEvent
│   ├── AgentInvokedEvent
│   ├── AgentDisabledEvent
│   ├── AgentEnabledEvent
│   ├── AgentSystemPromptUpdatedEvent
│   └── AgentCapabilitiesUpdatedEvent
│
├── Repositories/
│   └── IAgentRepository.cs    ← GetByName, GetByRole, GetEnabled
│
└── Specifications/
    ├── EnabledAgentsSpecification
    └── AgentsByRoleSpecification
```

**Invariantes de Negócio:**
- ✓ AgentName é único e imutável
- ✓ SystemPrompt max 32k chars (limite Claude)
- ✓ Role define comportamento semântico
- ✓ Status lifecycle: Idle → Running → Done
- ✓ Metrics acumulam apenas invocações bem-sucedidas

**Exemplo de uso:**
```csharp
var agentName = new AgentName("QuestionGenerator");
var prompt = new AgentSystemPrompt("Generate ENEM questions...");
var agent = Agent.Create(
    agentName,
    new AgentDescription("Generates multiple choice questions"),
    AgentRole.Generator,
    prompt,
    temperature: 0.7
);

// Validar antes de persistir
if (agent.IsValid)
{
    await _agentRepository.AddAsync(agent);
    // PublishDomainEvents(agent.UncommittedEvents);
}
```

---

### 2. 💬 **SESSIONS Bounded Context**

Gerencia conversas entre usuários e agentes (memória de conversa).

```
Sessions/
├── Aggregates/
│   └── AgentSession.cs       ← AGGREGATE ROOT
│       ├─ SessionId (VO)
│       ├─ AgentId (referência)
│       ├─ State (Enum VO)
│       ├─ MemoryEntries (List<MemoryEntry>)
│       ├─ ExecutionHistory (List<ExecutionRecord>)
│       └─ TTL e Timestamps
│
├── ValueObjects/
│   ├── SessionId.cs          ← Guid único por conversa
│   ├── SessionState.cs       ← Enum: Active, Paused, Closed, Expired
│   ├── MemoryRole.cs         ← Enum: User, Agent, System
│   └── MemoryEntry.cs        ← Uma mensagem no histórico
│
├── Events/
│   ├── SessionCreatedEvent
│   ├── MemoryEntryAddedEvent
│   ├── SessionPausedEvent
│   ├── SessionResumedEvent
│   ├── SessionClosedEvent
│   ├── ExecutionFailedEvent
│   └── SessionMemoryClearedEvent
│
├── Repositories/
│   └── IAgentSessionRepository.cs ← GetBySessionId, GetByUserId, GetActive
│
└── Specifications/
    ├── ActiveSessionsSpecification
    └── ExpiredSessionsSpecification
```

**Invariantes de Negócio:**
- ✓ SessionId é único e imutável
- ✓ Memória nunca é deletada (soft delete)
- ✓ Apenas sessões Active aceitam novas entradas
- ✓ Session pode pausar/resumir mas não voltar de Closed
- ✓ TTL opcional para expiração automática

**Exemplo de uso:**
```csharp
var session = AgentSession.Create(agentId, userId: currentUser, ttl: TimeSpan.FromHours(24));

// Conversa
session.AddMemoryEntry(MemoryRole.User, "Explique fotossíntese");
session.RecordExecution("ExplainerAgent", success: true, duration);
session.AddMemoryEntry(MemoryRole.Agent, "Fotossíntese é o processo...");

// Persistir
await _sessionRepository.UpdateAsync(session);

// Pausar (sem perder memória)
session.Pause();

// Recuperar histórico
var recentMessages = session.GetLastMemoryEntries(10);
```

---

### 3. 🔄 **PIPELINES Bounded Context**

Orquestra execução de múltiplos steps em sequência.

```
Pipelines/
├── Aggregates/
│   └── Pipeline.cs           ← AGGREGATE ROOT
│       ├─ PipelineId (VO)
│       ├─ PipelineName (VO)
│       ├─ Status (Enum VO)
│       ├─ Steps[] (List<PipelineStep>)
│       └─ Métricas acumuladas
│
│   └── PipelineStep.cs       ← ENTIDADE (filha de Pipeline)
│       ├─ StepName (VO)
│       ├─ Status (Enum VO)
│       ├─ Tokens (TokenMetrics)
│       └─ Duration
│
├── ValueObjects/
│   ├── PipelineId.cs         ← Guid único
│   ├── PipelineName.cs       ← String validado
│   ├── StepName.cs           ← String validado
│   ├── PipelineStatus.cs     ← Enum: Pending, Running, Completed, Failed, Cancelled
│   ├── StepStatus.cs         ← Enum: Pending, Running, Completed, Failed, Skipped
│   └── TokenMetrics.cs       ← Input + Output tokens (immutável)
│
├── Events/
│   ├── PipelineCreatedEvent
│   ├── PipelineStartedEvent
│   ├── StepCompletedEvent
│   ├── PipelineCompletedEvent
│   ├── PipelineFailedEvent
│   └── PipelineCancelledEvent
│
├── Repositories/
│   └── IPipelineRepository.cs ← GetByName, GetByStatus, GetRecent
│
└── Specifications/
    ├── CompletedPipelinesSpecification
    └── FailedPipelinesSpecification
```

**Invariantes de Negócio:**
- ✓ Pipeline status lifecycle: Pending → Running → Completed|Failed|Cancelled
- ✓ Não pode adicionar steps após Start()
- ✓ Short-circuit em falha (não executa steps restantes)
- ✓ Métricas acumulam de todos os steps
- ✓ TokenMetrics é imutável (Add retorna novo objeto)

**Exemplo de uso:**
```csharp
var pipeline = Pipeline.Create(new PipelineName("GenerateQuestion"));
pipeline.AddStep(new PipelineStep(new StepName("Validate"), order: 0));
pipeline.AddStep(new PipelineStep(new StepName("GenerateQuestion"), order: 1));
pipeline.AddStep(new PipelineStep(new StepName("ValidateOutput"), order: 2));

pipeline.Start();
pipeline.CompleteStep(0, TokenMetrics.Empty, TimeSpan.FromMs(50));
pipeline.CompleteStep(1, new TokenMetrics(2000, 500), TimeSpan.FromMs(2400));
pipeline.Complete();

// Métricas finais
Console.WriteLine($"Total: {pipeline.TotalTokens}"); // 2500 tokens
Console.WriteLine($"Duration: {pipeline.TotalDuration}");
```

---

### 4. 🔌 **CAPABILITIES Bounded Context** (A implementar)

Gerencia Plugins e MCPs (Model Context Protocol) conectados.

**Estrutura esperada:**
```
Capabilities/
├── Aggregates/
│   ├── Plugin.cs             ← AGGREGATE ROOT
│   │   └─ PluginName (VO)
│   └── McpConnection.cs      ← AGGREGATE ROOT
│       └─ McpEndpoint (VO)
├── ValueObjects/
│   ├── FunctionName.cs
│   ├── PluginName.cs
│   ├── McpEndpoint.cs
│   ├── FunctionCallDescriptor.cs
│   └── ParameterDescriptor.cs
├── Events/
│   ├── PluginRegisteredEvent
│   └── McpConnectedEvent
└── Repositories/
    ├── IPluginRepository.cs
    └── IMcpRepository.cs
```

---

### 5. 📄 **DOCUMENTS Bounded Context** (A refatorar)

Gerencia documentos, embeddings, storage.

**Estrutura esperada:**
```
Documents/
├── Aggregates/
│   └── Document.cs           ← AGGREGATE ROOT
│       ├─ DocumentId (VO)
│       ├─ FileName (VO)
│       ├─ Status (Enum VO)
│       ├─ EmbeddingMetadata (VO)
│       └─ SignedUrl (VO)
├── ValueObjects/
│   ├── DocumentId.cs
│   ├── FileName.cs
│   ├── FileSize.cs
│   ├── DocumentStatus.cs
│   ├── EmbeddingStatus.cs
│   └── EmbeddingMetadata.cs
├── Events/
│   ├── DocumentUploadedEvent
│   ├── DocumentEmbeddingStartedEvent
│   ├── DocumentEmbeddingCompletedEvent
│   └── DocumentEmbeddingFailedEvent
└── Repositories/
    └── IDocumentRepository.cs
```

---

## Padrões DDD

### 🎯 Aggregate Root (Raiz Agregada)

**O que é:**
- Entidade que é ponto de entrada para um agregado
- Garante invariantes de negócio
- Gera eventos de domínio
- NUNCA é acessada de fora do agregado via entidades filhas

**No projeto:**
```csharp
public sealed class Agent : AggregateRoot          // ← Agent é a raiz
{
    public AgentId AgentId { get; }
    public AgentName Name { get; }
    public List<DomainEvent> UncommittedEvents { get; } // ← Eventos não persistidos
    
    public static Agent Create(...) => ...         // ← Factory method
    public void RecordInvocation(...) { }          // ← Métodos de negócio
}
```

**Padrão:**
- Sempre `sealed class`
- Sempre herda de `AggregateRoot`
- NUNCA public setter (mutações via métodos)
- Construtor privado + Factory method estático `Create()`

---

### 💎 Value Object (Objeto de Valor)

**O que é:**
- Imutável, sem identidade
- Comparado por valor (não por referência)
- Validação no constructor
- Encapsula lógica de domínio

**No projeto:**
```csharp
public sealed class AgentName : ValueObject       // ← VO é sealed
{
    public string Value { get; }                   // ← Readonly
    
    public AgentName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            AddNotification(...);                  // ← Validação sem exceção
        Value = value;
    }
    
    public override bool Equals(object? obj) =>    // ← Comparação por valor
        obj is AgentName other && Value == other.Value;
}
```

**Padrão:**
- Sempre `sealed class`
- Sempre herda de `ValueObject`
- Propriedades readonly
- Validação no constructor via `AddNotification()`
- Override `Equals()` e `GetHashCode()`

---

### 📋 Specification Pattern

**O que é:**
- Encapsula critérios de query
- Reutilizável, testável
- Sem lógica de filtro espalhada no repositório

**No projeto:**
```csharp
public sealed class EnabledAgentsSpecification : Specification<Agent>
{
    public EnabledAgentsSpecification()
    {
        Criteria = agent => agent.IsEnabled && agent.DeletedAt == null;
    }
}

// Usar em repositório:
var enabledAgents = await _agentRepository.ListAsync(
    new EnabledAgentsSpecification()
);
```

**Padrão:**
- Herda de `Specification<T>` (base genérica)
- Define `Criteria` (LINQ predicate)
- Pode incluir `Includes`, `OrderBy`, `Paging`
- Reutilizável entre repositórios

---

### 🎤 Domain Events (Eventos de Domínio)

**O que é:**
- Fatos que aconteceram no passado
- Imutáveis, com timestamp
- Fonte de sincronização entre BC

**No projeto:**
```csharp
public sealed record AgentCreatedEvent(Guid AggregateId, string AgentName, AgentRole Role)
    : DomainEvent(AggregateId);

// No agregado:
public static Agent Create(...) 
{
    var agent = new Agent { ... };
    agent.RaiseDomainEvent(new AgentCreatedEvent(agent.Id, ...));
    return agent;
}

// Consumidor escuta evento:
public class CreateAgentHandler : IDomainEventHandler<AgentCreatedEvent>
{
    public async Task Handle(AgentCreatedEvent evt)
    {
        // Reagir ao evento (enviar notificação, sincronizar outros BCs, etc)
    }
}
```

**Padrão:**
- Sempre `sealed record`
- Herda de `DomainEvent`
- Imutável
- Publicar via `RaiseDomainEvent()`
- Consumir via `IDomainEventHandler<T>`

---

### 🏪 Repository Pattern

**O que é:**
- Interface entre Domain e Infrastructure
- Trabalha APENAS com Aggregates (raízes)
- Espera especificações, não SQL

**No projeto:**
```csharp
public interface IAgentRepository : IRepository<Agent>
{
    Task<Agent?> GetByNameAsync(AgentName name, CancellationToken ct);
    Task<IReadOnlyList<Agent>> GetByRoleAsync(AgentRole role, CancellationToken ct);
    Task<IReadOnlyList<Agent>> GetEnabledAsync(CancellationToken ct);
}

// Usar em Application:
var agent = await _agentRepository.GetByNameAsync(name);
```

**Padrão:**
- Interface em Domain, implementação em Infrastructure
- Genérico `IRepository<T>` para CRUD comum
- Métodos específicos por BC (GetByName, GetByRole)
- Aceita `Specification<T>` para queries complexas

---

### 🛠️ Unit of Work Pattern

**O que é:**
- Coordena persistência de múltiplos agregados
- Garant transação atômica
- Publica eventos de domínio após commit

**Interface:**
```csharp
public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync(CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);  // ← Persiste agregados
    Task CommitAsync(CancellationToken ct);           // ← Publica eventos
    Task RollbackAsync(CancellationToken ct);
}

// Usar em Handler:
public class CreateAgentHandler
{
    public async Task Handle(CreateAgentCommand cmd)
    {
        var agent = Agent.Create(...);
        
        await _uow.BeginTransactionAsync();
        await _agentRepository.AddAsync(agent);
        await _uow.SaveChangesAsync();  // ← Persiste
        
        foreach (var evt in agent.UncommittedEvents)
            await _eventPublisher.PublishAsync(evt);
            
        await _uow.CommitAsync();
    }
}
```

---

## Como Usar

### Criar um novo Agregado

1. **Definir Value Objects** em `{BC}/ValueObjects/`:
```csharp
public sealed class AgentName : ValueObject { ... }
```

2. **Criar Aggregate Root** em `{BC}/Aggregates/`:
```csharp
public sealed class Agent : AggregateRoot 
{
    public static Agent Create(...) { ... }
}
```

3. **Definir Events** em `{BC}/Events/`:
```csharp
public sealed record AgentCreatedEvent(...) : DomainEvent(...) { }
```

4. **Criar Repository Interface** em `{BC}/Repositories/`:
```csharp
public interface IAgentRepository : IRepository<Agent> { ... }
```

5. **Implementar Repository** em `Infrastructure/{BC}/Repositories/`:
```csharp
public class AgentRepository : IAgentRepository { ... }
```

---

### Adicionar Invariante de Negócio

```csharp
public sealed class Agent : AggregateRoot
{
    public void RecordInvocation(long inputTokens, long outputTokens)
    {
        // Invariante: não registra invocações com tokens negativos
        if (inputTokens < 0 || outputTokens < 0)
        {
            AddNotification(nameof(inputTokens), "Tokens cannot be negative");
            return;
        }
        
        Metrics.RecordInvocation(inputTokens, outputTokens, duration);
        RaiseDomainEvent(new AgentInvokedEvent(Id, inputTokens, outputTokens));
    }
}
```

---

### Usar Specification

```csharp
// Definir
public sealed class ActiveAgentsWithHighMetricsSpecification : Specification<Agent>
{
    public ActiveAgentsWithHighMetricsSpecification()
    {
        Criteria = a => a.IsEnabled && a.Metrics.InvocationCount > 100;
        AddInclude(a => a.Capabilities);
        ApplyPaging(skip: 0, take: 50);
    }
}

// Usar
var activeAgents = await _agentRepository.ListAsync(
    new ActiveAgentsWithHighMetricsSpecification()
);
```

---

### Eventos de Domínio

**Publicar:**
```csharp
public sealed class Agent : AggregateRoot
{
    public void Disable()
    {
        IsEnabled = false;
        RaiseDomainEvent(new AgentDisabledEvent(Id, Name.Value));
    }
}
```

**Consumir:**
```csharp
public class AgentDisabledEventHandler : IDomainEventHandler<AgentDisabledEvent>
{
    private readonly IEmailService _emailService;
    
    public async Task Handle(AgentDisabledEvent evt, CancellationToken ct)
    {
        // Reage ao evento
        await _emailService.SendAsync(
            to: "admin@example.com",
            subject: $"Agent {evt.AgentName} foi desabilitado"
        );
    }
}
```

---

## Checklist DDD

- ✅ Cada Bounded Context é independente
- ✅ Raízes agregadas são a entrada
- ✅ Value Objects são imutáveis e validados
- ✅ Eventos de domínio representam fatos
- ✅ Repositories trabalham com Aggregates
- ✅ Specifications encapsulam queries
- ✅ Invariantes de negócio estão protegidos
- ✅ Sem regra de negócio em Infrastructure
- ✅ Sem regra de negócio em Application

---

## Próximos Passos

1. **Refatorar Capabilities BC** para seguir mesmo padrão
2. **Refatorar Documents BC** - Document como Aggregate Root
3. **Implementar Repositories** em Infrastructure
4. **Adicionar Event Handlers** para sincronização entre BCs
5. **Criar Domain Services** para operações multi-agregados
6. **Adicionar Notifications** aos Application Commands

---

**Autor:** DDD Refactoring  
**Data:** 2026-07-09  
**Status:** ✅ Prototipo - Agents, Sessions, Pipelines implementados
