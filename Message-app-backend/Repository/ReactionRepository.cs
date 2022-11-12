using Message_app_backend.Entities;
using Message_app_backend.Repository.Implement;
using System.Linq;

namespace Message_app_backend.Repository
{
    public class ReactionRepository : BaseRepositoryImpl<Reaction>
    {
        public ReactionRepository(AppDb appDb) : base(appDb) { }

        public List<Reaction> FindReactionByMessageId(List<int> ids)
        {
            return Model.Where(reaction => ids.Contains(reaction.MessageId)).ToList();
        }
    }
}
