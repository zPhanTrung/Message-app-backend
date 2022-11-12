using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class Activity : BaseEntity
    {
        [Key]
        public int ActivityId { get; set; }
        public int? GroupChatId { get; set; }
        public string ActivityContent { get; set; }

        [ForeignKey("GroupChatId")]
        public virtual GroupChat GroupChat { get; set; }
    }
}
