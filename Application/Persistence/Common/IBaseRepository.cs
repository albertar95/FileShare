using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence.Common
{
    public interface IBaseRepository
    {
        Task<bool> Add<T>(T entity) where T : class;
        Task<bool> Delete<T>(T entity) where T : class;
        Task<bool> Update<T>(T entity) where T : class;
    }
}
