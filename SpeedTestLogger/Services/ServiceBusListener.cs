using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;

namespace SpeedTestLogger.Services
{
    public class ServiceBusListener
    {
        private readonly SubscriptionClient _subscriptionClient;

        public ServiceBusListener(LoggerConfiguration config)
        {
            _subscriptionClient = new SubscriptionClient(
                config.ServiceBusConnectionString,
                config.ServiceBusTopic,
                config.ServiceBusSubscriptionName);
        }

        public async Task Listen(Func<Message, Task> receivedAction)
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(HandleException)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            _subscriptionClient.RegisterMessageHandler(async (m, c) => {
                await receivedAction(m);
                await _subscriptionClient.CompleteAsync(m.SystemProperties.LockToken);
            }, messageHandlerOptions);

            while (true)
            {
                await Task.Delay(100);
            }
        }

        private async Task HandleException(ExceptionReceivedEventArgs args)
        {
            await Task.Run(() => Console.WriteLine(args.Exception.Message));
        }
    }
}
