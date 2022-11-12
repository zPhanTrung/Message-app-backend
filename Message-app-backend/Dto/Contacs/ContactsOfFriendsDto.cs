using Message_app_backend.Shared;

namespace Message_app_backend.Dto.Contacs
{
    public class ContactsOfFriendsDto
    {
        public int UserId { get; set; }
        public string AvatarBase64;
        public string DisplayName { get; set; }
        public ConnectStausEnum ConnectStatus { get; set; } = ConnectStausEnum.Disconnect;
        public string DisconnectTime { get; set; } = null;

    }
}
