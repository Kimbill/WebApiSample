using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController(IConfiguration configuration, IUserRepository userRepository, 
            ILogger<AuthController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto adduser)
        {

            var responseObject = new ResponseObject<string>()
            {
                StatusCode = 400
            };
            //throw new Exception("This application must stop here");
            
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  
                }

            var user = await _userManager.FindByEmailAsync(adduser.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, adduser.Password, false, false);

                    if (result.Succeeded)
                    {
                        var jwt = new Utilities(_configuration);
                        var token = jwt.GenerateJwt(user);
                        responseObject.Message = "Login Successful";
                        responseObject.StatusCode = 200;
                        responseObject.Data = token;
                        return Ok(responseObject);
                    }

                    else
                    {
                        responseObject.Message = "Invalid password";
                        return BadRequest(responseObject);
                    }

                }
                responseObject.Message = "Email not yet confirmed";
                return BadRequest(responseObject);

            }

            responseObject.Message = "Invalid Email";
            return BadRequest(responseObject);
            //var user =  _userRepository.GetByEmail(adduser.Email);
            //if(user != null)
            //{
            //    if (user.PasswordHash == adduser.Password)
            //    {
            //        var jwt = new Utilities(_configuration);
            //        var token = jwt.GenerateJwt(user);
            //        return Ok(token);
            //    }

            //    return BadRequest("Invalid Password credential");
            //}

            //return BadRequest("Invalid Email Credential");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto adduser)
        {
            //get the email to send a password reset token to
            var responseObject = new ResponseObject<string>()
            {
                StatusCode = 400
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //generate token to reset password

            var user = await _userManager.FindByEmailAsync(adduser.Email);
            
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", new { email = adduser.Email, token });
                //_logger.LogError(link);
                //responseObject.Message = "Confirmation token sent to your mail";
                //responseObject.StatusCode = 200;
                //return Ok(responseObject);
                responseObject.Message = "Confirmation token sent to your mail";
                responseObject.StatusCode = 200;
                return Ok(responseObject);
            }

            responseObject.Message = $"user with email: {adduser.Email} was not found";
            return BadRequest(responseObject);
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto adduser)
        {
            var responseObject = new ResponseObject<string>()
            {
                StatusCode = 400
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var user = await _userManager.FindByEmailAsync(adduser.Email);

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, adduser.Token, adduser.NewPassword);
                if (result.Succeeded)
                {
                    responseObject.Message = "Password reset Successful";
                    responseObject.StatusCode = 200;
                    return Ok();
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                        responseObject.Message = "Password reset Failed";
                        //responseObject.ErrorMessages.Add(item.Description);
                        return BadRequest(ModelState);
                    }
                }
            }
            responseObject.Message = $"user with email: {adduser.Email} was not found";
            return BadRequest(responseObject);

        }
    }
}
