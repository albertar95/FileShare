using Application.DTO.User;
using Application.EntityMapper.Contract;
using Application.Model;
using Application.Persistence;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FileShareApi.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private readonly IEntityMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository, IEntityMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CreateUserDTO user)
        {
            try
            {
                return Ok(await _userRepository.Add(_mapper.EntityMap<Domain.User>(user)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPatch]
        public async Task<IHttpActionResult> Patch([FromBody] UpdateUserDTO user)
        {
            try
            {
                var Currentuser = _userRepository.GetUserById(user.Id);
                if (Currentuser == null) return NotFound();
                else
                {
                    Currentuser = _mapper.EntityMap<User>(user, Currentuser);
                    if (await _userRepository.Update(Currentuser))
                        return Ok();
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
        public async Task<IHttpActionResult> Delete(Guid userId)
        {
            try
            {
                var Currentuser = _userRepository.GetUserById(userId);
                if (Currentuser == null) return NotFound();
                else
                {
                    if (await _userRepository.Delete(Currentuser))
                        return Ok();
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
        public IHttpActionResult Get(bool IncludeDisabled = true,int Skip = 0,int PageSize = 100)
        {
            try
            {
                return Ok(_mapper.EntityMap<List<UserDTO>>(_userRepository.GetUsers(IncludeDisabled, Skip, PageSize)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("GetUserById/{UserId}")]
        public IHttpActionResult GetUserById(Guid UserId)
        {
            try
            {
                return Ok(_mapper.EntityMap<UserDTO>(_userRepository.GetUserById(UserId)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public IHttpActionResult LoginUser([FromBody] LoginCredential user)
        {
            try
            {
                var loginResult = _userRepository.LoginUser(user.Username, user.Password);
                switch (loginResult.Item1)
                {
                    case 1:
                        return Ok(new { successLogin = false, message = "user not found" });
                    case 2:
                        return Ok(new { successLogin = false, message = "user is disabled" });
                    case 3:
                        return Ok(new { successLogin = false, message = "password not match" });
                    case 4:
                        return Ok(new { successLogin = true, message = "login successfull", CurrentUser = loginResult.Item2 });
                    default:
                        return Ok(new { successLogin = false, message = "error not specified" });

                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
