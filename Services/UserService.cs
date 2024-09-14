using Application_Tracker.Contracts;
using Application_Tracker.Contracts.DTO;
using Application_Tracker.Models;
using Microsoft.AspNetCore.Identity;

namespace Application_Tracker.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDTO> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new ArgumentException("User not found");

            return new UserDTO { Id = user.Id, Email = user.Email, };
        }

        public async Task<bool> RegisterUserAsync(UserRegistrationDTO registrationDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(registrationDTO.Email);

            if (existingUser != null)
                throw new ArgumentException("Email already in use");

            var user = new User { UserName = registrationDTO.Email, Email = registrationDTO.Email };

            var result = await _userManager.CreateAsync(user, registrationDTO.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var request = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                var confirmationLink =
                    $"{baseUrl}/users/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

                var emailBody =
                    $@"
                <html>
                    <body>
                        <p>Hello,</p>
                        <p>Please confirm your email by clicking the button below:</p>
                        <a href='{confirmationLink}' style='background-color: #3b82f6; color: #fff; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; font-weight:700; border-radius: 5px;'>
                            Confirm Email
                        </a>
                        <p>If you did not request this, please ignore this email.</p>
                    </body>
                </html>";

                try
                {
                    await _emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);
                    return true;
                }
                catch (Exception)
                {
                    throw new ApplicationException("Email could not be sent");
                }
            }

            throw new ApplicationException("User creation failed");
        }

        public async Task<bool> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new ArgumentException("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                throw new ApplicationException("Could not confirm email");

            return true;
        }

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new ArgumentException("User not found");

            if (await _userManager.IsEmailConfirmedAsync(user))
                throw new ArgumentException("Email already confirmed");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var confirmationLink =
                $"{baseUrl}/users/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var emailBody =
                $@"
            <html>
                <body>
                    <p>Hello,</p>
                    <p>Please confirm your email by clicking the button below:</p>
                    <a href='{confirmationLink}' style='background-color: #3b82f6; color: #fff; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; font-weight:700; border-radius: 5px;'>
                        Confirm Email
                    </a>
                    <p>If you did not request this, please ignore this email.</p>
                </body>
            </html>";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);
                return true;
            }
            catch (Exception)
            {
                throw new ApplicationException("Email could not be sent");
            }
        }
    }
}
