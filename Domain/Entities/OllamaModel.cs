using Domain.Entities.Abstracts;
using Domain.ValueObjects;

namespace Domain.Entities;

internal class OllamaModel : Entity
{
    private OllamaModel() { }

    public ModelIdentifier ModelId { get; private set; } = null!;
    public DisplayName DisplayName { get; private set; } = null!;
    public bool IsEmbeddingModel { get; private set; }
    public bool IsDefaultChat { get; private set; }
    public ContextLength ContextLength { get; private set; } = null!;

    public static OllamaModel Create(
        string modelId,
        string displayName,
        bool isEmbeddingModel,
        int contextLength,
        bool isDefaultChat = false)
    {
        var model = new OllamaModel();

        var modelIdVo = new ModelIdentifier(modelId);
        var displayNameVo = new DisplayName(displayName);
        var contextLengthVo = new ContextLength(contextLength);

        model.AddNotificationsFromValueObjects(modelIdVo, displayNameVo, contextLengthVo);

        if (model.IsValid)
        {
            model.ModelId = modelIdVo;
            model.DisplayName = displayNameVo;
            model.IsEmbeddingModel = isEmbeddingModel;
            model.IsDefaultChat = isDefaultChat;
            model.ContextLength = contextLengthVo;
        }

        return model;
    }
}