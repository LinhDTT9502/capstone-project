namespace _2Sport_BE.ViewModels
{
    public class CommentDTO
    {
        public string Content { get; set; }
    }

    public class CommentVM : CommentDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username{ get; set; }
        public string ProductCode { get; set; }
        public int ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentCM : CommentDTO
    {
    }
    public class CommentUM : CommentDTO
    {

    }
}
