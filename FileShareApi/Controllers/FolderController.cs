using Application.DTO.Folder;
using Application.Helper;
using Application.Helpers;
using Application.Persistence;
using Application.Service.EntityMapper;
using Domain;
using FileShareApi.Auth.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

namespace FileShareApi.Controllers
{
    [RoutePrefix("api/Folder")]
    public class FolderController : ApiController
    {
        private readonly IEntityMapper _mapper;
        private readonly IFolderRepository _folderRepository;
        private readonly IDirectoryHelper _directoryHelper;
        private readonly IAccessControl _accessControl;
        private readonly IEncryptionHelper _encryptionHelper;
        public FolderController(IFolderRepository folderRepository,IEntityMapper mapper,IDirectoryHelper directoryHelper, IAccessControl accessControl, IEncryptionHelper encryptionHelper)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _directoryHelper = directoryHelper;
            _accessControl = accessControl;
            _encryptionHelper = encryptionHelper;
        }
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CreateFolderDTO folder)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    folder.Id = Guid.NewGuid();
                    if (folder.IsLocal)
                    {
                        if (!_directoryHelper.CheckDirectory(folder.Path))
                            return InternalServerError(new Exception("folder path not exists."));
                    }
                    else
                        folder.Path = $"{ConfigurationManager.AppSettings["RootVirtualFolders"]}\\{folder.Id}";
                    if (_directoryHelper.CreateFolder(folder.Path, folder.Id.ToString(), folder.IsLocal))
                    {
                        //folder.VirtualPath = $"http://localhost:5394/content/files/demo";
                        folder.VirtualPath = $"http://{ConfigurationManager.AppSettings["ServerIp"]}/fs/{folder.Id}";
                        return Ok(await _folderRepository.Add(_mapper.EntityMap<Domain.Folder>(folder)));
                    }
                    else
                        return InternalServerError(new Exception("error in creating folder or virtual directory"));
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> Patch([FromBody] UpdateFolderDTO folder)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    var CurrentFolder = _folderRepository.GetFolderById(folder.Id);
                    if (CurrentFolder == null) return NotFound();
                    else
                    {
                        CurrentFolder = _mapper.EntityMap<Folder>(folder, CurrentFolder);
                        if (await _folderRepository.Update(CurrentFolder))
                            return Ok(true);
                        else
                            return InternalServerError();
                    }
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    var folder = _folderRepository.GetFolderById(id);
                    if (folder == null) return NotFound();
                    else
                    {
                        var result = await _folderRepository.Delete(folder);
                        if (result)
                        {
                            _directoryHelper.RemoveFolder(folder.Path, folder.VirtualPath, folder.IsLocal);
                            return Ok(true);
                        }
                        else
                            return InternalServerError();
                    }
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("GetFoldersByUserId/{UserId}")]
        public IHttpActionResult GetFoldersByUserId(Guid UserId, bool IncludePublics = false, bool HasAdminAccess = false, int Skip = 0, int PageSize = 100)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    return Ok(_mapper.EntityMap<List<FolderDTO>>(_folderRepository.GetFoldersByUserId(UserId, IncludePublics, HasAdminAccess, Skip, PageSize)));
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("GetFolderById/{FolderId}")]
        public IHttpActionResult GetFolderById(Guid FolderId)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    return Ok(_mapper.EntityMap<FolderDTO>(_folderRepository.GetFolderById(FolderId)));
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("AddSubFolder")]
        public IHttpActionResult AddSubFolder([FromBody] string path)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    return Ok(_directoryHelper.CreateSubFolder(path));
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
