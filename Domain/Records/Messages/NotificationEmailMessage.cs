namespace Domain.Messages;

internal record NotificationEmailMessage(
    string RecipientEmail,
    string RecipientName,
    string Subject,
    string Content,
    bool IsHtmlContent = true,
    string? SenderName = null,
    string? SenderEmail = null);