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
        private Dictionary<string, List<RabbitMessage>> ProcessFiles;
        private string storageFolder;
        public ReceiverService(string folder)
        {
            ProcessFiles = new Dictionary<string, List<RabbitMessage>>();
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "results",
                durable: true,
                exclusive: false,
                autoDelete: false);

            storageFolder = folder;

            if (!Directory.Exists(storageFolder))
            {
                Directory.CreateDirectory(storageFolder);
            }
        }

        public void Start(CancellationToken token)
        {
            // create a consumer that listens on the channel (queue)
            var consumer = new EventingBasicConsumer(_channel);

            // handle the Received event on the consumer
            // this is triggered whenever a new message
            // is added to the queue by the producer
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Task.Run(() =>
                {
                    var item = JsonConvert.DeserializeObject<RabbitMessage>(message);
                    CollectFile(item);
                }, token);
            };

            _channel.BasicConsume(queue: "results", autoAck: true, consumer: consumer);
            Console.WriteLine("ReceiverService started");
        }

        private void CollectFile(RabbitMessage item)
        {
            string filePath = Path.Combine(storageFolder, item.Filename);
            if (item.DataBytes.Length == item.FullSize)
            {
                File.WriteAllBytes(filePath, item.DataBytes);
                Console.WriteLine("File received");
            }
            else
            {
                if (!ProcessFiles.ContainsKey(item.Filename))
                {
                    var list = new List<RabbitMessage> { item };
                    ProcessFiles.Add(item.Filename, list);
                }
                else
                {
                    var segments = ProcessFiles[item.Filename];
                    segments.Add(item);

                    bool isLastSegment = segments.Select(seg=>seg.DataBytes.Length).Sum() == item.FullSize;
                    if (!isLastSegment) 
                        return;

                    var fileBytes = new byte[item.FullSize];
                    var sortedSegments = segments.OrderBy(s => s.SegmentIndex).ToList();
                    for (var i = 0; i < sortedSegments.Count; i++)
                    {
                        var segment = sortedSegments[i];
                        Array.Copy(segment.DataBytes, 0, fileBytes, i * segment.DataBytes.Length, segment.DataBytes.Length);
                    }
                    segments.Clear();
                    File.WriteAllBytes(filePath, fileBytes);

                    ProcessFiles.Remove(item.Filename);
                    Console.WriteLine("Collected file received");
                }
            }
        }
    }
}
