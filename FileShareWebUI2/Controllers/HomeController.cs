using Application.DTO.Folder;
using Application.DTO.User;
using Application.Helper;
using Application.Model;
using FileShareWebUI2.Helpers;
using FileShareWebUI2.ViewModels;
using ICSharpCode.SharpZipLib.Zip;
using NReco.VideoConverter;
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
            try
            {
                var Content = new List<UserDTO>();
                if (CacheHelper.GetCachedUserData().IsAdmin)
                {
                    var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/Get");
                    if (!result.IsSuccessfulResult())
                        return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                    Content = ApiHelper.Deserialize<List<UserDTO>>(result.Content);
                }
                return View(Content);
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public ActionResult AddUser()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> EditUser(Guid UserId)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/GetUserById/{UserId}");
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                var Content = ApiHelper.Deserialize<UpdateUserDTO>(result.Content);
                Content.Password = CacheHelper.DecryptResult(Content.Password);
                return View(Content);
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> UserDetail(Guid UserId)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/GetUserById/{UserId}");
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                var Content = ApiHelper.Deserialize<UserDTO>(result.Content);
                return View(Content);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorHandling", new { Code = 500 });
            }
        }
        [HttpPost]
        public async Task<ActionResult> SubmitAddUser(CreateUserDTO user)
        {
            try
            {
                if (CacheHelper.GetCachedUserData().IsAdmin)
                {
                    //user.Password = Encryption.EncryptStringView(user.Password.Trim());
                    var body = ApiHelper.Serialize(user);
                    var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/User/Post", body);
                    if (!result.IsSuccessfulResult())
                        return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                    if (ApiHelper.Deserialize<bool>(result.Content))
                        TempData["UserSuccess"] = "user created successfully.";
                    else
                        TempData["UserError"] = "error occured in creating user.try again later.";
                    return RedirectToAction("Users");
                }
                else
                    return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        [HttpPost]
        public async Task<ActionResult> SubmitEditUser(UpdateUserDTO user)
        {
            try
            {
                if (CacheHelper.GetCachedUserData().IsAdmin)
                {
                    //user.Password = Encryption.EncryptStringView(user.Password.Trim());
                    var body = ApiHelper.Serialize(user);
                    var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/User/Patch", body);
                    if (!result.IsSuccessfulResult())
                        return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                    if (ApiHelper.Deserialize<bool>(result.Content))
                        TempData["UserSuccess"] = "user edited successfully.";
                    else
                        TempData["UserError"] = "error occured in editing user.try again later.";
                    return RedirectToAction("Users");
                }
                else
                    return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> SubmitDeleteUser(Guid UserId)
        {
            try
            {
                if (CacheHelper.GetCachedUserData().IsAdmin)
                {
                    if (CacheHelper.userData.UserId != UserId)
                    {
                        var result = await ApiHelper.Call(ApiHelper.HttpMethods.Delete, $"{BaseApiAddress}/User/Delete/{UserId}");
                        if (!result.IsSuccessfulResult())
                            return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
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
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> UserProfile()
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/GetUserById/{CacheHelper.GetCachedUserData().UserId}");
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                var Content = ApiHelper.Deserialize<UserDTO>(result.Content);
                return View(Content);
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }

        //folder crud section

        public async Task<ActionResult> Folders()
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFoldersByUserId/{CacheHelper.GetCachedUserData().UserId}");
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                var Content = ApiHelper.Deserialize<List<FolderDTO>>(result.Content);
                return View(Content);
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public ActionResult AddFolder()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> EditFolder(Guid FolderId)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{FolderId}");
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                var Content = ApiHelper.Deserialize<UpdateFolderDTO>(result.Content);
                return View(Content);
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> FolderDetail(Guid FolderId)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{FolderId}");
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                var Content = ApiHelper.Deserialize<FolderDTO>(result.Content);
                return View(Content);
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        [HttpPost]
        public async Task<ActionResult> SubmitAddFolder(CreateFolderDTO folder)
        {
            try
            {
                folder.UserId = CacheHelper.GetCachedUserData().UserId;
                var body = ApiHelper.Serialize(folder);
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/Folder/Post", body);
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                if (ApiHelper.Deserialize<bool>(result.Content))
                    TempData["FolderSuccess"] = "folder created successfully.";
                else
                    TempData["FolderError"] = "error occured in creating folder.try again later.";
                IsFoldersChanged = true;
                return RedirectToAction("Folders");
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        [HttpPost]
        public async Task<ActionResult> SubmitEditFolder(UpdateFolderDTO folder)
        {
            try
            {
                var body = ApiHelper.Serialize(folder);
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/Folder/Patch", body);
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                if (ApiHelper.Deserialize<bool>(result.Content))
                    TempData["FolderSuccess"] = "folder edited successfully.";
                else
                    TempData["FolderError"] = "error occured in editing folder.try again later.";
                IsFoldersChanged = true;
                return RedirectToAction("Folders");
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> SubmitDeleteFolder(Guid FolderId)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Delete, $"{BaseApiAddress}/Folder/Delete/{FolderId}");
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                if (ApiHelper.Deserialize<bool>(result.Content))
                    TempData["FolderSuccess"] = "folder deleted successfully.";
                else
                    TempData["FolderError"] = "error occured in deleting folder.try again later.";
                IsFoldersChanged = true;
                return RedirectToAction("Folders");
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }

        //folder page cache view

        public async Task<ActionResult> Folder(Guid Id,long FolderId = 0)
        {
            try
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
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> SubFolder(Guid RootFolderId, long FolderId)
        {
            try
            {
                FolderViewModel fvm = new FolderViewModel();
                fvm.Folder = await CacheHelper.GetCachedFolder(RootFolderId);
                fvm.Contents = await CacheHelper.GetCachedDirectoryContentById(RootFolderId, FolderId);
                fvm.Directory = await CacheHelper.GetCachedContentById(RootFolderId, FolderId);
                return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_FolderPartial", fvm) });
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }

        //create new folder

        [HttpPost]
        public async Task<ActionResult> AddSubFolder(string FolderName, string RootPath, Guid RootFolderId,long RootFolderDirectoryId)
        {
            try
            {
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/Folder/AddSubFolder", ApiHelper.Serialize(Path.Combine(RootPath, FolderName)));
                if (!result.IsSuccessfulResult())
                    return Json(new { Successfull = false });
                else
                {
                    if (ApiHelper.Deserialize<bool>(result.Content))
                    {
                        var contents = await CacheHelper.GetCachedDirectoryContentById(RootFolderId, RootFolderDirectoryId, true);
                        FolderViewModel fvm = new FolderViewModel() { Contents = contents, Folder = await CacheHelper.GetCachedFolder(RootFolderId), Directory = await CacheHelper.GetCachedContentById(RootFolderId, RootFolderDirectoryId) };
                        return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_FolderTableContentPartial", fvm) });
                    }
                    else
                        return Json(new { isSuccessfull = false });
                }
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
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
            try
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
            catch (Exception)
            {
                return "error";
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadComplete(string fileName, string RootPath, Guid RootFolderId, long RootFolderDirectoryId)
        {
            try
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
            catch (Exception)
            {
                return Json(new { isSuccessfull = false });
            }
        }

        private static void MergeFiles(string file1, string file2)
        {
            try
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
            catch (Exception)
            {
            }
        }

        //download file in folder

        public async Task<bool> DownloadFile(long FileId, Guid RootFolderId)
        {
            try
            {
                Stream stream = null;
                var file = await CacheHelper.GetCachedContentById(RootFolderId, FileId);
                int bytesToRead = 200000;
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
            catch (Exception)
            {
                return false;
            }
        }
        //view file in folder

        public async Task<ActionResult> ViewFile(long FileId, Guid RootFolderId)
        {
            try
            {
                FileViewModel fvm = new FileViewModel();
                fvm.CurrentFile = await CacheHelper.GetCachedContentById(RootFolderId, FileId);
                fvm.RootFolderId = RootFolderId;
                if (fvm.CurrentFile.FileContentType == FileContentType.Doc || fvm.CurrentFile.FileContentType == FileContentType.Presentation ||
                    fvm.CurrentFile.FileContentType == FileContentType.Pdf || fvm.CurrentFile.FileContentType == FileContentType.SpreadSheet)
                    return View("DocViewer", fvm);
                if (fvm.CurrentFile.FileContentType == FileContentType.Audio || fvm.CurrentFile.FileContentType == FileContentType.Video ||
                    fvm.CurrentFile.FileContentType == FileContentType.Picture)
                {
                    fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentById(RootFolderId, fvm.CurrentFile.RootFolderId, FileId, fvm.CurrentFile.FileContentType);
                    return View("MultimediaViewer", fvm);
                }
                else
                    return RedirectToAction("DownloadFile", new { FileId = FileId, RootFolderId = RootFolderId });
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> ViewFileJson(long FileId, Guid RootFolderId)
        {
            try
            {
                FileViewModel fvm = new FileViewModel();
                fvm.CurrentFile = await CacheHelper.GetCachedContentById(RootFolderId, FileId);
                fvm.RootFolderId = RootFolderId;
                fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentById(RootFolderId, fvm.CurrentFile.RootFolderId, FileId, fvm.CurrentFile.FileContentType);
                return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_MultimediaViewerPartial", fvm) });
            }
            catch (Exception)
            {
                return Json(new { isSuccessfull = false });
            }
        }
        public async Task<ActionResult> MediaGallery(long FolderId, Guid RootFolderId,FileContentType FileType)
        {
            try
            {
                FileViewModel fvm = new FileViewModel();
                fvm.RootFolderId = RootFolderId;
                fvm.FolderId = FolderId;
                fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentForGallery(RootFolderId, FolderId, FileType, 0,
                    FileType == FileContentType.Video ? 9 : FileType == FileContentType.Audio ? 30 : 60);
                fvm.GalleryContext = FileType;
                return View(fvm);
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> MediaGalleryPagination(long FolderId, Guid RootFolderId, FileContentType FileType,int currentPage)
        {
            try
            {
                FileViewModel fvm = new FileViewModel();
                fvm.RootFolderId = RootFolderId;
                fvm.FolderId = FolderId;
                fvm.RelatedFiles = await CacheHelper.GetCachedRelatedContentForGallery(RootFolderId, FolderId, FileType
                    , currentPage, FileType == FileContentType.Video ? 9 : FileType == FileContentType.Audio ? 15 : 30);
                fvm.GalleryContext = FileType;
                return Json(new { isSuccessfull = true, PageContent = ViewHelper.RenderViewToString(this, "_MediaGalleryPartial", fvm), noMore = fvm.RelatedFiles.Count == 0 ? true : false });
            }
            catch (Exception)
            {
                return Json(new { isSuccessfull = false });
            }
        }
        public async Task<bool> DownloadAllFiles(int DirectoryId,Guid RootFolderId,string FolderName)
        {
            var dirContent = (await CacheHelper.GetCachedDirectoryContentById(RootFolderId,DirectoryId)).Where(p => p.ContentType == FolderContentType.File).GroupBy(p => p.Path).Select(p => p.Key).ToList();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + FolderName + ".zip");
            Response.ContentType = "application/zip";

            using (var zipStream = new ZipOutputStream(Response.OutputStream))
            {
                foreach (string filePath in dirContent)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                    var fileEntry = new ZipEntry(Path.GetFileName(filePath))
                    {
                        Size = fileBytes.Length
                    };

                    zipStream.PutNextEntry(fileEntry);
                    zipStream.Write(fileBytes, 0, fileBytes.Length);
                }

                zipStream.Flush();
                zipStream.Close();
            }
            return true;
        }
        public async Task<bool> ConvertToMp4(long FileId, Guid RootFolderId)
        {
            try
            {
                FileViewModel fvm = new FileViewModel();
                fvm.CurrentFile = await CacheHelper.GetCachedContentById(RootFolderId, FileId);
                fvm.RootFolderId = RootFolderId;
                var stream = new FileStream(fvm.CurrentFile.Path.Replace(fvm.CurrentFile.Format, ".mp4"), FileMode.Create);
                var converter = new FFMpegConverter();
                converter.ConvertMedia(fvm.CurrentFile.Path,fvm.CurrentFile.Format.Replace(".", ""), stream, Format.mp4, new ConvertSettings() {  VideoFrameSize = FrameSize.hd720 });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //general section
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl = "")
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    TempData["RedirectUrl"] = ReturnUrl;
                    return View();
                }
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SubmitLogin(LoginCredential credential)
        {
            try
            {
                var body = ApiHelper.Serialize(credential);
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/User/LoginUser", body);
                if (!result.IsSuccessfulResult())
                    return View("ErrorHandling", new Tuple<int, string>((int)result.ResultCode, ""));
                var Content = ApiHelper.Deserialize<LoginResult>(result.Content);
                if (Content.successLogin)
                {
                    //build cookie
                    var userDataCookie = new HttpCookie("FSCookie");
                    userDataCookie.Values.Add("UserId", Content.User.Id.ToString());
                    userDataCookie.Values.Add("UserLevel", Content.User.IsAdmin ? "Admin" : "Simple");
                    userDataCookie.Values.Add("Fullname", Content.User.Fullname);
                    userDataCookie.HttpOnly = true;
                    userDataCookie.Expires = DateTime.Now.AddHours(8);
                    Response.Cookies.Add(userDataCookie);
                    FormsAuthentication.SetAuthCookie(Content.User.Username, true);
                    CacheHelper.createInstance();
                    if (string.IsNullOrWhiteSpace(credential.ReturnUrl))
                        return RedirectToAction("Index");
                    else
                    {
                        try
                        {
                            return Redirect(credential.ReturnUrl);
                        }
                        catch (Exception)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    TempData["LoginError"] = Content.message;
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public ActionResult Logout()
        {
            try
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
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
        public async Task<ActionResult> Index()
        {
            try
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
            catch (Exception ex)
            {
                return View("ErrorHandling", new Tuple<int, string>(500, ex.Message));
            }
        }
    }
}