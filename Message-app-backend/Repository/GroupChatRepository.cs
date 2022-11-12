using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class GroupChatRepository : BaseRepositoryImpl<GroupChat>
    {
        public GroupChatRepository(AppDb appDb) : base(appDb) { }

        public GroupChat? FindByRoomId(int roomId)
        {
            return Model.FirstOrDefault(group => group.RoomId == roomId);
        }
    }
}
