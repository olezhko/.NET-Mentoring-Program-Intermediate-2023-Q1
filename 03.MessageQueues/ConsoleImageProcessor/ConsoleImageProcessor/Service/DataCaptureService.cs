using ConsoleImageProcessor.Models;

namespace ConsoleImageProcessor.Service
{
    public interface IDataCaptureService
    {
        void Start(string folder, string extension);
    }
    internal class DataCaptureService : IDataCaptureService
    {
        private readonly ITransmitterService _transmitter;
        const int MaxMessageSize = 1000000;
        private byte[] _buffer = new byte[MaxMessageSize];
        public DataCaptureService(ITransmitterService transmitter)
        {
            _transmitter = transmitter;
        }

        public void Start(string folder, string extension)
        {
            ValidateFolder(folder);

            var watcher = new FileSystemWatcher();
            watcher.Path = folder;
            watcher.Filter = extension;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Created += (_, e) =>
            {
                SendFile(e);
            };

            watcher.EnableRaisingEvents = true;
            Console.WriteLine("DataCaptureService started");
        }

        private void SendFile(FileSystemEventArgs args)
        {
            Console.WriteLine("Start sending new file...");
            while (true)
            {
                try
                {
                    using FileStream fileStream = new FileStream(args.FullPath, FileMode.Open);
                    int bytesRead;
                    int segmentIndex = 0;
                    while ((bytesRead = fileStream.Read(_buffer, 0, MaxMessageSize)) > 0)
                    {
                        // Send the chunk as a message to the message queue
                        byte[] segment = new byte[bytesRead];
                        Array.Copy(_buffer, 0, segment, 0, bytesRead);
                        SendMessageToQueue(segment, args.Name, segmentIndex, fileStream.Length);
                        segmentIndex++;
                    }
                    return;
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void SendMessageToQueue(byte[] segment, string name, int segmentIndex, long fullSize)
        {
            var document = new RabbitMessage()
            {
                Filename = name,
                DataBytes = segment,
                SegmentIndex = segmentIndex,
                FullSize = fullSize,
            };

            _transmitter.SendMessage(document);
        }

        private void ValidateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new Exception("Folder not exists");
            }
        }
    }
}
