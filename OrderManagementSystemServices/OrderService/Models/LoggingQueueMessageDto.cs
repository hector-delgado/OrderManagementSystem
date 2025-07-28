namespace OrderService.Models
{
    public class LoggingQueueMessageDto
    {
        public string Message { get; set; }

        public string Level { get; set; }

        public string Exception { get; set; }
    }
}
