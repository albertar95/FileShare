using Application.DTO.Folder;
using Application.EntityMapper.Contract;
using Application.Persistence;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FileShareApi.Controllers
{
    [RoutePrefix("api/Folder")]
    public class FolderController : ApiController
    {
        private readonly IEntityMapper _mapper;
        private readonly IFolderRepository _folderRepository;
        public FolderController(IFolderRepository folderRepository,IEntityMapper mapper)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CreateFolderDTO folder)
        {
            try
            {
                return Ok(await _folderRepository.Add(_mapper.EntityMap<Domain.Folder>(folder)));
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
                var folder = _folderRepository.GetFolderById(id);
                if (folder == null) return NotFound();
                else
                {
                    var result = await _folderRepository.Delete(folder);
                    if (result)
                    {
                        try
                        {
                            if (!folder.IsLocal)
                                System.IO.Directory.Delete(folder.Path);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (result)
                        return Ok(true);
                    else
                        return InternalServerError();
                }
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
                return Ok(_mapper.EntityMap<List<FolderDTO>>(_folderRepository.GetFoldersByUserId(UserId, IncludePublics, HasAdminAccess, Skip, PageSize)));
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
                return Ok(_mapper.EntityMap<FolderDTO>(_folderRepository.GetFolderById(FolderId)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
