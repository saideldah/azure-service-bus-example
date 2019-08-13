using Microsoft.Azure.ServiceBus.Management;
using System.Threading.Tasks;

namespace SB.Manegment
{
    class Program
    {
        const string serviceBusConnectionString = "Endpoint=sb://saeed-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Xd4t5V4L0Ia7YTmsB++hVcUzpwEG7Jedxab6qpQJEE8=";
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task<NamespaceInfo> MainAsync()
        {
            var client = new ManagementClient(serviceBusConnectionString);
            var taskOfNamespaceInfo = await client.GetNamespaceInfoAsync();
            var topics = await client.GetTopicsAsync();
            foreach (var topic in topics)
            {
                var subscriptions = await client.GetSubscriptionsAsync(topic.Path);
                foreach (var subscription in subscriptions)
                {
                    var deadLetterQueuePath = $"{topic.Path}/Subscriptions/{subscription.SubscriptionName}/$DeadLetterQueue";
                    System.Console.WriteLine(deadLetterQueuePath);
                }
            }
            return taskOfNamespaceInfo;
        }
    }
}
