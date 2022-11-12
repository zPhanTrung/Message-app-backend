using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Entities
{
    public class Friend : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? FriendId { get; set; }
        public string? DisplayName { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo UserInfo { get; set; }

    }
}
