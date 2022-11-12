using AutoMapper;
using Message_app_backend.Dto.Register;
using Message_app_backend.Entities;
using Message_app_backend.Repository;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Message_app_backend.Service
{
    public class RegisterService : BaseService
    {
        UserInfoRepository userInfoRepository;
        IMapper mapper;
        public RegisterService(UserInfoRepository userInfoRepository, IMapper mapper)
        {
            this.userInfoRepository = userInfoRepository;
            this.mapper = mapper;
        }

        public bool RegisterAccount(RegisterAccountDto registerAccountDto)
        {
            var userInfo = mapper.Map<UserInfo>(registerAccountDto);
            userInfoRepository.Create(userInfo);
            return true;
        }
    }
}
