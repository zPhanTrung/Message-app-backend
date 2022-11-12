using AutoMapper;
using Message_app_backend.Dto.Personal;
using Message_app_backend.Dto.Register;
using Message_app_backend.Repository;
using Message_app_backend.Service;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Message_app_backend.Controllers
{
    [ApiController]
    public class PersonalController : ControllerBase
    {
        IMapper mapper;
        UserInfoRepository userInfoRepository;
        public PersonalController(IMapper mapper, UserInfoRepository userInfoRepository)
        {
            this.mapper = mapper;
            this.userInfoRepository = userInfoRepository;
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/GetUserInfoCurrent")]
        public MessageResponse<UserInfoCurrentDto> GetUserInfoCurrent()
        {
            try
            {
                int userId = GetUserId();
                var user = userInfoRepository.FindById(userId);
                var data = mapper.Map<UserInfoCurrentDto>(user);

                return new MessageResponse<UserInfoCurrentDto> { Code = HttpStatusCode.OK, Data = data };
            }
            catch (Exception ex)
            {
                return new MessageResponse<UserInfoCurrentDto> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        private int GetUserId()
        {
            return int.Parse(User.Claims.FirstOrDefault(claim => claim.Type.Equals("UserId")).Value);
        }
    }
}
