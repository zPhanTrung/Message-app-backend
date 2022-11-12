using Message_app_backend.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Message_app_backend.Entities
{
    public class UserInfo : BaseEntity
    {
        [Key]
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Column(TypeName = "varchar(20)")]
        public GenderEnum Gender { get; set; } = GenderEnum.Male;
        public ActiveStatusEnum ActiveStatus { get; set; } = ActiveStatusEnum.Active;
        public string RefreshToken { get; set; } = string.Empty;

        [Column(TypeName = "varchar(max)")]
        public string Avatar { get; set; } = string.Empty;
        public DateTime DisconnectTime { get; set; }
        public int FriendRequest { get; set; } = 0;
        public int InviteJoinGroupChat { get; set; } = 0;

        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<RoomMember> RoomMembers { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<GroupChat> GroupChats { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
