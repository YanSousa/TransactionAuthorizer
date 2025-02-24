using Microsoft.AspNetCore.Mvc;
using TransactionAuthorizer.Dtos;
using TransactionAuthorizer.Services;

namespace TransactionAuthorizer.Controllers
{
    [ApiController]
    [Route("usersManager")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepositoryService _userRepository;

        public UsersController(IUserRepositoryService userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            return Ok(_userRepository.GetAllUsers());
        }

        [HttpGet("user/{account}")]
        public IActionResult GetUser(string account)
        {
            var user = _userRepository.GetUser(account);
            if (user == null)
                return NotFound(TransactionResult.UserNotFound());

            return Ok(user);
        }
    }
}
