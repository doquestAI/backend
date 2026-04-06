using Domain.Constants;
using Flunt.Notifications;

namespace Domain.ValueObjects;

internal class Embedding : BaseValueObject
{
    public float[] Values { get; private set; } = [];

    protected Embedding() { }

    public Embedding(float[] values)
    {
        if (values == null || values.Length == 0)
            AddNotification(new Notification(nameof(Values), "Embedding é obrigatório"));
        else if (values.Length != ModelConstants.EmbeddingDimension)
            AddNotification(new Notification(nameof(Values),
                $"Embedding deve ter {ModelConstants.EmbeddingDimension} dimensões, mas recebeu {values.Length}"));

        if (IsValid)
            Values = values!;
    }

    public int Dimension => Values.Length;
}
