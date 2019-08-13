using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace SB.DeadLetter
{

    public class Program
    {
        static async Task ClearDeadLetterQueue(string connectionString, string topicName, string subscriptionName)
        {
            // For picking up the message from a DLQ, we make a receiver just like for a 
            // regular queue. We could also use QueueClient and a registered handler here. 
            // The required path is constructed with the EntityNameHelper.FormatDeadLetterPath() 
            // helper method, and always follows the pattern "{entity}/$DeadLetterQueue", 
            // meaning that for a queue "Q1", the path is "Q1/$DeadLetterQueue" and for a 
            // topic "T1" and subscription "S1", the path is "T1/Subscriptions/S1/$DeadLetterQueue" 

            var deadLetterQueuePath = $"{topicName}/Subscriptions/{subscriptionName}/$DeadLetterQueue";
            var deadletterReceiver = new MessageReceiver(connectionString, deadLetterQueuePath, ReceiveMode.PeekLock);
            while (true)
            {
                // receive a message
                var msg = await deadletterReceiver.ReceiveAsync(TimeSpan.FromSeconds(10));
                if (msg != null)
                {
                    // write out the message and its user properties
                    Console.WriteLine("Deadletter message:");
                    foreach (var prop in msg.UserProperties)
                    {
                        Console.WriteLine("{0}={1}", prop.Key, prop.Value);
                    }
                    // complete and therefore remove the message from the DLQ
                    await deadletterReceiver.CompleteAsync(msg.SystemProperties.LockToken);
                }
                else
                {
                    // DLQ was empty on last receive attempt
                    break;
                }
            }
        }
        static async Task MainAsync()
        {
            try
            {
                string connectionString = "Endpoint=sb://saeed-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Xd4t5V4L0Ia7YTmsB++hVcUzpwEG7Jedxab6qpQJEE8=";
                string topicName = "saeed-topic";
                string subscriptionName = "saeed-subscription";
                Console.WriteLine("Clear Dead Letter Queue");
                await ClearDeadLetterQueue(connectionString, topicName, subscriptionName);
                Console.WriteLine("Clear Dead Letter Queue Done!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static void Main(string[] args)
        {
            Console.WriteLine("start clear dead letter queue? ([yes], no)");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input == "yes")
            {
                MainAsync().GetAwaiter().GetResult();
            }
        }
    }
}
