using Application_Tracker.Contracts.DTO;

namespace Application_Tracker.Contracts
{
    public interface IApplicationService
    {
        Task<List<ApplicationDTO>> GetAllApplicationsAsync(Guid userId);
        Task<ApplicationDTO> GetApplicationByIdAsync(Guid id);
        Task<ApplicationDTO> CreateApplicationAsync(CreateApplicationDTO application);
        Task<ApplicationDTO> UpdateApplicationAsync(
            Guid id,
            UpdateApplicationDTO updatedApplication
        );
        Task<bool> DeleteApplicationAsync(Guid id);
    }
}
