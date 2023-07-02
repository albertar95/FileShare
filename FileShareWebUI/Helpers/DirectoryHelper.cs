using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace FileShareWebUI.Helpers
{
    public class DirectoryHelper
    {
        private static string RootVirtualFoldersAddr = ConfigurationManager.AppSettings["RootVirtualFolders"];
        public static bool CheckDirectory(string path)
        {
            return Directory.Exists(path);
        }
        public static string CreateNewVirtualFolder()
        {
            var folderPath = Path.Combine(RootVirtualFoldersAddr, Guid.NewGuid().ToString());
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }
    }
}