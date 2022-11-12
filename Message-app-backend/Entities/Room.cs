using Message_app_backend.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class Room : BaseEntity
    {
        [Key]
        public int RoomId { get; set; }

        [Column(TypeName = "varchar(20)")]
        public RoomTypeEnum RoomType { get; set; }

        [Column(TypeName = "varchar(20)")]
        public ActiveStatusEnum ActiveStatus { get; set; }
        public virtual ICollection<RoomMember> RoomMembers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<GroupChat> GroupChats { get; set; }  
    }
}
