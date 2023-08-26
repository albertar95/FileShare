using Application.Helpers;
using Application.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileShareWebUI2.Helpers
{
    public class LocalDirectoryHelper : IDirectoryHelper
    {
        private static List<string> PictureExts = new List<string>() { ".jpg", ".png", ".gif" };
        private static List<string> VideoExts = new List<string>() { ".mp4", ".mov", ".flv", ".ts", ".mkv", ".avi", ".wmv", ".3gp", ".3gpp", ".webm", ".vob", ".m4v" };
        private static List<string> PdfExts = new List<string>() { ".pdf" };
        private static List<string> DocExts = new List<string>() { ".doc", ".docx" };
        private static List<string> SpreadSheetExts = new List<string>() { ".xls", ".xlsx" };
        private static List<string> PresentationExts = new List<string>() { ".ppt", ".pptx" };
        private static List<string> AudioExts = new List<string>() { ".mp3", ".wav", ".flac", ".m4a", ".wma" };
        private static List<string> CompressExts = new List<string>() { ".zip", ".rar", ".7z" };
        private static Dictionary<string, string> Mimes = new Dictionary<string, string>()
        { {".mp4","video/mp4"},{".mov","video/quicktime"},{".flv","video/x-flv"},{".ts","video/mp2t"},{".mkv","video/x-matroska"},{ ".avi","video/x-msvideo"}
        ,{ ".wmv","video/x-ms-wmv"},{ ".3gp","video/3gpp"},{ ".3gpp","video/3gpp"},{ ".webm","video/webm"},{ ".vob","video/dvd video/mpeg"},{ ".m4v","video/x-m4v"},
            { ".mp3","audio/mpeg"},{ ".wav","audio/x-wav"},{ ".flac","audio/x-flac"},{ ".m4a","audio/m4a"},{ ".wma","audio/x-ms-wma"} };
        public bool CheckDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public bool CreateFolder(string path, string VirtualPath, bool isLocal = true)
        {
            throw new NotImplementedException();
        }

        public bool CreateSubFolder(string path)
        {
            throw new NotImplementedException();
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

        public List<DirectoryContent> GetFolderContent(string RootPath, string VirtualPath, int height = 1, long rootId = 0, long startIndex = 0)
        {
            List<DirectoryContent> result = new List<DirectoryContent>();
            var FirstLevelContents = GetChildElements(RootPath, VirtualPath, height, rootId, startIndex);
            result.AddRange(FirstLevelContents);
            height++;
            foreach (var node in FirstLevelContents.Where(p => p.ContentType == FolderContentType.Folder).ToList())
            {
                result.AddRange(GetFolderContent(node.Path, string.Concat(VirtualPath, node.Path.Replace(RootPath, "").Replace('\\', '/')), height, node.Id, result.Max(p => p.Id)));
            }
            return result;
        }
        private List<DirectoryContent> GetChildElements(string path, string vpath, int height, long rootId, long startIndex)
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
                        MimeType = "",
                        Title = dir.Split('\\').Last(),
                        HeightLevel = height,
                        RootFolderId = rootId,
                        FileContentType = FileContentType.Unknown,
                        Vpath = string.Concat(vpath, dir.Replace(path, "").Replace('\\', '/')).ToLower().Replace(" ", "%20")
                    });
                }
                foreach (var dir in Directory.GetFiles(path))
                {
                    var tmpFormat = Path.GetExtension(dir);
                    var tmpFileType = GetFileType(dir);
                    if (tmpFileType == FileContentType.Video)
                    {
                        if (!File.Exists(dir.Replace(tmpFormat, ".vtt")))
                        {
                            if (File.Exists(dir.Replace(tmpFormat, ".srt")))
                                ConvertSRTToVTT(dir.Replace(tmpFormat, ".srt"), 0, false);
                        }
                    }
                    result.Add(new DirectoryContent()
                    {
                        Id = ++startIndex,
                        ContentType = FolderContentType.File,
                        Path = dir,
                        Format = tmpFormat,
                        MimeType = string.Format("{0}", tmpFileType == FileContentType.Video || tmpFileType == FileContentType.Audio ? Mimes.FirstOrDefault(p => p.Key == tmpFormat).Value : ""),
                        Title = Path.GetFileName(dir),
                        HeightLevel = height,
                        RootFolderId = rootId,
                        FileContentType = tmpFileType,
                        Vpath = string.Concat(vpath, dir.Replace(path, "").Replace('\\', '/')).ToLower().Replace(" ", "%20")
                    });
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        //srt to vtt convert section
        private static readonly Regex _rgxCueID = new Regex(@"^\d+$");
        private static readonly Regex _rgxTimeFrame = new Regex(@"(\d\d:\d\d:\d\d(?:[,.]\d\d\d)?) --> (\d\d:\d\d:\d\d(?:[,.]\d\d\d)?)");
        public static void ConvertSRTToVTT(string filePath, int offsetMilliseconds, bool readANSI)
        {
            using (var srtReader = readANSI ? new StreamReader(filePath, Encoding.Default) : new StreamReader(filePath))
            using (var vttWriter = new StreamWriter(filePath.Replace(".srt", ".vtt")))
            {
                vttWriter.WriteLine("WEBVTT"); // Starting line for the WebVTT files
                vttWriter.WriteLine("");

                string srtLine;
                while ((srtLine = srtReader.ReadLine()) != null)
                {
                    if (_rgxCueID.IsMatch(srtLine)) // Ignore cue ID number lines
                    {
                        continue;
                    }

                    Match match = _rgxTimeFrame.Match(srtLine);
                    if (match.Success) // Format the time frame to VTT format (and handle offset)
                    {
                        var startTime = TimeSpan.Parse(match.Groups[1].Value.Replace(',', '.'));
                        var endTime = TimeSpan.Parse(match.Groups[2].Value.Replace(',', '.'));

                        if (offsetMilliseconds != 0)
                        {
                            double startTimeMs = startTime.TotalMilliseconds + offsetMilliseconds;
                            double endTimeMs = endTime.TotalMilliseconds + offsetMilliseconds;

                            startTime = TimeSpan.FromMilliseconds(startTimeMs < 0 ? 0 : startTimeMs);
                            endTime = TimeSpan.FromMilliseconds(endTimeMs < 0 ? 0 : endTimeMs);
                        }

                        srtLine =
                            startTime.ToString(@"hh\:mm\:ss\.fff") +
                            " --> " +
                            endTime.ToString(@"hh\:mm\:ss\.fff");
                    }

                    vttWriter.WriteLine(srtLine);
                }
            }
        }
        public bool RemoveFolder(string path, string VirtualPath, bool isLocal = true)
        {
            throw new NotImplementedException();
        }
    }
}