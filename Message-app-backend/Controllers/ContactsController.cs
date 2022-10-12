using Message_app_backend.Entities;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Message_app_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        [HttpGet]
        [Route("GetContacts")]
        [Authorize]
        public string GetContacts()
        {
            return "Get contacts";
        }

        [HttpGet]
        [Route("")]
        public MessageResponse<string> Get(int id)
        {
            var data = id.ToString();
            return new MessageResponse<string> { Code = HttpStatusCode.OK, Message = "", Data = data };
        }

    }
}
