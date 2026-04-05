using DoQuest.Domain.Entities;
using DoQuest.Domain.Enums;
using FluentAssertions;

namespace DoQuest.Tests.Domain;

public sealed class UserPlanLimitTests
{
    private static Plan CreateFreePlan() =>
        Plan.Create(
            Plan.FreePlanId,
            PlanType.Free,
            "Free",
            dailyMessageLimit: 10,
            dailyTokenLimit: 50_000,
            stripeProductId: "prod_free");

    private static Plan CreateMaxPlan() =>
        Plan.Create(
            Plan.MaxPlanId,
            PlanType.Max,
            "Max",
            dailyMessageLimit: -1,
            dailyTokenLimit: -1,
            stripeProductId: "prod_max");

    [Fact]
    public void ConsumeMessage_WhenUnderLimit_ShouldSucceed()
    {
        // Arrange
        var plan = CreateFreePlan();
        plan.IsValid.Should().BeTrue();

        var user = User.Create("uid_test_1", "test@test.com", plan.Id);
        // Inject plan via reflection (EF would normally load it)
        typeof(User).GetProperty("Plan")!
            .SetValue(user, plan, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, null, null);

        // Act
        user.ConsumeMessage();

        // Assert
        user.IsValid.Should().BeTrue("first message should be within Free plan limit of 10");
        user.DailyMessageCount.Should().Be(1);
    }

    [Fact]
    public void ConsumeMessage_WhenLimitReached_ShouldAddNotification()
    {
        // Arrange
        var plan = CreateFreePlan(); // limit = 10
        var user = User.Create("uid_test_2", "test2@test.com", plan.Id);
        typeof(User).GetProperty("Plan")!
            .SetValue(user, plan, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, null, null);

        // Simulate 10 messages already sent today
        for (var i = 0; i < 10; i++)
        {
            user.ConsumeMessage();
            // Clear notifications after each successful call so IsValid stays true
            typeof(DoQuest.Domain.Entities.User)
                .GetField("_notifications", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .SetValue(user, new System.Collections.Generic.List<Flunt.Notifications.Notification>());
        }

        // Act — 11th message should fail
        user.ConsumeMessage();

        // Assert
        user.IsInvalid.Should().BeTrue("11th message exceeds Free plan limit of 10");
        user.Notifications.Should().ContainSingle(n => n.Key == "DailyMessageCount");
    }

    [Fact]
    public void ConsumeMessage_WhenMaxPlan_ShouldAlwaysSucceed()
    {
        // Arrange
        var plan = CreateMaxPlan(); // limit = -1 (unlimited)
        var user = User.Create("uid_test_3", "test3@test.com", plan.Id);
        typeof(User).GetProperty("Plan")!
            .SetValue(user, plan, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, null, null);

        // Act — send 100 messages
        for (var i = 0; i < 100; i++)
        {
            user.ConsumeMessage();
        }

        // Assert
        user.IsValid.Should().BeTrue("Max plan has no message limit");
        user.DailyMessageCount.Should().Be(100);
    }

    [Fact]
    public void Plan_CanSendMessage_WhenUnlimited_ShouldReturnTrue()
    {
        var plan = CreateMaxPlan();
        plan.CanSendMessage(9999).Should().BeTrue();
    }

    [Fact]
    public void Plan_CanSendMessage_WhenAtLimit_ShouldReturnFalse()
    {
        var plan = CreateFreePlan(); // limit = 10
        plan.CanSendMessage(10).Should().BeFalse("count equals limit");
        plan.CanSendMessage(9).Should().BeTrue("count is below limit");
    }
}
