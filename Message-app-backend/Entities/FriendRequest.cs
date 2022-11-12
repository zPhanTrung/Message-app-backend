using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class FriendRequest : BaseEntity
    {
        public int FriendRequestId { get; set; }
        public int? UserReceiveId { get; set; }
        public int? UserSendId { get; set; } 
        public string Message { get; set; }

        [ForeignKey("UserSendId")]
        public UserInfo UserInfo { get; set; }


    }
}
