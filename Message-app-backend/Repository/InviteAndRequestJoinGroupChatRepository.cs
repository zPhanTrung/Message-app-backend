using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class InviteAndRequestJoinGroupChatRepository : BaseRepositoryImpl<InviteAndRequestJoinGroupChat>
    {
        public InviteAndRequestJoinGroupChatRepository(AppDb appDb) : base(appDb) { }
    }
}
