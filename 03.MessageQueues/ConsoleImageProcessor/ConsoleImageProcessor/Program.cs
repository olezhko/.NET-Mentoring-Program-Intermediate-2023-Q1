using ConsoleImageProcessor.Service;

Console.WriteLine("3. Message queues");
CancellationTokenSource cts = new CancellationTokenSource();

ITransmitterService transmitterService = new TransmitterService();
IDataCaptureService captureService = new DataCaptureService(transmitterService);
captureService.Start(Path.Combine(Environment.CurrentDirectory, "Source"),"*.pdf");

IReceiverService receiverService = new ReceiverService(Path.Combine(Environment.CurrentDirectory, "Results"));
receiverService.Start(cts.Token);

Console.ReadLine();