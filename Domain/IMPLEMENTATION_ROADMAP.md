# 🗺️ Implementation Roadmap - DDD Refactoring

## Status Atual

✅ **Fase 1: Domain Foundation - COMPLETA**

- [x] Shared Kernel (Core classes)
- [x] Agents BC (Aggregate + VOs + Events + Repository)
- [x] Sessions BC (Aggregate + VOs + Events + Repository)
- [x] Pipelines BC (Aggregate + VOs + Events + Repository)
- [x] Documentação (Architecture + Structure + Cheatsheet)

---

## 📋 Próximas Fases

### ⏳ Fase 2: Refactor Remaining BCs (1-2 semanas)

#### Tarefa 2.1: Capabilities BC

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Criar `Domain/Capabilities/Aggregates/Plugin.cs`
   - Raiz agregada para gerenciar plugins
   - VOs: PluginName, FunctionName, ParameterDescriptor
   
2. Criar `Domain/Capabilities/Aggregates/McpConnection.cs`
   - Raiz agregada para MCPs
   - VOs: McpEndpoint, McpTransport
   
3. Criar events: PluginRegisteredEvent, McpConnectedEvent, etc

4. Criar `Domain/Capabilities/Repositories/IPluginRepository.cs`
   - GetByNameAsync, GetEnabledAsync, etc

**Tempo estimado:** 4 horas

**Dependências:** Nenhuma - pode fazer em paralelo

---

#### Tarefa 2.2: Documents BC

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Refatorar `Domain/Documents/Aggregates/Document.cs`
   - Audit: Document é raiz agregada
   - Remover lógica de Storage (mover para Domain Service)
   - Encapsular estado via métodos

2. Criar VOs necessários:
   - DocumentId
   - FileName
   - FileSize
   - DocumentStatus (já existe, mover)
   - EmbeddingMetadata
   - SignedUrl

3. Criar events: DocumentUploadedEvent, EmbeddingCompletedEvent, etc

4. Criar repository e specifications

**Tempo estimado:** 6 horas

**Dependências:** Nenhuma

---

#### Tarefa 2.3: Accounts BC (Novo!)

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Criar `Domain/Accounts/Aggregates/Account.cs`
   - Raiz agregada para usuários
   
2. VOs: Email, FullName, Cpf, Address, EntraUserId, StripeCustomerId
   - Mover do `Domain/ValueObjects/` global para aqui

3. Events: AccountCreatedEvent, AccountDeletedEvent, etc

4. Repository e specifications

**Tempo estimado:** 4 horas

**Dependências:** Nenhuma

---

#### Tarefa 2.4: Subscriptions BC (Novo!)

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Criar `Domain/Subscriptions/Aggregates/AccountSubscription.cs`
   - Raiz agregada para planos

2. VOs: SubscriptionId, SubscriptionPeriod, SubscriptionStatus, SubscriptionType

3. Events: SubscriptionActivatedEvent, SubscriptionCancelledEvent, etc

4. Repository e specifications

**Tempo estimado:** 4 horas

**Dependências:** Accounts BC

---

### ⏳ Fase 3: Infrastructure Implementation (2-3 semanas)

#### Tarefa 3.1: Database Schema

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Criar EF Core DbContext com DbSets para cada Aggregate Root
2. Configurar Fluent API mappings
3. Gerar migrations

**Estrutura:**
```
Infrastructure/
├── Data/
│   ├── DoquestDbContext.cs
│   ├── Configurations/
│   │   ├── AgentConfiguration.cs
│   │   ├── AgentSessionConfiguration.cs
│   │   ├── PipelineConfiguration.cs
│   │   ├── DocumentConfiguration.cs
│   │   ├── AccountConfiguration.cs
│   │   └── SubscriptionConfiguration.cs
│   └── Migrations/
│       └── [Auto-generated]
```

**Tempo estimado:** 8 horas

**Dependências:** Fase 2 (todas BCs)

---

#### Tarefa 3.2: Repository Implementations

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Implementar todos `IRepository<T>` genéricos
2. Implementar `ISpecificationEvaluator<T>`
3. Implementar `IUnitOfWork`

**Estrutura:**
```
Infrastructure/
├── Agents/
│   └── Repositories/
│       └── AgentRepository.cs
├── Sessions/
│   └── Repositories/
│       └── AgentSessionRepository.cs
├── Pipelines/
│   └── Repositories/
│       └── PipelineRepository.cs
├── Documents/
│   └── Repositories/
│       └── DocumentRepository.cs
├── Accounts/
│   └── Repositories/
│       └── AccountRepository.cs
├── Subscriptions/
│   └── Repositories/
│       └── SubscriptionRepository.cs
└── UnitOfWork.cs
```

**Tempo estimado:** 12 horas

**Dependências:** Tarefa 3.1 (DbContext)

---

### ⏳ Fase 4: Application Integration (2 semanas)

#### Tarefa 4.1: Event Publishing

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Integrar MediatR com DomainEvents
2. Criar `IDomainEventPublisher`
3. Publicar events após UnitOfWork.CommitAsync()

**Código exemplo:**
```csharp
// UnitOfWork.cs
public async Task CommitAsync(CancellationToken ct)
{
    // Coletar eventos de todos agregados em transaction
    var events = GetUncommittedDomainEvents();
    
    // Publicar via MediatR
    foreach (var evt in events)
        await _mediator.Publish(evt, ct);
}
```

**Tempo estimado:** 4 horas

**Dependências:** Tarefa 3.2

---

#### Tarefa 4.2: Domain Event Handlers

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Criar handlers para cada Domain Event
2. Reagir aos eventos (ex: enviar emails, atualizar cache)

**Exemplos:**
```csharp
public class AgentCreatedEventHandler : INotificationHandler<AgentCreatedEvent>
{
    public async Task Handle(AgentCreatedEvent evt, CancellationToken ct)
    {
        // Enviar notificação
        await _notificationService.SendAsync(...);
    }
}
```

**Tempo estimado:** 6 horas

**Dependências:** Tarefa 4.1

---

#### Tarefa 4.3: Update Application Handlers

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Atualizar todos Commands em Application/UseCases
2. Usar Aggregates ao invés de Entities
3. Usar Repositories
4. Usar UnitOfWork

**Exemplo de mudança:**
```csharp
// ANTES
public async Task<Response> Handle(CreateAgentCommand cmd, CancellationToken ct)
{
    var agent = new Agent { Name = cmd.Name };
    _dbContext.Agents.Add(agent);
    await _dbContext.SaveChangesAsync(ct);
}

// DEPOIS
public async Task<Response> Handle(CreateAgentCommand cmd, CancellationToken ct)
{
    var agent = Agent.Create(new AgentName(cmd.Name), ...);
    if (!agent.IsValid)
        return new Response(false, string.Join("; ", agent.Notifications.Select(n => n.Message)));

    await _agentRepository.AddAsync(agent, ct);
    await _unitOfWork.SaveChangesAsync(ct);
    
    foreach (var evt in agent.UncommittedEvents)
        await _mediator.Publish(evt, ct);
        
    await _unitOfWork.CommitAsync(ct);
    
    return new Response(true, "Agent created successfully");
}
```

**Tempo estimado:** 8 horas

**Dependências:** Tarefa 4.1

---

#### Tarefa 4.4: Add Notifications/Validations

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Converter Flunt Notifications em responses estruturadas
2. Adicionar FluentValidation em Commands (optional)
3. Standardizar error responses

**Tempo estimado:** 4 horas

**Dependências:** Tarefa 4.3

---

### ✅ Fase 5: Testing

#### Tarefa 5.1: Domain Unit Tests

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Testes de VOs (validation, equality)
2. Testes de Agregados (invariantes, eventos)
3. Testes de Specifications

**Exemplo:**
```csharp
[Fact]
public void Agent_Create_ShouldRaiseDomainEvent()
{
    var agent = Agent.Create(...);
    Assert.NotEmpty(agent.UncommittedEvents);
}

[Fact]
public void AgentName_WithEmptyValue_ShouldFail()
{
    var name = new AgentName("");
    Assert.False(name.IsValid);
}
```

**Tempo estimado:** 8 horas

**Dependências:** Nenhuma - podem ser feitos agora

---

#### Tarefa 5.2: Repository Integration Tests

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Testes E2E com banco de dados
2. Testes de Specifications
3. Testes de UnitOfWork

**Tempo estimado:** 8 horas

**Dependências:** Tarefa 3.2

---

#### Tarefa 5.3: Application Tests

**Status:** ⏳ Não iniciado

**O que fazer:**
1. Testes de Commands (Handler)
2. Testes de EventHandlers
3. Testes de Queries

**Tempo estimado:** 6 horas

**Dependências:** Tarefa 4.3

---

## 📊 Timeline Geral

```
Semana 1:  Fase 2 (Refactor BCs)       [4 dias úteis]
Semana 2:  Fase 3 (Infrastructure)     [5 dias úteis]
Semana 3:  Fase 4 (Application)        [5 dias úteis]
Semana 4:  Fase 5 (Testing)            [4 dias úteis]
---------
Total: 4 semanas / 18 dias úteis
```

---

## 🚦 Checklist Detalhado

### Fase 2: Refactor BCs

- [ ] 2.1 Capabilities BC
  - [ ] Plugin Aggregate
  - [ ] McpConnection Aggregate
  - [ ] ValueObjects
  - [ ] Events
  - [ ] Repository
  
- [ ] 2.2 Documents BC
  - [ ] Document Aggregate (refactored)
  - [ ] ValueObjects (moved)
  - [ ] Events
  - [ ] Repository
  
- [ ] 2.3 Accounts BC
  - [ ] Account Aggregate
  - [ ] ValueObjects (moved)
  - [ ] Events
  - [ ] Repository
  
- [ ] 2.4 Subscriptions BC
  - [ ] AccountSubscription Aggregate
  - [ ] ValueObjects
  - [ ] Events
  - [ ] Repository

---

### Fase 3: Infrastructure

- [ ] 3.1 Database
  - [ ] DbContext created
  - [ ] Configurations for each Aggregate
  - [ ] Migrations generated
  - [ ] Database schema created
  
- [ ] 3.2 Repositories
  - [ ] All IRepository<T> implementations
  - [ ] SpecificationEvaluator
  - [ ] UnitOfWork implementation

---

### Fase 4: Application

- [ ] 4.1 Event Publishing
  - [ ] IDomainEventPublisher
  - [ ] MediatR integration
  - [ ] Events published after Commit
  
- [ ] 4.2 Domain Event Handlers
  - [ ] Handler for each Domain Event
  - [ ] Notifications/Reactions implemented
  
- [ ] 4.3 Application Handlers
  - [ ] All Commands updated
  - [ ] All Queries updated
  - [ ] Using Aggregates + Repositories
  
- [ ] 4.4 Validations
  - [ ] Notifications structured
  - [ ] Error responses standardized

---

### Fase 5: Testing

- [ ] 5.1 Domain Tests
  - [ ] VO tests (all)
  - [ ] Aggregate tests (all)
  - [ ] Specification tests
  
- [ ] 5.2 Repository Tests
  - [ ] CRUD operations
  - [ ] Specifications
  - [ ] UnitOfWork
  
- [ ] 5.3 Application Tests
  - [ ] Command handlers
  - [ ] Event handlers
  - [ ] Queries

---

## 🎯 Dependências Entre Tarefas

```
2.1 Capabilities     ────┐
2.2 Documents       ────┤
2.3 Accounts        ────┼──→ 3.1 DbContext ──→ 3.2 Repositories ──→ 4.1-4.3 Application
2.4 Subscriptions   ────┘
                          
5.1 Domain Tests (independent) ────→ Can run anytime
5.2 Repository Tests ────→ After 3.2
5.3 Application Tests ────→ After 4.3
```

---

## 💡 Dicas de Implementação

### 1. Não fazer tudo de uma vez

Fazer por BC:
1. Refactor BC
2. Criar Repository
3. Implementar Repository
4. Criar Command Handler
5. Testar

### 2. Manter compatibilidade temporária

Durante transição, manter velhos Controllers/Services funcionando:
```csharp
// Novo jeito (DDD)
var agent = Agent.Create(...);
await _repository.AddAsync(agent);

// Velho jeito (Legacy) - gradualmente remover
var agent = new AgentEntity { ... };
_dbContext.Agents.Add(agent);
```

### 3. Use Feature Flags (opcional)

```csharp
if (_config.UseDDDImplementation)
{
    // New DDD code
    var agent = Agent.Create(...);
}
else
{
    // Old code
    var agent = new AgentEntity { ... };
}
```

### 4. Commit por BC

```
commit: refactor(agents): implement DDD aggregate and repository
commit: refactor(sessions): implement DDD aggregate and repository
commit: refactor(pipelines): implement DDD aggregate and repository
```

---

## 📚 Recursos para Referência

1. **DDD_ARCHITECTURE.md** - Explicação detalhada de cada padrão
2. **DDD_STRUCTURE.md** - Estrutura completa de pastas
3. **DDD_CHEATSHEET.md** - Código pronto para copiar/colar
4. **Este arquivo** - Roadmap e próximas ações

---

## 🚀 Como Começar Fase 2

### 1. Abrir uma feature branch

```bash
git checkout -b refactor/ddd-phase2-capabilities
```

### 2. Seguir template

Usar templates em `DDD_CHEATSHEET.md`:
- Aggregate Root Template
- Value Object Template
- Repository Interface Template
- Event Template

### 3. Copiar estrutura

```bash
mkdir -p Domain/Capabilities/Aggregates
mkdir -p Domain/Capabilities/ValueObjects
mkdir -p Domain/Capabilities/Events
mkdir -p Domain/Capabilities/Repositories
mkdir -p Domain/Capabilities/Specifications
```

### 4. Implementar por ordem

```
1. ValueObjects/
2. Aggregates/
3. Events/
4. Repositories/
5. Specifications/
```

### 5. Validar antes de commit

- [x] Compila sem errors
- [x] Todos VOs têm Equals + GetHashCode
- [x] Agregado tem Factory method
- [x] Eventos documentados
- [x] Repository interface clara

---

## ⚠️ Armadilhas Comuns

### ❌ Não fazer

- ❌ Colocar lógica de negócio em Infrastructure
- ❌ Deixar setters públicos em VOs
- ❌ Misturar agregados em um só
- ❌ Publicar eventos fora do agregado
- ❌ Usar exceptions para validação (usar Notifications)
- ❌ Queries complexas direto em Repository

### ✅ Fazer

- ✅ Factory methods estáticos
- ✅ VOs imutáveis
- ✅ Eventos gerados pelo agregado
- ✅ Validação via Notifications
- ✅ Specifications para queries complexas

---

## 📞 Support

**Dúvidas durante implementação?**

1. Consultar `DDD_ARCHITECTURE.md`
2. Consultar `DDD_CHEATSHEET.md`
3. Copiar template apropriado
4. Adaptar para seu contexto

---

**Status Geral:** 🟢 Ready for Phase 2  
**Última atualização:** 2026-07-09  
**Versão:** 1.0

🎉 **Você tem tudo que precisa para transformar sua Domain em 100% DDD profissional!**
