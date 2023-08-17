using Application.DTO.Folder;
using Application.DTO.User;
using Application.Helper;
using Application.Model;
using FileShareWebUI2.Helpers;
using FileShareWebUI2.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FileShareWebUI2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private static string BaseApiAddress = ConfigurationManager.AppSettings["BaseApiAddress"];
        private static string UploadTempFolder = ConfigurationManager.AppSettings["FileUploadTempFolderPath"];
        private static bool IsFoldersChanged = false;
        private CacheHelper _cacheHelper { get; set; }

        //user crud section

        public async Task<ActionResult> Users()
        {
            var Content = new List<UserDTO>();
            if (CacheHelper.GetCachedUserData().IsAdmin)
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/Get");
                if (!result.IsSuccessfulResult())
                    return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                Content = ApiHelper.Deserialize<List<UserDTO>>(result.Content);
            }
            return View(Content);
        }
        public ActionResult AddUser()
        {
            return View();
        }
        public async Task<ActionResult> EditUser(Guid UserId)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/GetUserById/{UserId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            var Content = ApiHelper.Deserialize<UpdateUserDTO>(result.Content);
            Content.Password = Encoding.UTF8.GetString(Convert.FromBase64String(Content.Password));
            return View(Content);
        }
        public async Task<ActionResult> UserDetail(Guid UserId)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/GetUserById/{UserId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            var Content = ApiHelper.Deserialize<UserDTO>(result.Content);
            return View(Content);
        }
        [HttpPost]
        public async Task<ActionResult> SubmitAddUser(CreateUserDTO user)
        {
            if (CacheHelper.GetCachedUserData().IsAdmin)
            {
                //user.Password = Encryption.EncryptStringView(user.Password.Trim());
                var body = ApiHelper.Serialize(user);
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/User/Post", body);
                if (!result.IsSuccessfulResult())
                    return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                if (ApiHelper.Deserialize<bool>(result.Content))
                    TempData["UserSuccess"] = "user created successfully.";
                else
                    TempData["UserError"] = "error occured in creating user.try again later.";
                return RedirectToAction("Users");
            }
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<ActionResult> SubmitEditUser(UpdateUserDTO user)
        {
            if (CacheHelper.GetCachedUserData().IsAdmin)
            {
                //user.Password = Encryption.EncryptStringView(user.Password.Trim());
                var body = ApiHelper.Serialize(user);
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/User/Patch", body);
                if (!result.IsSuccessfulResult())
                    return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                if (ApiHelper.Deserialize<bool>(result.Content))
                    TempData["UserSuccess"] = "user edited successfully.";
                else
                    TempData["UserError"] = "error occured in editing user.try again later.";
                return RedirectToAction("Users");
            }
            else
                return RedirectToAction("Login");
        }
        public async Task<ActionResult> SubmitDeleteUser(Guid UserId)
        {
            if (CacheHelper.GetCachedUserData().IsAdmin)
            {
                if (CacheHelper.userData.UserId != UserId)
                {
                    var result = await ApiHelper.Call(ApiHelper.HttpMethods.Delete, $"{BaseApiAddress}/User/Delete/{UserId}");
                    if (!result.IsSuccessfulResult())
                        return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                    if (ApiHelper.Deserialize<bool>(result.Content))
                        TempData["UserSuccess"] = "user deleted successfully.";
                    else
                        TempData["UserError"] = "error occured in deleting user.try again later.";
                }
                else
                    TempData["UserError"] = "user can not delete itself";
            }
            else
                TempData["UserError"] = "current user dont have access to complete this action.";
            return RedirectToAction("Users");
        }
        public async Task<ActionResult> UserProfile()
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/GetUserById/{CacheHelper.GetCachedUserData().UserId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            var Content = ApiHelper.Deserialize<UserDTO>(result.Content);
            return View(Content);
        }

        //folder crud section

        public async Task<ActionResult> Folders()
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFoldersByUserId/{CacheHelper.GetCachedUserData().UserId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            var Content = ApiHelper.Deserialize<List<FolderDTO>>(result.Content);
            return View(Content);
        }
        public ActionResult AddFolder()
        {
            return View();
        }
        public async Task<ActionResult> EditFolder(Guid FolderId)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{FolderId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            var Content = ApiHelper.Deserialize<UpdateFolderDTO>(result.Content);
            return View(Content);
        }
        public async Task<ActionResult> FolderDetail(Guid FolderId)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{FolderId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            var Content = ApiHelper.Deserialize<FolderDTO>(result.Content);
            return View(Content);
        }
        [HttpPost]
        public async Task<ActionResult> SubmitAddFolder(CreateFolderDTO folder)
        {
            folder.UserId = CacheHelper.GetCachedUserData().UserId;
            var body = ApiHelper.Serialize(folder);
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/Folder/Post", body);
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            if (ApiHelper.Deserialize<bool>(result.Content))
                TempData["FolderSuccess"] = "folder created successfully.";
            else
                TempData["FolderError"] = "error occured in creating folder.try again later.";
            IsFoldersChanged = true;
            return RedirectToAction("Folders");
        }
        [HttpPost]
        public async Task<ActionResult> SubmitEditFolder(UpdateFolderDTO folder)
        {
            var body = ApiHelper.Serialize(folder);
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/Folder/Patch", body);
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            if (ApiHelper.Deserialize<bool>(result.Content))
                TempData["FolderSuccess"] = "folder edited successfully.";
            else
                TempData["FolderError"] = "error occured in editing folder.try again later.";
            IsFoldersChanged = true;
            return RedirectToAction("Folders");
        }
        public async Task<ActionResult> SubmitDeleteFolder(Guid FolderId)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Delete, $"{BaseApiAddress}/Folder/Delete/{FolderId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            if (ApiHelper.Deserialize<bool>(result.Content))
                TempData["FolderSuccess"] = "folder deleted successfully.";
            else
                TempData["FolderError"] = "error occured in deleting folder.try again later.";
            IsFoldersChanged = true;
            return RedirectToAction("Folders");
        }

        //folder page cache view

        public async Task<ActionResult> Folder(Guid Id,long FolderId = 0)
        {
            FolderViewModel fvm = new FolderViewModel();
            fvm.Folder = await CacheHelper.GetCachedFolder(Id);
            fvm.Directory = await CacheHelper.GetCachedContentById(Id, FolderId);
            if (FolderId == 0)
                fvm.Contents = await CacheHelper.GetCachedAllDirectoryContent(fvm.Folder.Id);
            else
                fvm.Contents = await CacheHelper.GetCachedDirectoryContentById(Id, FolderId);
            return View(fvm);
        }
        public async Task<ActionResult> SubFolder(Guid RootFolderId, long FolderId)
        {
            FolderViewModel fvm = new FolderViewModel();
            fvm.Folder = await CacheHelper.GetCachedFolder(RootFolderId);
            fvm.Contents = await CacheHelper.GetCachedDirectoryContentById(RootFolderId, FolderId);
            fvm.Directory = await CacheHelper.GetCachedContentById(RootFolderId, FolderId);
            return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_FolderPartial", fvm) });
        }

        //create new folder

        [HttpPost]
        public async Task<ActionResult> AddSubFolder(string FolderName, string RootPath, Guid RootFolderId,long RootFolderDirectoryId)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/Folder/AddSubFolder",ApiHelper.Serialize(Path.Combine(RootPath, FolderName)));
            if (!result.IsSuccessfulResult())
                return Json(new { Successfull = false });
            else
            {
                if (ApiHelper.Deserialize<bool>(result.Content))
                {
                    var contents = await CacheHelper.GetCachedDirectoryContentById(RootFolderId, RootFolderDirectoryId, true);
                    return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_FolderTableContentPartial", contents) });
                }
                else
                    return Json(new { isSuccessfull = false });
            }
        }

        //upload file in folder

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileUpload, string RootPath, Guid RootFolderId, long RootFolderDirectoryId)
        {
            try
            {
                if (FileUpload.ContentLength > 0)
                {
                    string FileName = Path.GetFileName(FileUpload.FileName);
                    string path = Path.Combine(RootPath, FileName);
                    FileUpload.SaveAs(path);
                    TempData["FolderPageSuccess"] = "file uploaded successfully";
                }
            }
            catch
            {
                TempData["FolderPageError"] = "error occured in uploading file";
            }
            return RedirectToAction("SubFolder", new { FolderId = RootFolderId, FolderPath = RootPath });
        }


        [HttpPost]
        public string MultiUpload(string id, string fileName)
        {
            var chunkNumber = id;
            var chunks = Request.InputStream;
            string newpath = Path.Combine(UploadTempFolder, fileName + chunkNumber);
            using (FileStream fs = System.IO.File.Create(newpath))
            {
                byte[] bytes = new byte[3757000];
                int bytesRead;
                while ((bytesRead = Request.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    fs.Write(bytes, 0, bytesRead);
                }
            }
            return "done";
        }

        [HttpPost]
        public async Task<ActionResult> UploadComplete(string fileName, string RootPath, Guid RootFolderId, long RootFolderDirectoryId)
        {
            string newPath = Path.Combine(UploadTempFolder, fileName);
            string[] filePaths = Directory.GetFiles(UploadTempFolder).Where(p => p.Contains(fileName)).OrderBy(p => Int32.Parse(p.Replace(fileName, "$").Split('$')[1])).ToArray();
            foreach (string filePath in filePaths)
            {
                MergeFiles(newPath, filePath);
            }
            System.IO.File.Move(Path.Combine(UploadTempFolder, fileName), Path.Combine(RootPath, fileName));
            var contents = await CacheHelper.GetCachedDirectoryContentById(RootFolderId, RootFolderDirectoryId, true);
            return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_FolderTableContentPartial", contents) });
        }

        private static void MergeFiles(string file1, string file2)
        {
            FileStream fs1 = null;
            FileStream fs2 = null;
            try
            {
                fs1 = System.IO.File.Open(file1, FileMode.Append);
                fs2 = System.IO.File.Open(file2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                if (fs1 != null) fs1.Close();
                if (fs2 != null) fs2.Close();
                System.IO.File.Delete(file2);
            }
        }

        //download file in folder

        public async Task<bool> DownloadFile(long FileId, Guid RootFolderId)
        {
            Stream stream = null;
            var file = await CacheHelper.GetCachedContentById(RootFolderId,FileId);
            int bytesToRead = 50000;
            byte[] buffer = new Byte[bytesToRead];
            try
            {
                HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(file.Vpath);
                HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();
                if (fileReq.ContentLength > 0)
                    fileResp.ContentLength = fileReq.ContentLength;
                stream = fileResp.GetResponseStream();
                var resp = Response;
                resp.ContentType = MediaTypeNames.Application.Octet;
                resp.AddHeader("Content-Disposition", "attachment; filename=\"" + file.Title + "\"");
                resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());
                int length;
                do
                {
                    if (resp.IsClientConnected)
                    {
                        length = stream.Read(buffer, 0, bytesToRead);
                        resp.OutputStream.Write(buffer, 0, length);
                        resp.Flush();
                        buffer = new Byte[bytesToRead];
                    }
                    else
                    {
                        length = -1;
                    }
                } while (length > 0); //Repeat until no data is read
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return true;
        }
        //view file in folder

        public async Task<ActionResult> ViewFile(long FileId, Guid RootFolderId)
        {
            FileViewModel fvm = new FileViewModel();
            fvm.CurrentFile = await CacheHelper.GetCachedContentById(RootFolderId, FileId);
            fvm.RootFolderId = RootFolderId;
            if (fvm.CurrentFile.FileContentType == FileContentType.Doc || fvm.CurrentFile.FileContentType == FileContentType.Presentation ||
                fvm.CurrentFile.FileContentType == FileContentType.Pdf || fvm.CurrentFile.FileContentType == FileContentType.SpreadSheet)
                    return View("DocViewer",fvm);
            if (fvm.CurrentFile.FileContentType == FileContentType.Audio || fvm.CurrentFile.FileContentType == FileContentType.Video ||
                fvm.CurrentFile.FileContentType == FileContentType.Picture)
            {
                fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentById(RootFolderId, fvm.CurrentFile.RootFolderId, FileId, fvm.CurrentFile.FileContentType);
                return View("MultimediaViewer", fvm);
            }
            else
                return RedirectToAction("DownloadFile",new { FileId = FileId , RootFolderId = RootFolderId });
        }
        public async Task<ActionResult> ViewFileJson(long FileId, Guid RootFolderId)
        {
            FileViewModel fvm = new FileViewModel();
            fvm.CurrentFile = await CacheHelper.GetCachedContentById(RootFolderId, FileId);
            fvm.RootFolderId = RootFolderId;
            fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentById(RootFolderId, fvm.CurrentFile.RootFolderId,FileId, fvm.CurrentFile.FileContentType);
            return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_MultimediaViewerPartial", fvm) });
        }
        public async Task<ActionResult> MediaGallery(long FolderId, Guid RootFolderId,FileContentType FileType)
        {
            FileViewModel fvm = new FileViewModel();
            fvm.RootFolderId = RootFolderId;
            fvm.FolderId = FolderId;
            fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentForGallery(RootFolderId, FolderId,FileType,0,
                FileType == FileContentType.Video ? 9 : FileType == FileContentType.Audio ? 30 : 60);
            return View(fvm);
        }
        public async Task<ActionResult> MediaGalleryPagination(long FolderId, Guid RootFolderId, FileContentType FileType,int currentPage)
        {
            FileViewModel fvm = new FileViewModel();
            fvm.RootFolderId = RootFolderId;
            fvm.FolderId = FolderId;
            fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentForGallery(RootFolderId, FolderId, FileType
                , currentPage,FileType == FileContentType.Video ? 9 : FileType == FileContentType.Audio ? 30 : 60);
            return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_MediaGalleryPartial",fvm),noMore = fvm.RelatedFiles.Count == 0 ? true : false });
        }

        //general section
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SubmitLogin(LoginCredential credential)
        {
            //credential.Password = Encryption.EncryptStringView(credential.Password);
            var body = ApiHelper.Serialize(credential);
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/User/LoginUser", body);
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            var Content = ApiHelper.Deserialize<LoginResult>(result.Content);
            if (Content.successLogin)
            {
                //build cookie
                var userDataCookie = new HttpCookie("FSCookie");
                userDataCookie.Values.Add("UserId", Content.User.Id.ToString());
                userDataCookie.Values.Add("UserLevel", Content.User.IsAdmin ? "Admin" : "Simple");
                userDataCookie.Values.Add("Fullname", Content.User.Fullname);
                userDataCookie.Secure = true;
                userDataCookie.HttpOnly = true;
                userDataCookie.Expires = DateTime.Now.AddHours(8);
                Response.Cookies.Add(userDataCookie);
                FormsAuthentication.SetAuthCookie(Content.User.Username, true);
                CacheHelper.createInstance();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["LoginError"] = Content.message;
                return RedirectToAction("Login");
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            if (Request.Cookies["FSCookie"] != null)
            {
                var c = new HttpCookie("FSCookie")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(c);
            }
            try
            {
                CacheHelper.ReleaseResources();
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Login");
        }
        public async Task<ActionResult> Index()
        {
            var userdata = CacheHelper.GetCachedUserData();
            List<FolderDTO> Content = null;
            if (!IsFoldersChanged)
                Content = await CacheHelper.GetCachedFolders(userdata.UserId, userdata.IsAdmin);
            else
            {
                Content = await CacheHelper.GetCachedFolders(userdata.UserId, userdata.IsAdmin, true, true);
                IsFoldersChanged = false;
            }
            return View(Content);
        }
        [AllowAnonymous]
        public ActionResult ErrorHandling(int Code = 0)
        {
            return View(Code);
        }
    }
}