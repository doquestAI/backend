using Google.Cloud.PubSub.V1;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.GoogleCloud.PubSub.Subscribers.Factories;

internal sealed class SubscriberFactory(IOptions<GoogleCloudSettings> options)
{
    private readonly GoogleCloudSettings _settings = options.Value;

    public SubscriberClient Create(string subscriptionId)
    {
        var path = SubscriptionName.FromProjectSubscription(
            _settings.ProjectId,
            subscriptionId);

        return SubscriberClient.Create(path);
    }
}