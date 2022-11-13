using AutoMapper;
using Message_app_backend.Dto.Message;
using Message_app_backend.Repository;

namespace Message_app_backend.Service
{
    public class MessageService : BaseService
    {
        MessageRepository messageRepository;
        UserInfoRepository userInfoRepository;
        RoomMemberRepository roomMemberRepository;
        IMapper mapper;

        public MessageService(MessageRepository messageRepository, IMapper mapper, UserInfoRepository userInfoRepository, RoomMemberRepository roomMemberRepository)
        {
            this.messageRepository = messageRepository;
            this.userInfoRepository = userInfoRepository;
            this.roomMemberRepository = roomMemberRepository;
            this.mapper = mapper;
        }

        public List<MessageInMessageDto> GetListMessage(int userId)
        {
            var messages = messageRepository.GetLastMessage(userId);

            var roomIds = roomMemberRepository.FinByUserId(userId).Select(x=>x.RoomId).ToList();

            var result = new List<MessageInMessageDto>();

            foreach(var roomId in roomIds)
            {
                var message = messageRepository.GetLastMessage((int)roomId);
                
                if (message != null)
                {
                    var user = userInfoRepository.FindById((int)message.UserId);
                    var messageInMessageDto = new MessageInMessageDto
                    {
                        UserId = user.UserId,
                        Avatar = user.Avatar,
                        DisplayName = user.DisplayName,
                        ConnectStatus = 0,
                        RoomId = (int)message.RoomId,
                        SendTime = message.SendTime.ToString(),
                        MessageContent = message.MessageContent,
                        UserCurrentId = userId
                    };

                    result.Add(messageInMessageDto);
                }
            }

            return result;
        }
    }
}
