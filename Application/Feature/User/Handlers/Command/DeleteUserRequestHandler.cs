using Application.Features.User.Request;
using Application.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.User.Handler.Command
{
    public class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFolderRepository _folderRepository;

        public DeleteUserRequestHandler(IUserRepository userRepository,IFolderRepository folderRepository)
        {
            _userRepository = userRepository;
            _folderRepository = folderRepository;
        }
        public async Task<bool> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.Id);
            if (user == null) return false;
            else
            {
                if ((await _folderRepository.GetFoldersByUserId(user.Id)).Any()) return false;
                else
                {
                    var result = await _userRepository.Delete(user);
                    return result;
                }

            }
        }
    }
}
