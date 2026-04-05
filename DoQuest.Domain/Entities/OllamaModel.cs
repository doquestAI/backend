using Flunt.Notifications;
using Flunt.Validations;

namespace DoQuest.Domain.Entities;

public class OllamaModel : Notifiable<Notification>
{
    private OllamaModel() { }

    public Guid Id { get; private set; }
    public string ModelId { get; private set; } = string.Empty;     // e.g., "nomic-embed-text"
    public string DisplayName { get; private set; } = string.Empty;
    public bool IsEmbeddingModel { get; private set; }
    public bool IsDefaultChat { get; private set; }
    public int ContextLength { get; private set; }

    public static OllamaModel Create(
        string modelId,
        string displayName,
        bool isEmbeddingModel,
        int contextLength,
        bool isDefaultChat = false)
    {
        var model = new OllamaModel();

        model.AddNotifications(new Contract<OllamaModel>()
            .Requires()
            .IsNotNullOrEmpty(modelId, nameof(ModelId), "ModelId é obrigatório")
            .IsNotNullOrEmpty(displayName, nameof(DisplayName), "DisplayName é obrigatório")
            .IsGreaterThan(contextLength, 0, nameof(ContextLength), "ContextLength deve ser > 0"));

        if (model.IsValid)
        {
            model.Id = Guid.NewGuid();
            model.ModelId = modelId;
            model.DisplayName = displayName;
            model.IsEmbeddingModel = isEmbeddingModel;
            model.IsDefaultChat = isDefaultChat;
            model.ContextLength = contextLength;
        }

        return model;
    }
}
