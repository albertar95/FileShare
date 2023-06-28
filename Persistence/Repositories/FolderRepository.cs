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
    public class FolderRepository : BaseRepository, IFolderRepository
    {
        private readonly FileShareDbContext _context;
        public FolderRepository(FileShareDbContext context) : base(context)
        {
            _context = context;
            _context.Configuration.AutoDetectChangesEnabled = false;
        }

        public Folder GetFolderById(Guid Id)
        {
            return _context.Folders.ToList().FirstOrDefault(p => p.Id == Id);
        }

        public List<Folder> GetFoldersByUserId(Guid UserId, bool IncludePublics = false, bool HasAdminAccess = false, int Skip = 0, int PageSize = 100)
        {
            List<Folder> result = new List<Folder>();
            var tmpFolders = _context.Folders.ToList();
            result.AddRange(tmpFolders.Where(p => p.UserId == UserId).ToList());
            if (IncludePublics)
                result.AddRange(tmpFolders.Where(p => p.UserId != UserId && !p.IsProtected && p.IsPublic).ToList());
            if (HasAdminAccess && IncludePublics)
                result.AddRange(tmpFolders.Where(p => p.UserId != UserId && !p.IsProtected && !p.IsPublic).ToList());
            if (HasAdminAccess && !IncludePublics)
                result.AddRange(tmpFolders.Where(p => p.UserId != UserId && !p.IsProtected).ToList());
            return result.OrderBy(p => p.Id).Skip(Skip).Take(PageSize).ToList();
        }
    }
}
