﻿using System;
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
        private static List<string> PictureExts = new List<string>() { ".jpg", ".png",".gif" };
        private static List<string> VideoExts = new List<string>() { ".mp4", ".mov", ".flv", ".ts" };
        private static List<string> PdfExts = new List<string>() { ".pdf" };
        private static List<string> DocExts = new List<string>() { ".doc", ".docx" };
        private static List<string> SpreadSheetExts = new List<string>() { ".xls", ".xlsx" };
        private static List<string> PresentationExts = new List<string>() { ".ppt", ".pptx" };
        private static List<string> AudioExts = new List<string>() { ".mp3", ".wav" };
        private static List<string> CompressExts = new List<string>() { ".zip", ".rar", ".7z" };
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
        public static List<DirectoryContent> GetFolderContent(string RootPath)
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
        public static bool CreateNewFolder(string DirectoryPath)
        {
            Directory.CreateDirectory(DirectoryPath);
            return Directory.Exists(DirectoryPath);
        }
        public static FileContentType GetFileType(string FilePath)
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
    }
    public class DirectoryContent
    {
        public FolderContentType ContentType { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Format { get; set; }
    }
    public enum FolderContentType { File = 1,Folder = 2 }
    public enum FileContentType { Picture = 1,Video = 2,Pdf = 3,Doc = 4,Compress = 5,Audio = 6,SpreadSheet = 7,Presentation = 8, Unknown = 0 }
}