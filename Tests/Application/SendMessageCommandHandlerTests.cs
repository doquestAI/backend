using Application.Abstractions;
using Application.UseCases.Chat.Commands.SendMessage;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.AI;
using NSubstitute;

namespace Tests.Application;

internal sealed class SendMessageCommandHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IDocumentRepository _docRepo = Substitute.For<IDocumentRepository>();
    private readonly IEmbeddingService _embedding = Substitute.For<IEmbeddingService>();
    private readonly IChatAgentService _chat = Substitute.For<IChatAgentService>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private SendMessageCommandHandler CreateHandler() =>
        new(_userRepo, _docRepo, _embedding, _chat, _uow);

    private static User CreateUserWithPlan(int dailyLimit = 100)
    {
        var plan = Plan.Create(Plan.ProPlanId, PlanType.Pro, "Pro", dailyLimit, 500_000, "prod_pro");
        var user = User.Create("firebase_uid_test", "user@test.com", plan.Id);
        // Inject plan navigation property
        typeof(User).GetProperty("Plan")!
            .SetValue(user, plan, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, null, null);
        return user;
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        _userRepo.GetByFirebaseUidAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new SendMessageCommand("uid", "Qual é o tema de redação do ENEM?", null, []);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Notifications.Should().ContainSingle(n => n.Key == "User");
        await _chat.DidNotReceive().ChatAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserAndPlanValid_ShouldReturnSuccessWithReply()
    {
        // Arrange
        var user = CreateUserWithPlan(dailyLimit: 100);
        var embedding = new float[768];
        var expectedReply = "O ENEM avalia competências e habilidades...";

        _userRepo.GetByFirebaseUidAsync("uid_ok", Arg.Any<CancellationToken>()).Returns(user);
        _embedding.GenerateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(embedding);
        _docRepo.SearchSimilarChunksAsync(Arg.Any<float[]>(), Arg.Any<int>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(new List<global::Domain.Entities.DocumentChunk>().AsReadOnly() as IReadOnlyList<global::Domain.Entities.DocumentChunk>);
        _chat.ChatAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<CancellationToken>())
            .Returns(expectedReply);
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var command = new SendMessageCommand("uid_ok", "O que é o ENEM?", null, []);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Reply.Should().Be(expectedReply);
        result.Value.SourceChunksUsed.Should().Be(0);
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDailyLimitExceeded_ShouldReturnFailureWithoutCallingChat()
    {
        // Arrange — user already at limit (0 messages allowed)
        var user = CreateUserWithPlan(dailyLimit: 0);
        _userRepo.GetByFirebaseUidAsync("uid_limited", Arg.Any<CancellationToken>()).Returns(user);

        var command = new SendMessageCommand("uid_limited", "Pergunta", null, []);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Notifications.Should().ContainSingle(n => n.Key == "DailyMessageCount");
        await _chat.DidNotReceive().ChatAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<CancellationToken>());
        await _uow.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
