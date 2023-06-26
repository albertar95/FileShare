using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Folder : BaseProperties
    {
        public string Title { get; set; }
        public bool IsLocal { get; set; }
        public bool IsPublic { get; set; }
        public bool IsProtected { get; set; }
        public string Path { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
