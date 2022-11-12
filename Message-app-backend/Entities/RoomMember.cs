using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Message_app_backend.Entities
{
    public class RoomMember : BaseEntity
    {
        [Key]
        public int RoomMemberId { get; set; }

        public int? UserId { get; set; }
        public int? RoomId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo UserInfo { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

    }
}
