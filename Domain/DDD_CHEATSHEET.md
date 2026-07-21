# 🚀 DDD Cheat Sheet - Referência Rápida

## Criar novo Bounded Context

```bash
# Estrutura mínima
MyBoundedContext/
├── Aggregates/
│   └── MyAggregate.cs
├── ValueObjects/
│   └── MyId.cs
├── Events/
│   └── MyAggregateCreatedEvent.cs
├── Repositories/
│   └── IMyRepository.cs
└── Specifications/
    └── MySpecification.cs
```

---

## Aggregate Root Template

```csharp
using Domain.Shared.Core;

namespace Domain.MyBC.Aggregates;

/// <summary>
/// AGGREGATE ROOT: MyAggregate
/// Descrição...
/// </summary>
public sealed class MyAggregate : AggregateRoot
{
    public MyId MyId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public MyStatus Status { get; private set; }

    private MyAggregate() { }

    public static MyAggregate Create(string name)
    {
        var aggregate = new MyAggregate
        {
            Id = Guid.NewGuid(),
            MyId = MyId.New(),
            Name = name,
            Status = MyStatus.Active,
        };

        aggregate.AddNotifications(...); // Valida VOs
        
        if (aggregate.IsValid)
            aggregate.RaiseDomainEvent(new MyAggregateCreatedEvent(aggregate.Id));

        return aggregate;
    }

    public void DoSomething()
    {
        // Lógica de negócio
        RaiseDomainEvent(new MyAggregateDidSomethingEvent(Id));
    }
}
```

---

## Value Object Template

```csharp
using Domain.Shared.Core;

namespace Domain.MyBC.ValueObjects;

/// <summary>
/// Descrição do VO...
/// </summary>
public sealed class MyId : ValueObject
{
    public Guid Value { get; }

    public MyId(Guid value)
    {
        if (value == Guid.Empty)
            AddNotification(nameof(MyId), "MyId cannot be empty");
        Value = value;
    }

    public static MyId New() => new(Guid.NewGuid());

    public override bool Equals(object? obj) =>
        obj is MyId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
```

---

## Domain Event Template

```csharp
using Domain.Shared.Core;

namespace Domain.MyBC.Aggregates;

public sealed record MyAggregateCreatedEvent(Guid AggregateId, string Name)
    : DomainEvent(AggregateId)
{
    public string EventType => "my_aggregate.created";
}

public sealed record MyAggregateDidSomethingEvent(Guid AggregateId)
    : DomainEvent(AggregateId)
{
    public string EventType => "my_aggregate.did_something";
}
```

---

## Repository Interface Template

```csharp
using Domain.MyBC.Aggregates;
using Domain.Shared.Repositories;

namespace Domain.MyBC.Repositories;

public interface IMyAggregateRepository : IRepository<MyAggregate>
{
    Task<MyAggregate?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyList<MyAggregate>> GetActiveAsync(CancellationToken ct = default);
}
```

---

## Specification Template

```csharp
using Domain.MyBC.Aggregates;
using Domain.Shared.Core;

namespace Domain.MyBC.Specifications;

public sealed class ActiveAggregatesSpecification : Specification<MyAggregate>
{
    public ActiveAggregatesSpecification()
    {
        Criteria = a => a.Status == MyStatus.Active && a.DeletedAt == null;
        // OrderBy = a => a.Name;
        // ApplyPaging(skip: 0, take: 50);
    }
}
```

---

## Repository Implementation Template (Infrastructure)

```csharp
using Domain.MyBC.Aggregates;
using Domain.MyBC.Repositories;
using Domain.Shared.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.MyBC.Repositories;

public class MyAggregateRepository : IMyAggregateRepository
{
    private readonly MyDbContext _context;

    public MyAggregateRepository(MyDbContext context) => _context = context;

    public async Task<MyAggregate?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.MyAggregates.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<MyAggregate?> GetByNameAsync(string name, CancellationToken ct)
        => await _context.MyAggregates.FirstOrDefaultAsync(a => a.Name == name, ct);

    public async Task<IReadOnlyList<MyAggregate>> ListAsync(CancellationToken ct)
        => await _context.MyAggregates.ToListAsync(ct);

    public async Task<IReadOnlyList<MyAggregate>> ListAsync(Specification<MyAggregate> spec, CancellationToken ct)
    {
        var query = ApplySpecification(spec);
        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(MyAggregate entity, CancellationToken ct)
    {
        await _context.MyAggregates.AddAsync(entity, ct);
    }

    public Task UpdateAsync(MyAggregate entity, CancellationToken ct)
    {
        _context.MyAggregates.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(MyAggregate entity, CancellationToken ct)
    {
        _context.MyAggregates.Remove(entity);
        return Task.CompletedTask;
    }

    private IQueryable<MyAggregate> ApplySpecification(Specification<MyAggregate> spec)
    {
        var query = _context.MyAggregates.AsQueryable();

        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
            query = query.Include(include);

        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);

        if (spec.IsPagingEnabled && spec.Skip.HasValue && spec.Take.HasValue)
            query = query.Skip(spec.Skip.Value).Take(spec.Take.Value);

        return query;
    }
}
```

---

## Domain Event Handler Template (Application)

```csharp
using Domain.MyBC.Aggregates;
using MediatR;

namespace Application.EventHandlers.MyBC;

public class MyAggregateCreatedEventHandler : INotificationHandler<MyAggregateCreatedEvent>
{
    private readonly IMyService _myService;
    private readonly ILogger<MyAggregateCreatedEventHandler> _logger;

    public MyAggregateCreatedEventHandler(IMyService myService, ILogger<MyAggregateCreatedEventHandler> logger)
    {
        _myService = myService;
        _logger = logger;
    }

    public async Task Handle(MyAggregateCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling event: {EventType}", notification.EventType);
        
        // Reagir ao evento
        await _myService.ProcessAsync(notification.AggregateId, cancellationToken);
    }
}
```

---

## Command Handler Template (Application)

```csharp
using Application.Commands.MyBC;
using Domain.MyBC.Aggregates;
using Domain.MyBC.Repositories;
using Domain.MyBC.ValueObjects;
using Domain.Shared.Repositories;
using MediatR;

namespace Application.CommandHandlers.MyBC;

public class CreateMyAggregateCommandHandler : IRequestHandler<CreateMyAggregateCommand, CreateMyAggregateResponse>
{
    private readonly IMyAggregateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateMyAggregateCommandHandler(
        IMyAggregateRepository repository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<CreateMyAggregateResponse> Handle(CreateMyAggregateCommand request, CancellationToken ct)
    {
        // Criar agregado
        var aggregate = MyAggregate.Create(request.Name);

        // Validar
        if (!aggregate.IsValid)
            return new CreateMyAggregateResponse(false, string.Join("; ", aggregate.Notifications.Select(n => n.Message)));

        // Persistir
        await _repository.AddAsync(aggregate, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Publicar eventos
        foreach (var evt in aggregate.UncommittedEvents)
            await _mediator.Publish(evt, ct);

        await _unitOfWork.CommitAsync(ct);

        return new CreateMyAggregateResponse(true, "Success");
    }
}
```

---

## Patterns Rápidos

### Value Object com Enum

```csharp
public sealed class MyStatus : ValueObject
{
    public Status Value { get; }

    private MyStatus(Status value) => Value = value;

    public static MyStatus Active => new(Status.Active);
    public static MyStatus Inactive => new(Status.Inactive);

    public override bool Equals(object? obj) =>
        obj is MyStatus other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}

public enum Status { Active, Inactive }
```

### Value Object Immutable

```csharp
public sealed class TokenMetrics
{
    public long InputTokens { get; }
    public long OutputTokens { get; }
    public long TotalTokens => InputTokens + OutputTokens;

    public TokenMetrics(long inputTokens = 0, long outputTokens = 0)
    {
        InputTokens = inputTokens;
        OutputTokens = outputTokens;
    }

    // Retorna NOVO objeto (não modifica this)
    public TokenMetrics Add(TokenMetrics other) =>
        new(InputTokens + other.InputTokens, OutputTokens + other.OutputTokens);
}
```

### Specification com Paging

```csharp
public sealed class PagedSpecification : Specification<MyAggregate>
{
    public PagedSpecification(int pageNumber, int pageSize)
    {
        Criteria = a => a.DeletedAt == null;
        OrderBy = a => a.Name;
        ApplyPaging(skip: (pageNumber - 1) * pageSize, take: pageSize);
    }
}
```

### Soft Delete

```csharp
public sealed class ActiveAggregatesSpecification : Specification<MyAggregate>
{
    public ActiveAggregatesSpecification()
    {
        // IMPORTANTE: Sempre filtrar DeletedAt == null
        Criteria = a => a.DeletedAt == null;
    }
}

// No agregado:
public void Delete()
{
    SoftDelete();  // Herdado de Entity
    RaiseDomainEvent(new MyAggregateDeletedEvent(Id));
}
```

---

## DI Registration Template

```csharp
// Infrastructure/DI/DependencyInjection.cs
using Domain.MyBC.Repositories;
using Infrastructure.MyBC.Repositories;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBCServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IMyAggregateRepository, MyAggregateRepository>();

        // Event Handlers
        services.AddScoped<INotificationHandler<MyAggregateCreatedEvent>, MyAggregateCreatedEventHandler>();

        return services;
    }
}

// Program.cs
services.AddMyBCServices();
```

---

## Validação Padrão

```csharp
// NO VALUE OBJECT
public sealed class AgentName : ValueObject
{
    public string Value { get; private set; } = null!;

    public AgentName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddNotification(nameof(AgentName), "Agent name cannot be empty");
            return;  // ← IMPORTANTE: retorna após erro
        }

        if (value.Length > 256)
        {
            AddNotification(nameof(AgentName), "Agent name cannot exceed 256 characters");
            return;
        }

        Value = value;  // ← Só atribui se válido
    }
}

// NO AGREGADO
public static MyAggregate Create(AgentName name, AgentDescription desc)
{
    var agg = new MyAggregate { ... };

    // Valida VOs filhos
    agg.AddNotificationsFromValueObjects(name, desc);

    if (!agg.IsValid)
        return agg;  // Retorna inválido

    agg.RaiseDomainEvent(...);
    return agg;
}
```

---

## Comparação de VOs

```csharp
var name1 = new AgentName("John");
var name2 = new AgentName("John");
var name3 = new AgentName("Jane");

// VOs são iguais POR VALOR, não por referência
Assert.Equal(name1, name2);  // ✅ True (mesmo valor)
Assert.NotEqual(name1, name3);  // ✅ True (valores diferentes)
```

---

## Invariantes de Negócio

```csharp
public sealed class Agent : AggregateRoot
{
    private Agent() { }

    public static Agent Create(...)
    {
        var agent = new Agent { ... };

        // INVARIANTE: Name é único
        // (Verificado em IAgentRepository.GetByNameAsync antes de Create)

        // INVARIANTE: SystemPrompt ≤ 32k
        // (Validado no VO AgentSystemPrompt)

        // INVARIANTE: Temperature ∈ [0, 2]
        if (temperature < 0 || temperature > 2)
            agent.AddNotification(...);

        // INVARIANTE: Apenas IsValid gera evento
        if (agent.IsValid)
            agent.RaiseDomainEvent(...);

        return agent;
    }

    public void RecordInvocation(long inputTokens, long outputTokens)
    {
        // INVARIANTE: Tokens ≥ 0
        if (inputTokens < 0 || outputTokens < 0)
        {
            AddNotification(...);
            return;
        }

        // INVARIANTE: Status deve ser Running (ou lança erro em chamador)
        Metrics.RecordInvocation(inputTokens, outputTokens, duration);
    }

    public void Pause()
    {
        // INVARIANTE: Apenas Active pode pausar
        if (State != SessionState.Active)
        {
            AddNotification(...);
            return;
        }

        State = SessionState.Paused;
        RaiseDomainEvent(new SessionPausedEvent(Id, SessionId.Value));
    }
}
```

---

## Testing Patterns

### Unit Test - Value Object

```csharp
[Fact]
public void AgentName_WithValidValue_ShouldCreate()
{
    var name = new AgentName("QuestionGenerator");

    Assert.True(name.IsValid);
    Assert.Equal("QuestionGenerator", name.Value);
}

[Fact]
public void AgentName_WithEmptyValue_ShouldFail()
{
    var name = new AgentName("");

    Assert.False(name.IsValid);
    Assert.Single(name.Notifications);
}

[Fact]
public void AgentName_Equality_ShouldWorkByValue()
{
    var name1 = new AgentName("John");
    var name2 = new AgentName("John");

    Assert.Equal(name1, name2);  // Por valor, não referência
}
```

### Unit Test - Aggregate

```csharp
[Fact]
public void Agent_Create_ShouldRaiseDomainEvent()
{
    var agent = Agent.Create(
        new AgentName("TestAgent"),
        new AgentDescription("Test"),
        AgentRole.Generator,
        new AgentSystemPrompt("Test")
    );

    Assert.True(agent.IsValid);
    Assert.NotEmpty(agent.UncommittedEvents);
    Assert.IsType<AgentCreatedEvent>(agent.UncommittedEvents.First());
}

[Fact]
public void Agent_RecordInvocation_ShouldUpdateMetrics()
{
    var agent = Agent.Create(...);
    var initialTokens = agent.Metrics.TotalTokens;

    agent.RecordInvocation(inputTokens: 100, outputTokens: 50, duration: TimeSpan.FromMs(100));

    Assert.Equal(150, agent.Metrics.TotalTokens - initialTokens);
}
```

---

## Debugging Tips

```csharp
// Verificar notificações
if (!aggregate.IsValid)
{
    foreach (var notification in aggregate.Notifications)
    {
        Console.WriteLine($"[{notification.Key}] {notification.Message}");
    }
}

// Verificar eventos
foreach (var evt in aggregate.UncommittedEvents)
{
    Console.WriteLine($"Event: {evt.GetType().Name} @ {evt.OccurredAt}");
}

// Verificar estado
Console.WriteLine($"Status: {agent.Status}");
Console.WriteLine($"Metrics: {agent.Metrics.TotalTokens} tokens");
```

---

## Erros Comuns

### ❌ Público Setter em VO

```csharp
public sealed class AgentName : ValueObject
{
    public string Value { get; set; }  // ❌ ERRADO! Não é imutável
}
```

### ✅ Correto

```csharp
public sealed class AgentName : ValueObject
{
    public string Value { get; }  // ✅ Apenas getter
}
```

---

### ❌ Agregado com múltiplas raízes

```csharp
// ❌ Errado - dois agregados misturados
public class Agent : AggregateRoot
{
    public AgentSession Session { get; set; }  // ❌ Outro agregado!
}
```

### ✅ Correto

```csharp
// ✅ Certo - agent referencia apenas ID de session
public class Agent : AggregateRoot
{
    public Guid? SessionId { get; set; }  // ✅ Apenas referência
}
```

---

### ❌ Evento sem agregado

```csharp
// ❌ Evento criado fora do agregado
var evt = new AgentCreatedEvent(agent.Id);
await _eventPublisher.PublishAsync(evt);
```

### ✅ Correto

```csharp
// ✅ Evento criado pelo agregado
agent.RaiseDomainEvent(new AgentCreatedEvent(agent.Id));
// Publicado após SaveChanges + Commit
```

---

## Links Úteis

- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Patterns, Principles, Practices of DDD - Millett & Tune](https://vaughnvernon.com/wp-content/uploads/2015/08/IDDD_Patterns_Principles_Practices_of_Domain-Driven_Design.pdf)
- [CQRS - Greg Young](https://www.youtube.com/watch?v=JHGkaShoyNs)
- [Microsoft DDD Guide](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)

---

**Versão:** 1.0  
**Última atualização:** 2026-07-09  
**Status:** ✅ Completo
