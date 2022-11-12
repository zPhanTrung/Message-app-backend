using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class GroupChat : BaseEntity
    {
        [Key]
        public int GroupChatId { get; set; }
        public string Name { get; set; }
        public int? MasterId { get; set; }
        public int QuanityMember { get; set; } = 0;
        public int? RoomId { get; set; }

        [ForeignKey("MasterId")]
        public virtual UserInfo UserInfo { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

    }
}
