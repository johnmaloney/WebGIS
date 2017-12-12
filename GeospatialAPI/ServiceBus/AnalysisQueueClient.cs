using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeospatialAPI.ServiceBus
{
    public interface IAnalysisQueueClient
    {
        Task SendMessages(GeoMessage tile);
    }

    public class AnalysisQueueClient
    {
        #region Fields

        private readonly QueueClient analysisResponseClient;
        private readonly QueueClient analysisResultClient;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public AnalysisQueueClient(QueueClientOptions options)
        {
            analysisResponseClient = new QueueClient(options.ConnectionString, ServiceBusKeys.AnalysisQuery);

            if (options.AppType == AppKeys.Responder)
            {
                analysisResultClient = new QueueClient(options.ConnectionString, ServiceBusKeys.AnalysisResult);
                // Register QueueClient's MessageHandler and receive messages in a loop
                RegisterOnMessageHandlerAndReceiveMessages();
            }
            else if (options.AppType == AppKeys.Requestor)
            { }
            else
            {
                throw new NotSupportedException($"The Queue named: {options.AppType} is not supported.");
            }
        }

        public async Task SendMessages(GeoMessage tile)
        {
            try
            {
                // Create a new message to send to the queue
                string messageBody = JsonConvert.SerializeObject(tile);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                message.To = tile.ClientName;
                message.CorrelationId = tile.SessionId;
                // Send the message to the queue
                await analysisResponseClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
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
            analysisResponseClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        public async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            var geoMessage = JsonConvert.DeserializeObject<GeoMessage>(Encoding.UTF8.GetString(message.Body));

            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
            await analysisResponseClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.

            Task.Delay(geoMessage.LatencyTime).Wait();

            string messageBody = JsonConvert.SerializeObject(
                new GeoMessage
                {
                    ClientName = geoMessage.ClientName,
                    SessionId = geoMessage.SessionId,
                    Result = this.sampleGeoJSON(geoMessage)
                });
            var resultMessage = new Message(Encoding.UTF8.GetBytes(messageBody));
            // Assign a SessionId for the message
            resultMessage.CorrelationId = message.CorrelationId;
            resultMessage.ContentType = AppKeys.Analysis;
            await analysisResultClient.SendAsync(resultMessage);
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

        private string sampleGeoJSON(GeoMessage tile)
        {
            return $"Geospatial Analysis conmpleted for {tile.ClientName}";
        }

        #endregion
    }
}
