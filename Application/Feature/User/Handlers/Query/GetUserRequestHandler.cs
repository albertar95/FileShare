﻿using Application.DTO.User;
using Application.Features.User.Request;
using Application.Persistence;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.User.Handler.Query
{
    public class GetUserRequestHandler : IRequestHandler<GetUserRequest, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserRequestHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserDTO> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            return _mapper.Map<UserDTO>(await _userRepository.GetUserById(request.Id));
        }
    }
}
