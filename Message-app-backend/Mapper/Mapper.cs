using AutoMapper;
using Message_app_backend.Dto.Chat;
using Message_app_backend.Dto.Contacs;
using Message_app_backend.Dto.Personal;
using Message_app_backend.Dto.Register;
using Message_app_backend.Entities;

namespace Message_app_backend.Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserInfo, AddFirendContactsInfoDto>();
            CreateMap<UserInfo, ContactsOfFriendsDto>();
            CreateMap<Reaction, ReactionDto>();
            CreateMap<Message, MessageDto>();
            CreateMap<RegisterAccountDto, UserInfo>();
            CreateMap<UserInfo, ReceiveFriendRequestDto>();
            CreateMap<UserInfo, UserInfoCurrentDto>();
        }
    }
}
