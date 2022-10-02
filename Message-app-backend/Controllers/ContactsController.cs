using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Message_app_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        [HttpGet]
        [Route("GetContacts")]
        public string GetContacts()
        {
            return "Get contacts";
        }
    }
}
