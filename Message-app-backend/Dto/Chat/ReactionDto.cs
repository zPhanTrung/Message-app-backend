namespace Message_app_backend.Dto.Chat
{
    public class ReactionDto
    {
        public int? UserId { get; set; }
        public int? MessageId { get; set; }
        public string ReactionContent { get; set; }
    }
}
