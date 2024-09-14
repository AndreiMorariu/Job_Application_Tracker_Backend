using Application_Tracker.Contracts.DTO;

namespace Application_Tracker.Contracts
{
    public interface IUserService
    {
        Task<UserDTO> GetCurrentUserAsync(Guid userId);
        Task<bool> RegisterUserAsync(UserRegistrationDTO registrationDTO);
        Task<bool> ConfirmEmailAsync(Guid userId, string token);
        Task<bool> LogOutAsync();
        Task<bool> ResendConfirmationEmailAsync(string email);
    }
}
