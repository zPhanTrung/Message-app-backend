using Message_app_backend.Dto.Contacs;
using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class FriendRequestRepository: BaseRepositoryImpl<FriendRequest>
    {
        public FriendRequestRepository(AppDb appDb) : base(appDb) { }

        public FriendRequest? CheckExistOfRequest(SendFriendRequestDto friendRequestDto)
        {
            return Model.FirstOrDefault(
                request => (request.UserSendId == friendRequestDto.UserSendId &&
                request.UserReceiveId == friendRequestDto.UserReceiveId) ||
                (request.UserSendId == friendRequestDto.UserReceiveId &&
                request.UserReceiveId == friendRequestDto.UserSendId)
                );
        }

        public List<FriendRequest> FindAllRequestByUserReceiveId(int userId)
        {
            return Model.Where(request => request.UserReceiveId == userId).ToList();
        }
    }
    
}
