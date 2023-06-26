using Application.Features.Folder.Request;
using Application.Persistence;
using AutoMapper;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Folder.Hanlder.Command
{
    public class AddFolderRequestHandler : IRequestHandler<AddFolderRequest, bool>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper _mapper;

        public AddFolderRequestHandler(IFolderRepository folderRepository, IMapper mapper)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
        }
        public async Task<bool> Handle(AddFolderRequest request, CancellationToken cancellationToken)
        {
            return await _folderRepository.Add(_mapper.Map<Domain.User>(request.Folder));
        }
    }
}
