using Message_app_backend.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class Message : BaseEntity
    {
        [Key]
        public int MessageId { get; set; }
        public int? RoomId { get; set; }
        public int? UserId { get; set; }
        public string MessageContent { get; set; }
        public bool Recall { get; set; } = false;
        public DateTime SendTime { get; set; }
        public int CallDuration { get; set; }

        [Column(TypeName = "varchar(20)")]
        public MessageTypeEnum MessageType { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo UserInfo { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
        public virtual ICollection<Reaction> Reactions { get; set; }

    }
}
