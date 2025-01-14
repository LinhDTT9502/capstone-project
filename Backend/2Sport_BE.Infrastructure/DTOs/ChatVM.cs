namespace _2Sport_BE.Service.DTOs
{
    public class ChatVM()
    {
        public Guid ChatSessionId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int CoordinatorId { get; set; }
        public string CoordinatorName { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public List<MessageVM> MessageVMs { get; set; }
    }

    public class MessageVM()
    {
        public Guid MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Timestamp { get; set; } 
    }
}
