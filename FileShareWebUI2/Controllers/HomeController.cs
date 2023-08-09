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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FileShareWebUI2.Controllers
{
    public class HomeController : Controller
    {
        private static string BaseApiAddress = ConfigurationManager.AppSettings["BaseApiAddress"];

        //user section

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

            return RedirectToAction("Login");
        }
        public async Task<ActionResult> Users()
        {
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                var Content = new List<UserDTO>();
                if (Request.Cookies["FSCookie"].Values["UserLevel"] == "Admin")
                {
                    var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/Get");
                    if (!result.IsSuccessfulResult())
                        return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                    Content = ApiHelper.Deserialize<List<UserDTO>>(result.Content);
                }
                return View(Content);
            }
            else
                return RedirectToAction("Login");
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
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                if (Request.Cookies["FSCookie"].Values["UserLevel"] == "Admin")
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
            else
                return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<ActionResult> SubmitEditUser(UpdateUserDTO user)
        {
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                if (Request.Cookies["FSCookie"].Values["UserLevel"] == "Admin")
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
            else
                return RedirectToAction("Login");
        }
        public async Task<ActionResult> SubmitDeleteUser(Guid UserId)
        {
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                if (Request.Cookies["FSCookie"].Values["UserLevel"] == "Admin")
                {
                    if (Request.Cookies["FSCookie"].Values["UserId"] != UserId.ToString())
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
            }
            else
                TempData["UserError"] = "in order to delete user,login first";
            return RedirectToAction("Users");
        }
        public async Task<ActionResult> UserProfile()
        {
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                var userId = Request.Cookies["FSCookie"].Values["UserId"];
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/User/GetUserById/{userId}");
                if (!result.IsSuccessfulResult())
                    return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                var Content = ApiHelper.Deserialize<UserDTO>(result.Content);
                return View(Content);
            }
            else
                return RedirectToAction("Login");
        }

        //folder section

        public async Task<ActionResult> Folders()
        {
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                var userId = Request.Cookies["FSCookie"].Values["UserId"];
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFoldersByUserId/{userId}");
                if (!result.IsSuccessfulResult())
                    return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                var Content = ApiHelper.Deserialize<List<FolderDTO>>(result.Content);
                return View(Content);
            }
            else
                return RedirectToAction("Login");
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
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                folder.UserId = Guid.Parse(Request.Cookies["FSCookie"].Values["UserId"]);
                var body = ApiHelper.Serialize(folder);
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Post, $"{BaseApiAddress}/Folder/Post", body);
                if (!result.IsSuccessfulResult())
                    return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                if (ApiHelper.Deserialize<bool>(result.Content))
                    TempData["FolderSuccess"] = "folder created successfully.";
                else
                    TempData["FolderError"] = "error occured in creating folder.try again later.";
                return RedirectToAction("Folders");
            }
            else
                return RedirectToAction("Login");
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
            return RedirectToAction("Folders");
        }

        //folder page and viewers

        public async Task<ActionResult> Folder(Guid Id)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{Id}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            FolderViewModel fvm = new FolderViewModel();
            fvm.Folder = ApiHelper.Deserialize<FolderDTO>(result.Content);
            //fvm.Contents = DirectoryHelper.GetFolderContent(fvm.Folder.Path);
            fvm.ReturnUrl = Url.Action("Index", "Home");
            return View(fvm);
        }
        public async Task<ActionResult> SubFolder(Guid FolderId, string FolderPath)
        {
            var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, $"{BaseApiAddress}/Folder/GetFolderById/{FolderId}");
            if (!result.IsSuccessfulResult())
                return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
            FolderViewModel fvm = new FolderViewModel();
            fvm.Folder = ApiHelper.Deserialize<FolderDTO>(result.Content);
            if (FolderPath != fvm.Folder.Path)
                fvm.ReturnUrl = Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FolderPath).FullName });
            else
                fvm.ReturnUrl = Url.Action("Index", "Home");
            fvm.Folder.Path = FolderPath;
            //fvm.Contents = DirectoryHelper.GetFolderContent(fvm.Folder.Path);
            return View("Folder", fvm);
        }
        [HttpPost]
        public ActionResult AddSubFolder(string FolderName, string RootPath, Guid FolderId)
        {
            //if (DirectoryHelper.CreateNewFolder(Path.Combine(RootPath, FolderName)))
            //    TempData["FolderPageSuccess"] = "folder created successfully";
            //else
            //    TempData["FolderPageError"] = "error occured in creating folder";
            return RedirectToAction("SubFolder", new { FolderId = FolderId, FolderPath = RootPath });
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase FileUpload, string RootPath, Guid FolderId)
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
            return RedirectToAction("SubFolder", new { FolderId = FolderId, FolderPath = RootPath });
        }
        public ActionResult DownloadFile(string FilePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
            string fileName = Path.GetFileName(FilePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public ActionResult ViewFile(string FilePath, Guid FolderId)
        {
            //switch (DirectoryHelper.GetFileType(FilePath))
            //{
            //    case FileContentType.Picture:
            //        return View("DocViewer", "", new List<string>() { "files\\\\demo.jpg"
            //            , Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FilePath).FullName }).ToString() });
            //    case FileContentType.Video:
            //        return View("MovieViewer", "", new List<string>() { "files\\\\demo.mp4"
            //            , Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FilePath).FullName }).ToString() });
            //    case FileContentType.Pdf:
            //        return View("DocViewer", "", new List<string>() { "files\\\\demo.pdf"
            //            , Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FilePath).FullName }).ToString() });
            //    case FileContentType.Doc:
            //        return View("DocViewer", "", new List<string>() { "files\\\\demo.docx"
            //            , Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FilePath).FullName }).ToString() });
            //    case FileContentType.Audio:
            //        return View("AudioViewer", "", new List<string>() { "files\\\\demo.mp3"
            //            , Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FilePath).FullName }).ToString() });
            //    case FileContentType.SpreadSheet:
            //        return View("DocViewer", "", new List<string>() { "files\\\\demo.xlsx"
            //            , Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FilePath).FullName }).ToString() });
            //    case FileContentType.Presentation:
            //        return View("DocViewer", "", new List<string>() { "files\\\\demo.pptx"
            //            , Url.Action("SubFolder", "Home", new { FolderId = FolderId, FolderPath = Directory.GetParent(FilePath).FullName }).ToString() });
            //    case FileContentType.Compress:
            //        byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
            //        string fileName = Path.GetFileName(FilePath);
            //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //    case FileContentType.Unknown:
            //        return RedirectToAction("ErrorHandling");
            //    default:
            //        return RedirectToAction("ErrorHandling");
            //}
            return View();
        }

        //general section

        public async Task<ActionResult> Index()
        {
            if (Request.Cookies.AllKeys.Contains("FSCookie"))
            {
                var userId = Request.Cookies["FSCookie"].Values["UserId"];
                var UserLevel = Request.Cookies["FSCookie"].Values["UserLevel"];
                string curl = "";
                if (UserLevel == "Admin")
                    curl = $"{BaseApiAddress}/Folder/GetFoldersByUserId/{userId}?IncludePublics=true&HasAdminAccess=true";
                else
                    curl = $"{BaseApiAddress}/Folder/GetFoldersByUserId/{userId}?IncludePublics=true";
                var result = await ApiHelper.Call(ApiHelper.HttpMethods.Get, curl);
                if (!result.IsSuccessfulResult())
                    return RedirectToAction("ErrorHandling", new { Code = (int)result.ResultCode });
                var Content = ApiHelper.Deserialize<List<FolderDTO>>(result.Content);
                return View(Content);
            }
            else
                return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public ActionResult ErrorHandling(int Code = 0)
        {
            return View(Code);
        }
    }
}