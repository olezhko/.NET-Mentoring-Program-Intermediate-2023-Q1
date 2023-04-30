namespace ConsoleImageProcessor.Models
{
    internal class RabbitMessage
    {
        public string Filename { get; set; }
        public byte[] DataBytes { get; set; }
        public int SegmentIndex { get; set; }
        public long FullSize { get; set; }
    }
}
