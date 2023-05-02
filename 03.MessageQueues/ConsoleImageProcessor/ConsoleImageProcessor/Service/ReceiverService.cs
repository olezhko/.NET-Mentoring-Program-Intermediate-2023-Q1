using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using ConsoleImageProcessor.Models;

namespace ConsoleImageProcessor.Service
{
    public interface IReceiverService
    {
        public void Start(CancellationToken token);
    }

    public class ReceiverService : IReceiverService
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _storageFolder;
        public ReceiverService(string folder)
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "results",
                durable: true,
                exclusive: false,
                autoDelete: false);

            _storageFolder = folder;

            if (!Directory.Exists(_storageFolder))
            {
                Directory.CreateDirectory(_storageFolder);
            }
        }

        public void Start(CancellationToken token)
        {
            // create a consumer that listens on the channel (queue)
            var consumer = new EventingBasicConsumer(_channel);

            // handle the Received event on the consumer
            // this is triggered whenever a new message
            // is added to the queue by the producer
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await Task.Run( async () =>
                {
                    var item = JsonConvert.DeserializeObject<RabbitMessage>(message);
                    await CollectFile(item);
                }, token);
            };

            _channel.BasicConsume(queue: "results", autoAck: true, consumer: consumer);
            Console.WriteLine("ReceiverService started");
        }

        private async Task CollectFile(RabbitMessage item)
        {
            string filePath = Path.Combine(_storageFolder, item.Filename);
            if (item.DataBytes.Length == item.FullSize)
            {
                await File.WriteAllBytesAsync(filePath, item.DataBytes);
                Console.WriteLine("File received");
            }
            else
            {
                var fileWrite = File.OpenWrite(filePath);
                await fileWrite.WriteAsync(item.DataBytes, (int)fileWrite.Length, item.DataBytes.Length);
                Console.WriteLine("Collected file received");
            }
        }
    }
}
