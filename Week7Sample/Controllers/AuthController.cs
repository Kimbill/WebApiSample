using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Week7Sample.Common.Security;
using Week7Sample.Data.Repositories.Interfaces;
using Week7Sample.Model;

namespace Week7Sample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                        var jwt = new Utilities(_configuration, _userManager);
                        var token = await jwt.GenerateJwt(user);
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
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("ResetPassword", "Auth", new { email = adduser.Email, token }, Request.Scheme);
                var link = Url.Action("ResetPassword", new { email = adduser.Email, token });

                // Example email sending functionality
                //var emailService = new MockEmailService(); // Use the mock service
                //var emailBody = $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>";
                //await emailService.SendEmailAsync(adduser.Email, "Reset Password", emailBody);


                responseObject.Message = "Password reset link has been sent to your email";
                responseObject.StatusCode = 200;
                return Ok(responseObject);
            }

            responseObject.Message = $"User with email {adduser.Email} was not found";
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
                    return Ok(responseObject);
                }
                else
                {
                    foreach(var err in result.Errors)
                    {
                        responseObject.Message = "Password reset Failed";
                        responseObject.ErrorMessages.Add(err.Description);
                        return BadRequest(responseObject);
                    }
                }
            }
            responseObject.Message = $"user with email: {adduser.Email} was not found";
            return BadRequest(responseObject);

        }

        [HttpPost("logout")]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new ResponseObject<string> { StatusCode = 200, Message = "Logout is Successful" });
            
        }
    }
}
