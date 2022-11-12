using Message_app_backend.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class InviteAndRequestJoinGroupChat : BaseEntity
    {
        public int Id { get; set; }
        public int? UserSendId { get; set; }
        public int UserReceiveId { get; set; }
        public int? GroupChatId { get; set; }

        [Column(TypeName = "varchar(20)")]
        public JoinGroupChatTypeEnum Type { get; set; }

        [ForeignKey("UserSendId")]
        public virtual UserInfo UserInfo { get; set; }

        [ForeignKey("GroupChatId")]
        public virtual GroupChat GroupChat { get; set; }
    }
}
