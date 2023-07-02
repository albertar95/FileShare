using Application.DTO.User;
using Application.EntityMapper.Contract;
using Application.Helper;
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
                user.Password = Encryption.MapFromViewToPersistence(user.Password);
                return Ok(await _userRepository.Add(_mapper.EntityMap<Domain.User>(user)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> Patch([FromBody] UpdateUserDTO user)
        {
            try
            {
                var Currentuser = _userRepository.GetUserById(user.Id);
                if (Currentuser == null) return NotFound();
                else
                {
                    user.Password = Encryption.MapFromViewToPersistence(user.Password);
                    Currentuser = _mapper.EntityMap<User>(user, Currentuser);
                    if (await _userRepository.Update(Currentuser))
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
                var Currentuser = _userRepository.GetUserById(id);
                if (Currentuser == null) return NotFound();
                else
                {
                    if (await _userRepository.Delete(Currentuser))
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
        public IHttpActionResult Get(bool IncludeDisabled = true,int Skip = 0,int PageSize = 100)
        {
            try
            {
                var result = new List<UserDTO>();
                var users = _userRepository.GetUsers(IncludeDisabled, Skip, PageSize).ToList();
                foreach (var usr in users)
                {
                    result.Add(_mapper.EntityMap<UserDTO>(usr));
                }
                result.ForEach(p => { p.Password = Encryption.MapFromPersistenceToView(p.Password); });
                return Ok(result);
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
                var result = new UserDTO();
                var user = _userRepository.GetUserById(UserId);
                result = _mapper.EntityMap<UserDTO>(user);
                result.Password = Encryption.MapFromPersistenceToView(user.Password);
                return Ok(result);
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
                user.Password = Encryption.MapFromViewToPersistence(user.Password);
                var loginResult = _userRepository.LoginUser(user.Username, user.Password);
                LoginResult result = new LoginResult() {  successLogin = false, User = null };
                switch (loginResult.Item1)
                {
                    case 1:
                        result.message = "user not found";
                        break;
                    case 2:
                        result.message = "user is disabled";
                        break;
                    case 3:
                        result.message = "password not match";
                        break;
                    case 4:
                        result.successLogin = true;
                        result.message = "login successfull";
                        result.User = _mapper.EntityMap<UserDTO>(loginResult.Item2);
                        result.User.Password = Encryption.MapFromPersistenceToView(loginResult.Item2.Password);
                        break;
                    default:
                        result.message = "error not specified";
                        break;

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
