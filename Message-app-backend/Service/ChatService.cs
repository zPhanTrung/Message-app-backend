using AutoMapper;
using Message_app_backend.Dto.Chat;
using Message_app_backend.Entities;
using Message_app_backend.Repository;

namespace Message_app_backend.Service
{
    public class ChatService : BaseService
    {
        MessageRepository messageRepository;
        ReactionRepository reactionRepository;
        IMapper mapper;
        public ChatService(MessageRepository messageRepository, ReactionRepository reactionRepository, IMapper mapper)
        {
            this.reactionRepository = reactionRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        public List<MessageDto> GetMessage(int roomId, int pageIndex, int pageSize)
        {
            List<MessageDto> messageDtos = new List<MessageDto>();
            var message = messageRepository.FindMessageByRoom(roomId, pageIndex, pageSize);
            messageDtos = mapper.Map<List<MessageDto>>(message);
            List<int> messageIds = message.Select(message => message.MessageId).ToList();
            var reactions = reactionRepository.FindReactionByMessageId(messageIds);
            List<ReactionDto> reactionDtos = mapper.Map<List<ReactionDto>>(reactions);

            foreach(var messageDto in messageDtos)
            {
                foreach(var reaction in reactionDtos)
                {
                    if(messageDto.MessageId == reaction.MessageId)
                    {
                        messageDto.Reaction = reaction;
                    }
                }
            }

            return messageDtos;

        }
    }
}
