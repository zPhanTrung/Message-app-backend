using Message_app_backend.Shared;

namespace Message_app_backend.RealTime.Dto
{
    public class ReceiveMessageDto
    {
        public int RoomId { get; set; }
        public string MessageContent { get; set; }
        public int UserSendId { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public string Time { get; set; }
        public RoomTypeEnum RoomType { get; set; }

    }
}
