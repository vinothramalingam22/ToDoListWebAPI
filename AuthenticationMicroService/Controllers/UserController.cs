
using AuthenticationMicroService.Models;
using AuthenticationMicroService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MassTransit;
using DnsClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationMicroService.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private ITokenService _tokenService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserController> _logger;

        public UserController (IUserService userService, ITokenService tokenService, IPublishEndpoint publishEndpoint, ILogger<UserController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        // POST api/<UserController>
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public ActionResult Register([FromBody] Register model)
        {
            try
            {
                var isUserExist = _userService.IsUerExist(model.UserName);

                if (isUserExist)
                {
                    _logger.LogError("User already exist", model.UserName);
                    return StatusCode(StatusCodes.Status409Conflict, "User already exist");
                }

                _userService.Add(model);

                _logger.LogInformation("User Registered Successfully", model.UserName);

                return Ok(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex?.Message, model.UserName);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        // POST api/<UserController>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public ActionResult Login([FromBody] Login model)
        {
            try
            {
                Login login = new Login();
                login.UserName = model.UserName;
                var userInfo = AuthenticateUser(model);
                if (userInfo != null)
                {
                    _publishEndpoint.Publish(new UserMessageQueue() { UserId = model.UserName });

                    var tokenString =_tokenService.GenerateToken(userInfo);

                    return Ok(new AuthenticatedResponse { Token = tokenString, StatusCode = StatusCodes.Status201Created });
                }

                _logger.LogError("Invalid login credentials", model.UserName);

                return Ok(new AuthenticatedResponse { StatusCode = StatusCodes.Status401Unauthorized });
                
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex?.Message, model.UserName);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        private Login AuthenticateUser(Login login)
        {         
            var userDetails = _userService.GetLoginUser(login);

            if (userDetails != null)
            {
                return new Models.Login
                {
                    UserName = userDetails.UserName,
                    Password = userDetails.Password,
                    RoleName = userDetails.Role                    
                };
            }
            return new Models.Login();
        }
    }
}
