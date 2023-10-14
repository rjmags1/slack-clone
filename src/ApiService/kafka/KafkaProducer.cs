using Confluent.Kafka;

namespace ApiService.Kafka.Producer;

public class KafkaProducer
{
    private static readonly string MESSAGE_TOPIC = "dev-realtime-messages";
    private IProducer<string, string> Producer { get; set; }

    public KafkaProducer()
    {
        var builder = new ProducerBuilder<string, string>(
            new ProducerConfig { BootstrapServers = "localhost:9092" }
        );
        Producer = builder.Build();
    }

    ~KafkaProducer()
    {
        Producer.Flush();
        Producer.Dispose();
    }

    private static void ProduceFinishedHandler(
        DeliveryReport<string, string> deliveryReport
    )
    {
        if (deliveryReport.Error.Code != ErrorCode.NoError)
        {
            Console.WriteLine(
                $"Failed to deliver message: {deliveryReport.Error.Reason}"
            );
        }
        else
        {
            Console.WriteLine(
                $"Produced event to topic {deliveryReport.Topic}: key = {deliveryReport.Key} value = {deliveryReport.Value} at {deliveryReport.Timestamp}"
            );
        }
    }

    public void ProduceMessageEvent(string key, string value)
    {
        Producer.Produce(
            MESSAGE_TOPIC,
            new Message<string, string>
            {
                Key = key,
                Value = value,
                Timestamp = Timestamp.Default
            },
            ProduceFinishedHandler
        );
    }
}
