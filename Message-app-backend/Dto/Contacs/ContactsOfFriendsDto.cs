using Message_app_backend.Shared;

namespace Message_app_backend.Dto.Contacs
{
    public class ContactsOfFriendsDto
    {
        public int UserCurrentId { get; set; }
        public int UserId { get; set; }
        public string Avatar { get; set; }
        public string DisplayName { get; set; }
        public ConnectStausEnum ConnectStatus { get; set; } = ConnectStausEnum.Disconnect;
        public string DisconnectTime { get; set; } = null;

        public int RoomId { get; set; }

    }
}
