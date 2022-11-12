using Message_app_backend.Dto;
using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class UserInfoRepository : BaseRepositoryImpl<UserInfo>
    {
        public UserInfoRepository(AppDb appDb) : base(appDb) { }

        public List<UserInfo> FindUserInfoByIds(List<int?> ids)
        {
            return Model.Where(user => ids.Contains(user.UserId)).ToList();
        }

        public UserInfo? FindByPhoneNumber(string phoneNumber)
        {
            return Model.FirstOrDefault(user => user.PhoneNumber.Equals(phoneNumber));
        }
        
    }
}
