using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class Reaction : BaseEntity
    {
        [Key]
        public int ReactionId { get; set; }
        public int? UserId { get; set; }
        public int MessageId { get; set; } = 0;
        public string ReactionContent { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo UserInfo { get; set; }

        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }
    }
}
