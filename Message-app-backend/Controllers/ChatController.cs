using AutoMapper;
using Message_app_backend.Dto.Chat;
using Message_app_backend.Dto.Contacs;
using Message_app_backend.Repository;
using Message_app_backend.Service;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Message_app_backend.Controllers
{
    [ApiController]
    public class ChatController : ControllerBase
    {
        ChatService chatService;
        RoomMemberRepository roomMemberRepository;
        public ChatController(ChatService chatService, RoomMemberRepository roomMemberRepository)
        {
            this.chatService = chatService;
            this.roomMemberRepository = roomMemberRepository;
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/GetMessage")]
        public MessageResponse<List<MessageDto>> GetMessage(int roomId, int pageIndex, int pageSize)
        {
            try
            {
                var data = chatService.GetMessage(roomId, pageIndex, pageSize);
                return new MessageResponse<List<MessageDto>> { Code = HttpStatusCode.OK, Message = "", Data = data };
            }
            catch (Exception ex)
            {
                return new MessageResponse<List<MessageDto>> { Code = HttpStatusCode.NotFound, Message = "Error:" + ex.Message };
            }
        }



        [HttpGet]
        [Authorize]
        [Route("[controller]/GetRoomId")]
        public MessageResponse<int> GetRoomId(int user1, int user2)
        {
            try
            {
                var roomId = roomMemberRepository.FindRoomIdByUserId(user1, user2);
                return new MessageResponse<int> { Code = HttpStatusCode.OK, Message = "", Data = roomId };
            }
            catch (Exception ex)
            {
                return new MessageResponse<int> { Code = HttpStatusCode.NotFound, Message = "Error:" + ex.Message };
            }
        }

    }
}
