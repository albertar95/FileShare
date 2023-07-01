using Application.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    public class LoginResult
    {
        public bool successLogin { get; set; }
        public string message { get; set; }
        public UserDTO User { get; set; }
    }
}
