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
        private static string BaseApiAddress = ConfigurationManager.AppSettings["BaseApiAddress"];
        private CacheHelper()
        {
            DirectoryRepository = new Dictionary<Guid, List<DirectoryContent>>();
            Folders = new List<FolderDTO>();
        }
        public static CacheHelper createInstance()
        {
            if (cacheHelper == null)
                cacheHelper = new CacheHelper();
            return cacheHelper;
        }
        public static void DirectoryRepositoryUpdate(Guid Id,List<DirectoryContent> Content)
        {
            if(DirectoryRepository.Any(p => p.Key == Id))
                DirectoryRepository.Remove(Id);
            DirectoryRepository.Add(Id, Content);

        }
        private static void FetchUserData()
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
        private static async Task<bool> FetchDirectoryContent(Guid Id)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderContentTreeById/{Id}");
            if (result.IsSuccessfulResult())
            {
                DirectoryRepository.Add(Id, ApiHelper.Deserialize<List<DirectoryContent>>(result.Content));
                return true;
            }
            else
                return false;
        }
        private static async Task<bool> FetchFolder(Guid Id)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{Id}");
            if (result.IsSuccessfulResult())
            {
                Folders.Add(ApiHelper.Deserialize<FolderDTO>(result.Content));
                return true;
            }
            else
                return false;
        }
        private static async Task<bool> FetchFolders(Guid UserId, bool HasAdminAccess = false, bool IncludePublics = true)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFoldersByUserId/{UserId}?IncludePublics={IncludePublics}&HasAdminAccess={HasAdminAccess}");
            if (result.IsSuccessfulResult())
            {
                Folders.Clear();
                Folders.AddRange(ApiHelper.Deserialize<List<FolderDTO>>(result.Content));
                return true;
            }
            else
                return false;
        }
        public static async Task<List<DirectoryContent>> GetCachedAllDirectoryContent(Guid Id,int Height = 1, bool refresh = false)
        {
            if(!refresh)
            {
                if (!DirectoryRepository.Any(p => p.Key == Id))
                {
                    if (!await FetchDirectoryContent(Id))
                        return new List<DirectoryContent>();
                }
            }else
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
        public static async Task<List<DirectoryContent>> GetCachedDirectoryContentById(Guid FolderId, long Id, bool refresh = false)
        {
            if(!refresh)
            {
                if (!DirectoryRepository.Any(p => p.Key == FolderId))
                {
                    if (!await FetchDirectoryContent(FolderId))
                        return new List<DirectoryContent>();
                }
            }else
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
        public static async Task<DirectoryContent> GetCachedContentById(Guid FolderId, long Id, bool refresh = false)
        {
            if(!refresh)
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
        public static async Task<List<DirectoryContent>> GetCachedRelatedContentById(Guid FolderId, long RootFolderId,long FileId,FileContentType FileType, bool refresh = false)
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
            Where(p => p.RootFolderId == RootFolderId && p.FileContentType == FileType).ToList(),FileId);
        }
        public static async Task<List<DirectoryContent>> GetCachedRelatedContentForGallery(Guid FolderId, long RootFolderId, FileContentType FileType,int skip = 0,int PageSize = 10, bool refresh = false)
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
            Where(p => p.RootFolderId == RootFolderId && p.FileContentType == FileType).ToList().Skip(skip*PageSize).Take(PageSize).ToList();
            long counter = skip * PageSize + 1;
            result.ForEach(x => { x.SecondaryIndex = counter;counter++; });
            return result;
        }
        private static List<DirectoryContent> GetMedianRelatedContents(List<DirectoryContent> overallContent,long FileId)
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
            else if(fileIndex == (overallContent.Count - 2))
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
        public static async Task<List<FolderDTO>> GetCachedFolders(Guid UserId, bool HasAdminAccess = false, bool IncludePublics = true, bool refresh = false)
        {
            if(refresh)
            {
                if (!await FetchFolders(UserId,IncludePublics,HasAdminAccess))
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
        public static async Task<FolderDTO> GetCachedFolder(Guid Id, bool refresh = false)
        {
            if(!refresh)
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
        public static UserData GetCachedUserData(bool refresh = false)
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

        public static void ReleaseResources()
        {
            DirectoryRepository.Clear();
            Folders.Clear();
            cacheHelper = null;
            GC.Collect();
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