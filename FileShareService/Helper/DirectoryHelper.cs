using Application.Model;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application.Helpers
{
    public class DirectoryHelper : IDirectoryHelper
    {
        private static List<string> PictureExts = new List<string>() { ".jpg", ".png", ".gif" };
        private static List<string> VideoExts = new List<string>() { ".mp4", ".mov", ".flv", ".ts",".mkv" };
        private static List<string> PdfExts = new List<string>() { ".pdf" };
        private static List<string> DocExts = new List<string>() { ".doc", ".docx" };
        private static List<string> SpreadSheetExts = new List<string>() { ".xls", ".xlsx" };
        private static List<string> PresentationExts = new List<string>() { ".ppt", ".pptx" };
        private static List<string> AudioExts = new List<string>() { ".mp3", ".wav" };
        private static List<string> CompressExts = new List<string>() { ".zip", ".rar", ".7z" };
        public bool CheckDirectory(string path)
        {
            return Directory.Exists(path);
        }
        public bool CreateFolder(string path, string folderId, bool isLocal = true)
        {
            try
            {
                if (!isLocal)
                    Directory.CreateDirectory(path);
                CreateVirtualDirectory(path,folderId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public bool RemoveFolder(string path,string folderId, bool isLocal = true)
        {
            try
            {
                DeleteVirtualDirectory(folderId);
                if (!isLocal)
                    Directory.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<DirectoryContent> GetFolderContent(string RootPath,string VirtualPath,int height = 1,long rootId = 0,long startIndex = 0)
        {
            List<DirectoryContent> result = new List<DirectoryContent>();
            var FirstLevelContents = GetChildElements(RootPath,VirtualPath, height,rootId,startIndex);
            result.AddRange(FirstLevelContents);
            height++;
            foreach (var node in FirstLevelContents.Where(p => p.ContentType == FolderContentType.Folder).ToList())
            {
                result.AddRange(GetFolderContent(node.Path,string.Concat(VirtualPath,node.Path.Replace(RootPath,"").Replace('\\','/')),height,node.Id,result.Max(p => p.Id)));
            }
            return result;
        }
        public FileContentType GetFileType(string FilePath)
        {
            var ext = Path.GetExtension(FilePath).ToLower();
            if (PictureExts.Contains(ext))
                return FileContentType.Picture;
            else if (VideoExts.Contains(ext))
                return FileContentType.Video;
            else if (PdfExts.Contains(ext))
                return FileContentType.Pdf;
            else if (DocExts.Contains(ext))
                return FileContentType.Doc;
            else if (AudioExts.Contains(ext))
                return FileContentType.Audio;
            else if (SpreadSheetExts.Contains(ext))
                return FileContentType.SpreadSheet;
            else if (PresentationExts.Contains(ext))
                return FileContentType.Presentation;
            else if (CompressExts.Contains(ext))
                return FileContentType.Compress;
            else
                return FileContentType.Unknown;
        }
        public bool CreateSubFolder(string path)
        {
            Directory.CreateDirectory(path);
            return Directory.Exists(path);
        }
        private void CreateVirtualDirectory(string path,string id)
        {
            ServerManager iisManager = new ServerManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\config\\applicationHost.config"));
            Microsoft.Web.Administration.Application mySite = iisManager.Sites["Default Web Site"].Applications[0];
            if (mySite.VirtualDirectories[$"/FS/{id}"] == null)
            {
                mySite.VirtualDirectories.Add($"/FS/{id}", path);
                iisManager.CommitChanges();
            }
        }
        private void DeleteVirtualDirectory(string folderId)
        {
            ServerManager iisManager = new ServerManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\config\\applicationHost.config"));
            VirtualDirectory vd = iisManager.Sites["Default Web Site"].Applications[0].VirtualDirectories[$"/FS/{folderId}"];
            if (vd != null)
            {
                iisManager.Sites["Default Web Site"].Applications[0].VirtualDirectories.Remove(vd);
                iisManager.CommitChanges();
            }
        }
        private List<DirectoryContent> GetChildElements(string path,string vpath,int height,long rootId,long startIndex)
        {
            List<DirectoryContent> result = new List<DirectoryContent>();
            try
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    result.Add(new DirectoryContent()
                    {
                        Id = ++startIndex,
                        ContentType = FolderContentType.Folder,
                        Path = dir,
                        Format = ".dir",
                        Title = dir.Split('\\').Last(),
                        HeightLevel = height,
                        RootFolderId = rootId,
                        FileContentType = FileContentType.Unknown,
                        Vpath = string.Concat(vpath, dir.Replace(path, "").Replace('\\', '/')).ToLower().Replace(" ","%20")
                    });
                }
                foreach (var dir in Directory.GetFiles(path))
                {
                    result.Add(new DirectoryContent()
                    {
                        Id = ++startIndex,
                        ContentType = FolderContentType.File,
                        Path = dir,
                        Format = Path.GetExtension(dir),
                        Title = Path.GetFileName(dir),
                        HeightLevel = height,
                        RootFolderId = rootId,
                        FileContentType = GetFileType(dir),
                        Vpath = string.Concat(vpath, dir.Replace(path, "").Replace('\\', '/')).ToLower().Replace(" ", "%20")
                    });
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}