using Message_app_backend.Dto.Contacs;
using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class FriendRepository : BaseRepositoryImpl<Friend>
    {
        public FriendRepository(AppDb appDb) : base(appDb) { }

        public List<Friend> GetFriends(int userId)
        {
            return Model.Where(friend => friend.UserId == userId).ToList();
        }

        public Friend? CheckExistFriend(SendFriendRequestDto friendRequestDto)
        {
            return Model.FirstOrDefault(
                friend => friend.UserId == friendRequestDto.UserSendId
                && friend.FriendId == friendRequestDto.UserReceiveId);
        }

        public Friend? FindByFriendId(int userId, int friendId)
        {
            return Model.FirstOrDefault(friend=>friend.FriendId == friendId && friend.UserId == userId);
        }
    }
}
