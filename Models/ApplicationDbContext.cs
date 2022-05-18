using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_2564.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> user { get; set; }
        public DbSet<Role> role { get; set; }
        public DbSet<Queue> queue { get; set; }
        public DbSet<Table> table { get; set; }
        public DbSet<Setting> setting { get; set; }
    }
}