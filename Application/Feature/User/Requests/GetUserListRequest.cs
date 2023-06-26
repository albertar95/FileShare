using Application.DTO.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.User.Request
{
    public class GetUserListRequest : IRequest<List<UserDTO>>
    {
        public bool IncludeDisabled { get; set; }
        public int Skip { get; set; } = 0;
        public int PageSize { get; set; } = 100;
    }
}
