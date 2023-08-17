using Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileShareWebUI2.ViewModels
{
    public class FileViewModel
    {
        public DirectoryContent CurrentFile { get; set; }
        public List<DirectoryContent> RelatedFiles { get; set; }
        public Guid RootFolderId { get; set; }
        public long FolderId { get; set; }
    }
}