using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;

namespace Message_app_backend.Repository
{
    public class GroupMemberRepository : BaseRepositoryImpl<GroupMember>
    {
        public GroupMemberRepository(AppDb appDb) : base(appDb) { }

        public List<GroupMember> FindByGroupId(int groupId)
        {
            return Model.Where(groupMember => groupMember.GroupChatId == groupId).ToList();
        }
    }
}
