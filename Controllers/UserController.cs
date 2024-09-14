using System.Net;
using System.Security.Claims;
using Application_Tracker.Contracts;
using Application_Tracker.Contracts.DTO;
using Application_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Application_Tracker.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService, SignInManager<User> signInManager)
        {
            _userService = userService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null || !currentUser.Identity.IsAuthenticated)
                return Problem(
                    "User is not authenticated",
                    statusCode: (int)HttpStatusCode.Unauthorized
                );

            var nameIdentifierClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);

            if (nameIdentifierClaim == null)
                return Problem(
                    "NameIdentifier claim not found",
                    statusCode: (int)HttpStatusCode.BadRequest
                );

            if (!Guid.TryParse(nameIdentifierClaim.Value, out var currentUserId))
                return Problem(
                    "Invalid user ID format",
                    statusCode: (int)HttpStatusCode.BadRequest
                );

            try
            {
                var userDto = await _userService.GetCurrentUserAsync(currentUserId);
                return Ok(userDto);
            }
            catch (Exception)
            {
                return Problem("User not found", statusCode: (int)HttpStatusCode.BadRequest);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogOutAsync();
            return Ok("User logged out successfully.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registerDto)
        {
            if (!ModelState.IsValid)
                return Problem(
                    "Password must be at least 6 characters",
                    statusCode: (int)HttpStatusCode.BadRequest
                );

            try
            {
                var isRegistered = await _userService.RegisterUserAsync(registerDto);
                return Ok();
            }
            catch (Exception e)
            {
                IActionResult result = e.Message switch
                {
                    "Email could not be sent"
                        => Problem(
                            "Email could not be sent",
                            statusCode: (int)HttpStatusCode.InternalServerError
                        ),
                    "User creation failed"
                        => Problem(
                            "User could not be created",
                            statusCode: (int)HttpStatusCode.InternalServerError
                        ),
                    "Email already in use"
                        => Problem(
                            "Email already in use",
                            statusCode: (int)HttpStatusCode.BadRequest
                        ),
                    _
                        => Problem(
                            "Server Error",
                            statusCode: (int)HttpStatusCode.InternalServerError
                        ),
                };

                return result;
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] Guid userId,
            [FromQuery] string token
        )
        {
            if (userId == Guid.Empty || string.IsNullOrEmpty(token))
                return Problem(
                    "UserId or token is missing",
                    statusCode: (int)HttpStatusCode.BadRequest
                );

            try
            {
                var isConfirmed = await _userService.ConfirmEmailAsync(userId, token);
                return File("~/email.html", "text/html");
            }
            catch (Exception e)
            {
                IActionResult result = e.Message switch
                {
                    "User not found"
                        => Problem("User not found", statusCode: (int)HttpStatusCode.BadRequest),
                    "Could not confirm email"
                        => Problem(
                            "\"Could not confirm email",
                            statusCode: (int)HttpStatusCode.InternalServerError
                        ),

                    _
                        => Problem(
                            "Server Error",
                            statusCode: (int)HttpStatusCode.InternalServerError
                        ),
                };

                return result;
            }
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return Problem("Email is required", statusCode: (int)HttpStatusCode.BadRequest);

            try
            {
                var result = await _userService.ResendConfirmationEmailAsync(email);
                return Ok("Confirmation email resent successfully.");
            }
            catch (Exception e)
            {
                IActionResult result = e.Message switch
                {
                    "User not found"
                        => Problem("User not found", statusCode: (int)HttpStatusCode.BadRequest),
                    "Email already confirmed"
                        => Problem(
                            "Email already confirmed",
                            statusCode: (int)HttpStatusCode.BadRequest
                        ),
                    "Email could not be sent"
                        => Problem(
                            "Email could not be sent",
                            statusCode: (int)HttpStatusCode.InternalServerError
                        ),
                    _
                        => Problem(
                            "Server Error",
                            statusCode: (int)HttpStatusCode.InternalServerError
                        ),
                };

                return result;
            }
        }
    }
}
