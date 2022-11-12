using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class RoomRepository : BaseRepositoryImpl<Room>
    {
        public RoomRepository(AppDb appDb) : base(appDb) { }

        public List<int?> FindUserIdsByRoom(int roomId)
        {
            var room = Model.Where(room => room.RoomId == roomId).FirstOrDefault();
            if (room == null)
            {
                throw new Exception("room not found");
            }
            return room.RoomMembers.Select(member => member.UserId).ToList();
        }

    }
}
