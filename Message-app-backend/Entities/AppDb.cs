using Message_app_backend.DataSeed;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message_app_backend.Entities
{
    public class AppDb:DbContext
    {
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<InviteAndRequestJoinGroupChat> InviteAndRequestJoinGroupChats { get; set; }


        public AppDb(DbContextOptions options) : base(options) { }

        public AppDb(){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            new UserInfoSeeds(modelBuilder);
        }

        

    }
}
