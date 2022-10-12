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
        public DbSet<UserInfo> UserInfos { get; set; }
        public AppDb(DbContextOptions options) : base(options) { }
        
    }
}
