using Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contexts
{
    public class FileShareDbContext : DbContext
    {
        public DbSet<Folder> Folders { get; set; }
        public DbSet<User> Users { get; set; }
        public FileShareDbContext(SQLiteConnection connection) : base(connection, true)
        {
        }
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            //primary keys
            builder.Entity<Domain.Folder>().HasKey(f => f.Id);
            builder.Entity<Domain.User>().HasKey(f => f.Id);
            //foreign keys
            builder.Entity<Domain.User>().HasMany(p => p.Folders);
            //indexes
            //builder.Entity<Domain.User>().HasIndex(p => p.Username).IsUnique().HasName("IX_Username");
            base.OnModelCreating(builder);
        }
    }
}
