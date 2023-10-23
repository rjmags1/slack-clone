using Microsoft.AspNetCore.SignalR;
using Confluent.Kafka;
using RealtimeService.Hubs;

namespace RealtimeService.Kafka.Consumer;

public class KafkaConsumer
{
    public static readonly string MESSAGE_TOPIC = "dev-realtime-messages";
    private readonly IConsumer<string, string> Consumer;
    private readonly IHubContext<SlackCloneHub> HubContext;

    public KafkaConsumer(IHubContext<SlackCloneHub> hubContext)
    {
        var builder = new ConsumerBuilder<string, string>(
            new ConsumerConfig
            {
                AutoOffsetReset = AutoOffsetReset.Latest,
                BootstrapServers = "localhost:9092",
                GroupId = "only-group"
            }
        );
        Consumer = builder.Build();
        HubContext = hubContext;
    }

    ~KafkaConsumer()
    {
        Consumer.Dispose();
    }

    public void InitConsumer(IEnumerable<string> topics)
    {
        Consumer.Subscribe(topics);
        var cancellationToken = (new CancellationTokenSource()).Token;
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = Consumer.Consume(cancellationToken);

                    // TODO: use hubContext to send message to appropriate group or person
                    //       based on consumed event message payload

                    Console.WriteLine(
                        @$"Consumed event from topic {consumeResult.Topic}: 
    key {consumeResult.Message.Key} 
    value {consumeResult.Message.Value}
    at {consumeResult.Message.Timestamp}
"
                    );
                    var key = consumeResult.Message.Key;
                    var message = consumeResult.Message.Value;
                    var workspaceSigninId = message.Substring(10);
                    await HubContext.Clients
                        .Group(workspaceSigninId)
                        .SendAsync("receiveMessage", key, message);
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine(e);
                    break;
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine(
                        $"Error while consuming: {e.Error.Reason}"
                    );
                }
            }
            Console.WriteLine("CLOSING KAFKA CONSUMER");
            Consumer.Close();
        });
    }
}
