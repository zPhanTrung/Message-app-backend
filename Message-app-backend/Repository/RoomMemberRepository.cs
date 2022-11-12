using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class RoomMemberRepository : BaseRepositoryImpl<RoomMember>
    {
        public RoomMemberRepository(AppDb appDb) : base(appDb) { }

        public RoomMember? FindByRoomId(int roomId)
        {
            return Model.FirstOrDefault(roomMember => roomMember.RoomId == roomId);
        }

        public List<RoomMember> FindByUserId(int user1, int user2)
        {
            return Model.Where(roomMember=> roomMember.UserId == user1 || roomMember.UserId == user2).ToList();
        }
    }
}
