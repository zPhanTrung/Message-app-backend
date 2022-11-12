namespace Message_app_backend.Dto.Contacs
{
    public class SendFriendRequestDto
    {
        public int? UserReceiveId { get; set; }
        public int? UserSendId { get; set; }
        public string Message { get; set; }
    }
}
