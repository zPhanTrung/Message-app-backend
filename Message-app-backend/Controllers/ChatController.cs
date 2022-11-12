using AutoMapper;
using Message_app_backend.Dto.Chat;
using Message_app_backend.Dto.Contacs;
using Message_app_backend.Repository;
using Message_app_backend.Service;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Message_app_backend.Controllers
{
    [ApiController]
    public class ChatController : ControllerBase
    {
        ChatService chatService;
        public ChatController(ChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet]
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
        [Route("[controller]/testDateTime")]
        public MessageResponse<string> testDateTime()
        {
            try
            {
                var date = DateTime.Parse("11/12/2022 20:51:38");
                return new MessageResponse<string> { Code = HttpStatusCode.OK, Message = "", Data = date.ToString("dd-MM-yyyy HH:mm:ss")};
            }
            catch (Exception ex)
            {
                return new MessageResponse<string> { Code = HttpStatusCode.NotFound, Message = "Error:" + ex.Message };
            }
        }

    }
}
