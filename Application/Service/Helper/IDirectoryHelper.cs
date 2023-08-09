using Application.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Application.Helpers
{
    public interface IDirectoryHelper
    {
        bool CheckDirectory(string path);
        bool CreateFolder(string path, string VirtualPath, bool isLocal = true);
        bool RemoveFolder(string path, string VirtualPath, bool isLocal = true);
        List<DirectoryContent> GetFolderContent(string RootPath);
        FileContentType GetFileType(string FilePath);
    }
}