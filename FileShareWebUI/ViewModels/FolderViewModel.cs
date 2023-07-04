using Application.DTO.Folder;
using FileShareWebUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileShareWebUI.ViewModels
{
    public class FolderViewModel
    {
        public FolderDTO Folder { get; set; }
        public List<DirectoryContent> Contents { get; set; }
        public string ReturnUrl { get; set; }
    }
}