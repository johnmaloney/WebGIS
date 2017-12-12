using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeospatialAPI.ServiceBus
{
    public interface IQueueObserverClient
    {
        List<GeoMessage> Results { get; }
    }

    public class QueueObserverClient : IQueueObserverClient
    {
        #region Fields

        private readonly QueueClient tileClient;

        private readonly QueueClient analysisClient;

        private static List<GeoMessage> repository = new List<GeoMessage>();

        #endregion

        #region Properties

        public List<GeoMessage> Results
        {
            get
            {
                return repository;
            }
        }

        #endregion

        #region Methods

        public QueueObserverClient(QueueClientOptions options)
        {
            if (options.AppType == AppKeys.Responder)
            {
                tileClient = new QueueClient(options.ConnectionString, ServiceBusKeys.TileResult);

                analysisClient = new QueueClient(options.ConnectionString, ServiceBusKeys.AnalysisResult);
                RegisterOnMessageHandlerAndReceiveMessages();
            }
            else if (options.AppType == AppKeys.Requestor)
            { }
            else
            {
                throw new NotSupportedException($"The Queue named: {options.AppType} is not supported.");
            }
        }

        public void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            tileClient.RegisterMessageHandler(ProcessTileMessagesAsync, messageHandlerOptions);
            analysisClient.RegisterMessageHandler(ProcessAnalysisMessagesAsync, messageHandlerOptions);
        }

        public async Task ProcessTileMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
            await tileClient.CompleteAsync(message.SystemProperties.LockToken);
                        
            var geoMessage = JsonConvert.DeserializeObject<GeoMessage>(Encoding.UTF8.GetString(message.Body));

            geoMessage.ResultsReceived = DateTime.Now;
            repository.Add(geoMessage);
        }

        public async Task ProcessAnalysisMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
            await analysisClient.CompleteAsync(message.SystemProperties.LockToken);

            var geoMessage = JsonConvert.DeserializeObject<GeoMessage>(Encoding.UTF8.GetString(message.Body));

            geoMessage.ResultsReceived = DateTime.Now;
            repository.Add(geoMessage);
        }

        public Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
        #endregion
    }
}
