using Message_app_backend.Dto.Message;
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
    public class MessageController : ControllerBase
    {
        MessageService messageService;
        public MessageController(MessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/GetMessage")]
        public MessageResponse<List<MessageInMessageDto>> GetMessage()
        {
            try
            {
                var userId = GetUserId();
                var data = messageService.GetListMessage(userId);
                return new MessageResponse<List<MessageInMessageDto>> { Code = HttpStatusCode.OK, Message = "", Data = data };
            }
            catch (Exception ex)
            {
                return new MessageResponse<List<MessageInMessageDto>> { Code = HttpStatusCode.NotFound, Message = "Error:" + ex.Message };
            }
        }

        private int GetUserId()
        {
            return int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value);
        }
    }
}
