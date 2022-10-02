using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppDb:DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public AppDb(DbContextOptions options) : base(options) { }
    }
}
