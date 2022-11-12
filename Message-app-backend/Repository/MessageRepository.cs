using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;
using Message_app_backend.Shared;

namespace Message_app_backend.Repository
{
    public class MessageRepository : BaseRepositoryImpl<Message>
    {
        public MessageRepository(AppDb appDb) : base(appDb) { }

        public List<Message> FindMessageByRoom(int roomId, int pageIndex, int pageSize)
        {
            var messsageList = Model.Where(message => message.RoomId == roomId)
                .OrderByDescending(messsage => messsage.SendTime).ToList();

            return new PageList<Message>(messsageList, pageIndex, pageSize).Result;
        }
    }
}
