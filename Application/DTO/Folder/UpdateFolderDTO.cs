using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Folder
{
    public class UpdateFolderDTO
    {
        public string Title { get; set; }
        public bool IsPublic { get; set; }
        public bool IsProtected { get; set; }
    }
}
