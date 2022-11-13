using Message_app_backend.Shared;

namespace Message_app_backend.Dto.Message
{
    public class MessageInMessageDto
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string MessageContent { get; set; }
        public string SendTime { get; set; }
        public int RoomId { get; set; }
        public ConnectStausEnum ConnectStatus { get; set; }
        public string Avatar { get; set; }
    }
}
