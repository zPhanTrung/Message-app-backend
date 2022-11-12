using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Message_app_backend.Shared;
using System.Reflection;
using Message_app_backend.Entities;
using System.Net;
using Message_app_backend.Service;
using Message_app_backend.Dto.Register;

namespace Message_app_backend.Controllers
{
    [ApiController]
    public class RegisterController : ControllerBase
    {
        RegisterService registerService;
        public RegisterController(RegisterService registerService)
        {
            this.registerService = registerService;
        }

        [HttpPost]
        [Route("[controller]/Account")]
        public MessageResponse<bool> RegisterAccount(RegisterAccountDto registerAccountDto)
        {
            try
            {
                registerService.RegisterAccount(registerAccountDto);
                return new MessageResponse<bool> { Code = HttpStatusCode.Created, Message="Register success"};
            }
            catch(Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message=ex.Message };
            }
        }




        [HttpPost]
        [Route("/SendOtp")]
        public IActionResult SendOtp()
        {
            SpeedSMSAPI api = new SpeedSMSAPI("BRFjnnD5G6Xvg8H97eMzWGCEspP2CqHy");
            string userInfo = api.getUserInfo();

            string[] phones = new string[] { "0969546798" };
            string content = "test sms";
            int type = 1;
            string sender = "NOTIFY";
            string response = api.sendSMS(phones, content, type, sender);

            return Ok(new { userinfo = userInfo, response = response});
        }
    }
}
