using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class GroupMember : BaseEntity
    {
        public int GroupMemberId { get; set; }
        public int UserId { get; set; }
        public int GroupChatId { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }

        [ForeignKey("GroupChatId")]
        public GroupChat Chat { get; set; }
    }
}
