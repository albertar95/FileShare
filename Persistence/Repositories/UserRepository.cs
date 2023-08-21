using Application.Persistence;
using Domain;
using Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly FileShareDbContext _context;
        public UserRepository(FileShareDbContext context) : base(context)
        {
            _context = context;
            _context.Configuration.AutoDetectChangesEnabled = false;
        }

        public User GetUserById(Guid Id)
        {
            return _context.Users.ToList().FirstOrDefault(p => p.Id == Id);
        }

        public List<User> GetUsers(bool IncludeDisabled = true, int Skip = 0, int PageSize = 100)
        {
            if(!IncludeDisabled)
                return _context.Users.Where(p => p.IsDisabled == false).OrderBy(p => p.Id).Skip(Skip).Take(PageSize).ToList();
            else
                return _context.Users.OrderBy(p => p.Id).Skip(Skip).Take(PageSize).ToList();
        }

        public Tuple<byte, User> LoginUser(string Username, string Password)
        {
            var user = _context.Users.FirstOrDefault(p => p.Username.Trim() == Username.Trim());
            if (user == null) return new Tuple<byte, User>(1, null);
            else
            {
                if (user.IsDisabled) return new Tuple<byte, User>(2, null);
                else
                {
                    if (user.Password != Password)
                    {
                        if (user.IncorrectPasswordCount >= 7)
                            user.IsDisabled = true;
                        else
                            user.IncorrectPasswordCount = user.IncorrectPasswordCount + 1;
                        _context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        return new Tuple<byte, User>(3, null);
                    }
                    else
                    {
                        user.LastLoginDate = DateTime.Now;
                        _context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        return new Tuple<byte, User>(4, user);
                    }
                }
            }
        }
    }
}
