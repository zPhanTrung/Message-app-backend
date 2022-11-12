namespace Message_app_backend.Dto.Contacs
{
    public class ReceiveFriendRequestDto
    {
        public int FriendRequestId { get; set; }
        public int UserSendId { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public string Message { get; set; }
    }
}
