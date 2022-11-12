using Message_app_backend.Entities;
using Message_app_backend.Repository;

namespace Message_app_backend.Service
{
    public class UserService : BaseService
    {
        UserInfoRepository userInfoRepository;
        public UserService (UserInfoRepository userInfoRepository)
        {
            this.userInfoRepository = userInfoRepository;
        }
    }
}
