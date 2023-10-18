using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger _logger;
        public AuthController(IConfiguration configuration, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto adduser)
        {
            throw new Exception("This application must stop here");
            
            if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  
                }

                var user =  _userRepository.GetByEmail(adduser.Email);
                if(user != null)
                {
                    if (user.PasswordHash == adduser.Password)
                    {
                        var jwt = new Utilities(_configuration);
                        var token = jwt.GenerateJwt(user);
                        return Ok(token);
                    }

                    return BadRequest("Invalid Password credential");
                }

                return BadRequest("Invalid Email Credential");
        }
    }
}
