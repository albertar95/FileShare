using Application.DTO.User;
using Application.Helper;
using Application.Model;
using Application.Persistence;
using Application.Service.EntityMapper;
using Domain;
using FileShareApi.Auth;
using FileShareApi.Auth.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FileShareApi.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private readonly IEntityMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionHelper _encryptionHelper;
        private readonly IAccessControl _accessControl;
        public UserController(IUserRepository userRepository, IEntityMapper mapper,IEncryptionHelper encryptionHelper,IAccessControl accessControl)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _encryptionHelper = encryptionHelper;
            _accessControl = accessControl;
        }
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CreateUserDTO user)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    user.Password = _encryptionHelper.EncryptString(user.Password);
                    return Ok(await _userRepository.Add(_mapper.EntityMap<Domain.User>(user)));
                }
                else
                {
                    return Unauthorized();
                }
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
                if (_accessControl.Check(Request))
                {
                    var Currentuser = _userRepository.GetUserById(user.Id);
                    if (Currentuser == null) return NotFound();
                    else
                    {
                        user.Password = _encryptionHelper.EncryptString(user.Password);
                        Currentuser = _mapper.EntityMap<User>(user, Currentuser);
                        if (await _userRepository.Update(Currentuser))
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
                else
                    return Unauthorized();
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
                if (_accessControl.Check(Request))
                    return Ok(_mapper.EntityMap<List<UserDTO>>(_userRepository.GetUsers(IncludeDisabled, Skip, PageSize).ToList()));
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        public IHttpActionResult GetUserById(Guid id)
        {
            try
            {
                if (_accessControl.Check(Request))
                {
                    var result = new UserDTO();
                    var user = _userRepository.GetUserById(id);
                    result = _mapper.EntityMap<UserDTO>(user);
                    result.Password = _encryptionHelper.LocalRSAEncrypt(_encryptionHelper.DecryptString(user.Password));
                    return Ok(result);
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
        public IHttpActionResult LoginUser([FromBody] LoginCredential user)
        {
            try
            {
                user.Password = _encryptionHelper.EncryptString(user.Password);
                var loginResult = _userRepository.LoginUser(user.Username, user.Password);
                LoginResult result = new LoginResult() { successLogin = false, User = null };
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
