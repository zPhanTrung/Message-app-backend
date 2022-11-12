using Message_app_backend.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Message_app_backend.Dto.Chat
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int? RoomId { get; set; }
        public int? UserId { get; set; }
        public string MessageContent { get; set; }
        public bool Recall { get; set; } = false;
        public DateTime SendTime { get; set; }
        public int CallDuration { get; set; }
        public ReactionDto Reaction { get; set; }
        public MessageTypeEnum MessageType { get; set; }
    }
}
