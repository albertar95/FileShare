using Application.Persistence.Common;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IFolderRepository : IBaseRepository
    {
        List<Folder> GetFoldersByUserId(Guid UserId,bool IncludePublics = false,bool HasAdminAccess = false
            , int Skip = 0, int PageSize = 100);
        Folder GetFolderById(Guid Id);
    }
}
