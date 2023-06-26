using Application.Features.Folder.Request;
using Application.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Folder.Hanlder.Command
{
    public class DeleteFolderRequestHandler : IRequestHandler<DeleteFolderRequest, bool>
    {
        private readonly IFolderRepository _folderRepository;

        public DeleteFolderRequestHandler(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }
        public async Task<bool> Handle(DeleteFolderRequest request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetFolderById(request.Id);
            if (folder == null) return false;
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
                return result;
            }
        }
    }
}
