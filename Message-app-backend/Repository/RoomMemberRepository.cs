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

        public int FindRoomIdByUserId(int user1, int user2)
        {
            var roomMembers1 = Model.Where(x => x.UserId == user1).ToList();
            var roomMembers2 = Model.Where(x => x.UserId == user2).ToList();

            int roomId = 0; 
            foreach(var roomMember1 in roomMembers1)
            {
                foreach(var roomMember2 in roomMembers2)
                {
                    if(roomMember1.RoomId == roomMember2.RoomId)
                    {
                        roomId = (int)roomMember1.RoomId;
                        break;
                    }
                } 
            }

            return roomId;
        }

        public List<RoomMember> FinByUserId(int userId)
        {
            return Model.Where(x => x.UserId == userId).ToList();
        }
    }
}
