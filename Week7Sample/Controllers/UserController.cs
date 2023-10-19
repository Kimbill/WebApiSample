using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Week7Sample.Data.Repositories.Interfaces;
using Week7Sample.Model;
using Week7Sample.Model.Enums;

namespace Week7Sample.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, UserManager<User> userManager, IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost("add")]
        //[Authorize]
        [AllowAnonymous]
        //[Authorize(Roles = UserRole.Admin)]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto adduser)
        {
            try
            {
                //perform validations on model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(adduser);
                }

                //checked if user type is valid

                var userExists = await _userManager.FindByEmailAsync(adduser.Email);
                
                
                //var user = new User
                //{
                //    FirstName = adduser.FirstName,
                //    LastName = adduser.LastName,
                //    UserName = adduser.Email,
                //    UserRole = UserRole.Admin,
                //};

                //var mappedUser = _mapper.Map<User>(adduser);
                var mappedUser = _mapper.Map<User>(adduser);
                //mappedUser.UserName = adduser.Email;


                //var result = _userRepository.AddNew(user);

                var result = await _userManager.CreateAsync(mappedUser, adduser.Password);

                if (result.Succeeded)
                {
                    //return CreatedAtAction(nameof(GetUser), new { id = mappedUser.Id }, mappedUser);
                    var userFromDb = _mapper.Map<UserReturnDto>(mappedUser);

                    var responseObject = new ResponseObject<UserReturnDto>
                    {
                        StatusCode = 200,
                        Message = $"User added",
                        Data = userFromDb
                    };

                    return Ok(responseObject);
                }

                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                
                    return BadRequest(result);

                return Ok(result);
            }
            catch(Exception ex)
            {
                //handle error thrown
                return BadRequest();
            }
        }

        [HttpPost("confirm-email")]
        public IActionResult ConfirmEmail([FromBody] LoginDto adduser)
        {
            return Ok();
        }

        [HttpGet("single/{id}")]
        [AllowAnonymous]
        public ActionResult GetUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id provided should not be empty");
            }

            var user = _userRepository.GetById(id);
            if(user == null)
            {
                return NotFound("No record found");
            }

            var userFromDb = _mapper.Map<UserReturnDto>(user);

            var responseObject = new ResponseObject<UserReturnDto>
            {
                StatusCode = 200,
                Message = $"User with id: {id} is found",
                Data = userFromDb
            };
            return Ok(responseObject);
        }

        [HttpGet("list")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<User>> GetAllusers(int page, int perpage)
        {
            try
            {
                //    if (page < 1)
                //    {
                //        page = 1;
                //    }

                //    if (pageSize < 1)
                //    {
                //        pageSize = 10;
                //    }

                //    var users = _userRepository.GetUsersBypagination(page, pageSize);

                //    return Ok(users);
                //}
                //catch (Exception ex)
                //{
                //    return BadRequest();

                var result = _userRepository.GetAll();
                if (result !=null && result.Count() > 0)
                {
                    var paged = _userRepository.GetUsersBypagination(result.ToList(), perpage, page);

                    return Ok(paged);
                }

                return Ok(result);

            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPut("update/{id}")]
        public ActionResult UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                // Perform validations on model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(errors);
                }

                var user = _userRepository.GetById(id);

                if (user == null)
                {
                    return NotFound("No record found");
                }

                // Update user properties based on updateUserDto
                user.FirstName = updateUserDto.FirstName;

                // You can apply other updates here if needed

                var result = _userRepository.Update(user);
                if (result != null)
                    return Ok(result);

                return BadRequest("Failed to update");
            }
            catch (Exception ex)
            {
                // Handle error thrown
                return BadRequest();
            }
        }
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return BadRequest("Id provided should be empty");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(adduser);
        //    }

        //    if(id != adduser.Id)
        //    {
        //        return BadRequest("Id mismatch");
        //    }

        //    var user = _userRepository.GetById(id);

        //    if (user == null)
        //    {
        //        return NotFound("No record found");
        //    }

        //    //map model to user record
        //    user.FirstName = adduser.FirstName;

        //    var result = _userRepository.Update(user);
        //    if(result !=null)
        //        return Ok(result);

        //    return BadRequest("Failed to update");
        //}

        [HttpPatch("partialupdate/{id}")]
        public ActionResult PartialUpdateUser(string id, [FromBody] PartialUpdateUserDto partialUpdateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(errors);
                }

                var existingUser = _userRepository.GetById(id);

                if (existingUser == null)
                {
                    return NotFound("No record found");
                }

                if (!string.IsNullOrWhiteSpace(partialUpdateUserDto.LastName))
                {
                    existingUser.LastName = partialUpdateUserDto.LastName;
                }

                var result = _userRepository.Update(existingUser);
                if (result != null)
                    return Ok(result);

                return BadRequest("Failed to partially update LastName");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id should not be empty");
            }

            var user = _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound("No record found");
            }

            var result = _userRepository.Delete(user);
            if (result)
                return Ok($"User with id {user.Id} has been deleted");

            return BadRequest("failed to delete user record");

        }

        [HttpDelete("delete/list")]
        public ActionResult DeleteMultipleUsers([FromBody] DeleteMultipleUsersDto adduser)
        {
            if (adduser.Id.Count() < 1)
            {
                return BadRequest("Id should not be empty");
            }

            var list = new List<User>();
            foreach(var id in adduser.Id)
            {
                var users = _userRepository.GetById(id);
                if(users != null)
                    list.Add(users);
            }

            if (list.Count > 0)
            {

                var result = _userRepository.Delete(list);
                if (result)
                    return Ok("List of users deleted");
                
            }

            return BadRequest("Failed to delete users");
  
        }
    }
}
