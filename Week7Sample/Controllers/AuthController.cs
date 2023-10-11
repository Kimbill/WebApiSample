using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Week7Sample.Common.Security;
using Week7Sample.Data.Repositories.Interfaces;
using Week7Sample.Model;

namespace Week7Sample.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public AuthController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto adduser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  
                }

                var user =  _userRepository.GetByEmail(adduser.Email);
                if(user != null)
                {
                    if (user.Password == adduser.Password)
                    {
                        var jwt = new Utilities(_configuration);
                        var token = jwt.GenerateJwt(user);
                        return Ok(token);
                    }

                    return BadRequest("Invalid Password credential");
                }

                return BadRequest("Invalid Email Credential");
            }

            catch(Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
