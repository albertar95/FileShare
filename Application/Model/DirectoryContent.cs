using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class DirectoryContent
    {
        public long Id { get; set; }
        public FolderContentType ContentType { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Vpath { get; set; }
        public string Format { get; set; }
        public int HeightLevel { get; set; }
        public long RootFolderId { get; set; }
        public long SecondaryIndex { get; set; }
        public FileContentType FileContentType { get; set; }
    }
    public enum FolderContentType { File = 1, Folder = 2 }
    public enum FileContentType { Picture = 1, Video = 2, Pdf = 3, Doc = 4, Compress = 5, Audio = 6, SpreadSheet = 7, Presentation = 8, Unknown = 0 }
}
