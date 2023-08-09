using Application.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application.Helpers
{
    public class DirectoryHelper : IDirectoryHelper
    {
        private static List<string> PictureExts = new List<string>() { ".jpg", ".png", ".gif" };
        private static List<string> VideoExts = new List<string>() { ".mp4", ".mov", ".flv", ".ts" };
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
        public List<DirectoryContent> GetFolderContent(string RootPath)
        {
            List<DirectoryContent> result = new List<DirectoryContent>();
            foreach (var dir in Directory.GetDirectories(RootPath))
            {
                result.Add(new DirectoryContent() {  ContentType = FolderContentType.Folder, Path = dir,
                    Format = ".dir", Title = dir.Split('\\').Last() });
            }
            foreach (var dir in Directory.GetFiles(RootPath))
            {
                result.Add(new DirectoryContent() { ContentType = FolderContentType.File, Path = dir,
                    Format = Path.GetExtension(dir), Title = Path.GetFileName(dir) });
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
        private void CreateVirtualDirectory(string path,string id)
        {
            //ServerManager iisManager = new ServerManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\config\\applicationHost.config"));
            //Microsoft.Web.Administration.Application mySite = iisManager.Sites["Default Web Site"].Applications[0];
            //if (mySite.VirtualDirectories[$"/FS/{Id}"] == null)
            //{
            //    mySite.VirtualDirectories.Add($"/FS/{Id}", path);
            //    iisManager.CommitChanges();
            //}
        }
        private void DeleteVirtualDirectory(string folderId)
        {
            //ServerManager iisManager = new ServerManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\config\\applicationHost.config"));
            //VirtualDirectory vd = iisManager.Sites["Default Web Site"].Applications[0].VirtualDirectories[$"/FS/{Id}"];
            //if (vd != null)
            //{
            //    iisManager.Sites["Default Web Site"].Applications[0].VirtualDirectories.Remove(vd);
            //    iisManager.CommitChanges();
            //}
        }
    }
}