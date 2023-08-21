using Application.DTO.Folder;
using Application.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FileShareWebUI2.Helpers
{
    public class CacheHelper : IDisposable
    {
        private static CacheHelper cacheHelper = null;
        public static Dictionary<Guid, List<DirectoryContent>> DirectoryRepository = new Dictionary<Guid, List<DirectoryContent>>();
        public static List<FolderDTO> Folders = new List<FolderDTO>();
        public static UserData userData = new UserData();
        public static string accessToken = "";
        public static long timestamp = 1;
        public static LocalEncryptionHelper _encryptionHelper = new LocalEncryptionHelper();
        public static LocalDirectoryHelper _directoryHelper = new LocalDirectoryHelper();
        private static string BaseApiAddress = ConfigurationManager.AppSettings["BaseApiAddress"];
        private static string VPTemplate = ConfigurationManager.AppSettings["VPTemplate"];

        private CacheHelper()
        {
            DirectoryRepository = new Dictionary<Guid, List<DirectoryContent>>();
            Folders = new List<FolderDTO>();
            userData = new UserData();
            accessToken = "";
            _encryptionHelper = new LocalEncryptionHelper();
            _directoryHelper = new LocalDirectoryHelper();
            timestamp = 1;
        }
        public static CacheHelper createInstance()
        {
            try
            {
                if (cacheHelper == null)
                    cacheHelper = new CacheHelper();
            }
            catch (Exception)
            {
            }
            return cacheHelper;
        }
        private static void FetchUserData()
        {
            try
            {
                UserData userdata = new UserData();
                if (HttpContext.Current.Request.Cookies.AllKeys.Contains("FSCookie"))
                {
                    userdata.UserId = Guid.Parse(HttpContext.Current.Request.Cookies["FSCookie"].Values["UserId"]);
                    userdata.Fullname = HttpContext.Current.Request.Cookies["FSCookie"].Values["Fullname"];
                    userdata.IsAdmin = HttpContext.Current.Request.Cookies["FSCookie"].Values["UserLevel"].ToString().ToLower() == "admin" ? true : false;
                }
                userData = userdata;
            }
            catch (Exception)
            {
            }
        }
        private static async Task<bool> FetchDirectoryContent(Guid Id)
        {
            try
            {
                var tmpFolder = await GetCachedFolder(Id);
                var tmpDirectoryContents = _directoryHelper.GetFolderContent(tmpFolder.Path, tmpFolder.VirtualPath);
                tmpDirectoryContents.Add(new Application.Model.DirectoryContent()
                {
                    Id = 0,
                    ContentType = Application.Model.FolderContentType.Folder,
                    Format = ".dir",
                    HeightLevel = 0,
                    Title = tmpFolder.Title,
                    Path = tmpFolder.Path,
                    RootFolderId = -1,
                    FileContentType = Application.Model.FileContentType.Unknown,
                    Vpath = tmpFolder.VirtualPath
                });
                DirectoryRepository.Add(Id, tmpDirectoryContents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static async Task<bool> FetchFolder(Guid Id)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{Id}");
                if (result.IsSuccessfulResult())
                {
                    var tmpFolder = ApiHelper.Deserialize<FolderDTO>(result.Content);
                    //tmpFolder.VirtualPath = string.Format("{0}/{1}",VPTemplate,tmpFolder.Id);
                    tmpFolder.VirtualPath = VPTemplate;
                    Folders.Add(tmpFolder);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static async Task<bool> FetchFolders(Guid UserId, bool HasAdminAccess = false, bool IncludePublics = true)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFoldersByUserId/{UserId}?IncludePublics={IncludePublics}&HasAdminAccess={HasAdminAccess}");
                if (result.IsSuccessfulResult())
                {
                    Folders.Clear();
                    var tmpFolders = ApiHelper.Deserialize<List<FolderDTO>>(result.Content);
                    //tmpFolders.ForEach(x => { x.VirtualPath = string.Format("{0}/{1}", VPTemplate, x.Id); });
                    tmpFolders.ForEach(x => { x.VirtualPath = VPTemplate; });
                    Folders.AddRange(tmpFolders);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<List<DirectoryContent>> GetCachedAllDirectoryContent(Guid Id,int Height = 1, bool refresh = false)
        {
            try
            {
                if (!refresh)
                {
                    if (!DirectoryRepository.Any(p => p.Key == Id))
                    {
                        if (!await FetchDirectoryContent(Id))
                            return new List<DirectoryContent>();
                    }
                }
                else
                {
                    if (DirectoryRepository.Any(p => p.Key == Id))
                    {
                        DirectoryRepository.Remove(Id);
                    }
                    if (!await FetchDirectoryContent(Id))
                        return new List<DirectoryContent>();
                }
                return DirectoryRepository.FirstOrDefault(p => p.Key == Id).Value.Where(p => p.HeightLevel == Height).ToList();
            }
            catch (Exception)
            {
                return new List<DirectoryContent>();
            }
        }
        public static async Task<List<DirectoryContent>> GetCachedDirectoryContentById(Guid FolderId, long Id, bool refresh = false)
        {
            try
            {
                if (!refresh)
                {
                    if (!DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        if (!await FetchDirectoryContent(FolderId))
                            return new List<DirectoryContent>();
                    }
                }
                else
                {
                    if (DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        DirectoryRepository.Remove(FolderId);
                    }
                    if (!await FetchDirectoryContent(FolderId))
                        return new List<DirectoryContent>();
                }
                return DirectoryRepository.FirstOrDefault(p => p.Key == FolderId).Value.Where(p => p.RootFolderId == Id).ToList();
            }
            catch (Exception)
            {
                return new List<DirectoryContent>();
            }
        }
        public static async Task<DirectoryContent> GetCachedContentById(Guid FolderId, long Id, bool refresh = false)
        {
            try
            {
                if (!refresh)
                {
                    if (!DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        if (!await FetchDirectoryContent(FolderId))
                            return new DirectoryContent();
                    }
                }
                else
                {
                    if (DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        DirectoryRepository.Remove(FolderId);
                    }
                    if (!await FetchDirectoryContent(FolderId))
                        return new DirectoryContent();
                }
                return DirectoryRepository.FirstOrDefault(p => p.Key == FolderId).Value.FirstOrDefault(p => p.Id == Id);
            }
            catch (Exception)
            {
                return new DirectoryContent();
            }
        }
        public static async Task<List<DirectoryContent>> GetCachedRelatedContentById(Guid FolderId, long RootFolderId,long FileId,FileContentType FileType, bool refresh = false)
        {
            try
            {
                if (!refresh)
                {
                    if (!DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        if (!await FetchDirectoryContent(FolderId))
                            return new List<DirectoryContent>();
                    }
                }
                else
                {
                    if (DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        DirectoryRepository.Remove(FolderId);
                    }
                    if (!await FetchDirectoryContent(FolderId))
                        return new List<DirectoryContent>();
                }
                return GetMedianRelatedContents(DirectoryRepository.FirstOrDefault(p => p.Key == FolderId).Value.
                Where(p => p.RootFolderId == RootFolderId && p.FileContentType == FileType).ToList(), FileId);
            }
            catch (Exception)
            {
                return new List<DirectoryContent>();
            }
        }
        public static async Task<List<DirectoryContent>> GetCachedRelatedContentForGallery(Guid FolderId, long RootFolderId, FileContentType FileType,int skip = 0,int PageSize = 10, bool refresh = false)
        {
            try
            {
                List<DirectoryContent> result = new List<DirectoryContent>();
                if (!refresh)
                {
                    if (!DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        if (!await FetchDirectoryContent(FolderId))
                            return new List<DirectoryContent>();
                    }
                }
                else
                {
                    if (DirectoryRepository.Any(p => p.Key == FolderId))
                    {
                        DirectoryRepository.Remove(FolderId);
                    }
                    if (!await FetchDirectoryContent(FolderId))
                        return new List<DirectoryContent>();
                }
                result = DirectoryRepository.FirstOrDefault(p => p.Key == FolderId).Value.
                Where(p => p.RootFolderId == RootFolderId && p.FileContentType == FileType).ToList().Skip(skip * PageSize).Take(PageSize).ToList();
                long counter = skip * PageSize + 1;
                result.ForEach(x => { x.SecondaryIndex = counter; counter++; });
                return result;
            }
            catch (Exception)
            {
                return new List<DirectoryContent>();
            }
        }
        private static List<DirectoryContent> GetMedianRelatedContents(List<DirectoryContent> overallContent,long FileId)
        {
            try
            {
                if (overallContent.Count < 5)
                    return overallContent.Where(p => p.Id != FileId).ToList();
                int counter = 0;
                Dictionary<int, long> ids = new Dictionary<int, long>();
                foreach (var item in overallContent.OrderBy(p => p.Id).ToList())
                {
                    ids.Add(counter, item.Id);
                    counter++;
                }
                int fileIndex = ids.FirstOrDefault(p => p.Value == FileId).Key;
                List<long> candidateIds = new List<long>();
                if (fileIndex == 0)
                {
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 1).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 2).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 1)).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 2)).Value);
                }
                else if (fileIndex == 1)
                {
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 0).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 2).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 3).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 1)).Value);
                }
                else if (fileIndex == (overallContent.Count - 1))
                {
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 0).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 1).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 3)).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 2)).Value);
                }
                else if (fileIndex == (overallContent.Count - 2))
                {
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == 0).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 1)).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 3)).Value);
                    candidateIds.Add(ids.FirstOrDefault(p => p.Key == (overallContent.Count - 4)).Value);
                }
                else
                {
                    candidateIds = ids.Where(p => p.Key >= (fileIndex - 2) && p.Key <= (fileIndex + 2) && p.Key != fileIndex).Select(q => q.Value).ToList();
                }
                return overallContent.Where(p => candidateIds.Contains(p.Id)).ToList();
            }
            catch (Exception)
            {
                return new List<DirectoryContent>();
            }
        }
        public static async Task<List<FolderDTO>> GetCachedFolders(Guid UserId, bool HasAdminAccess = false, bool IncludePublics = true, bool refresh = false)
        {
            try
            {
                if (refresh)
                {
                    if (!await FetchFolders(UserId, IncludePublics, HasAdminAccess))
                        return new List<FolderDTO>();
                }
                else
                {
                    if (!Folders.Any())
                    {
                        if (!await FetchFolders(UserId, IncludePublics, HasAdminAccess))
                            return new List<FolderDTO>();
                    }
                }
                return Folders;
            }
            catch (Exception)
            {
                return new List<FolderDTO>();
            }
        }
        public static async Task<FolderDTO> GetCachedFolder(Guid Id, bool refresh = false)
        {
            try
            {
                if (!refresh)
                {
                    if (!Folders.Any(p => p.Id == Id))
                    {
                        if (!await FetchFolder(Id))
                            return new FolderDTO();
                    }
                }
                else
                {
                    if (Folders.Any(p => p.Id == Id))
                        Folders.RemoveAll(p => p.Id == Id);

                    if (!await FetchFolder(Id))
                        return new FolderDTO();
                }
                return Folders.FirstOrDefault(p => p.Id == Id);
            }
            catch (Exception)
            {
                return new FolderDTO();
            }
        }
        public static UserData GetCachedUserData(bool refresh = false)
        {
            try
            {
                if (!refresh)
                {
                    if (userData.UserId == Guid.Empty)
                        FetchUserData();
                }
                else
                    FetchUserData();
                return userData;
            }
            catch (Exception)
            {
                return userData;
            }
        }
        public static string GetCachedAccessToken()
        {
            try
            {
                if (timestamp != 1)
                {
                    if (DateTime.Now.Subtract(new DateTime(timestamp)) > TimeSpan.FromMinutes(1))
                    {
                        timestamp = DateTime.Now.Ticks;
                        accessToken = _encryptionHelper.GenerateToken(HttpContext.Current.Request.UserHostAddress, timestamp.ToString());
                    }
                }
                else
                {
                    timestamp = DateTime.Now.Ticks;
                    accessToken = _encryptionHelper.GenerateToken(HttpContext.Current.Request.UserHostAddress, timestamp.ToString());
                }
                return accessToken;
            }
            catch (Exception)
            {
                return accessToken;
            }
        }
        public static string DecryptResult(string cipher)
        {
            try
            {
                return _encryptionHelper.LocalRSADecrypt(cipher);
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }

        public static void ReleaseResources()
        {
            try
            {
                DirectoryRepository.Clear();
                Folders.Clear();
                cacheHelper = null;
                GC.Collect();
            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
        }
    }
    public class UserData
    {
        public Guid UserId { get; set; }
        public string Fullname { get; set; }
        public bool IsAdmin { get; set; }
    }
}