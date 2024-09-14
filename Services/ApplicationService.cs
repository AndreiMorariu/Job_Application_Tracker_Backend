using Application_Tracker.Contracts;
using Application_Tracker.Contracts.DTO;
using Application_Tracker.Data;
using Application_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Application_Tracker.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ApplicationDbContext _context;

        public ApplicationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationDTO>> GetAllApplicationsAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
                throw new ArgumentException("User not found");

            var applications = await _context
                .Applications.Where(ap => ap.UserId == userId)
                .Select(ap => new ApplicationDTO
                {
                    Id = ap.Id,
                    Company = ap.Company,
                    Position = ap.Position,
                    Location = ap.Location,
                    Link = ap.Link,
                    Date = ap.Date,
                    Status = ap.Status
                })
                .ToListAsync();

            return applications;
        }

        public async Task<ApplicationDTO> GetApplicationByIdAsync(Guid id)
        {
            var application = await _context
                .Applications.Where(ap => ap.Id == id)
                .Select(ap => new ApplicationDTO
                {
                    Id = ap.Id,
                    Company = ap.Company,
                    Position = ap.Position,
                    Location = ap.Location,
                    Link = ap.Link,
                    Date = ap.Date,
                    Status = ap.Status
                })
                .FirstOrDefaultAsync();

            if (application == null)
                throw new ArgumentException($"Application with id {id} not found");

            return application;
        }

        public async Task<ApplicationDTO> CreateApplicationAsync(
            CreateApplicationDTO createApplicationDTO
        )
        {
            var user = await _context.Users.FirstOrDefaultAsync(user =>
                user.Id == createApplicationDTO.UserId
            );

            if (user == null)
                throw new ArgumentException("User not found.");

            var application = new Application
            {
                Company = createApplicationDTO.Company,
                Position = createApplicationDTO.Position,
                Location = createApplicationDTO.Location,
                Link = createApplicationDTO.Link,
                Date = createApplicationDTO.Date,
                Status = createApplicationDTO.Status,
                UserId = createApplicationDTO.UserId,
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            var applicationDTO = new ApplicationDTO
            {
                Id = application.Id,
                Company = application.Company,
                Position = application.Position,
                Location = application.Location,
                Link = application.Link,
                Date = application.Date,
                Status = application.Status
            };

            return applicationDTO;
        }

        public async Task<bool> DeleteApplicationAsync(Guid id)
        {
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
                throw new ArgumentException($"Application with id {id} not found");

            _context.Applications.Remove(application);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ApplicationDTO> UpdateApplicationAsync(
            Guid id,
            UpdateApplicationDTO updateApplicationDTO
        )
        {
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
                throw new ArgumentException($"Application with id {id} not found");

            application.Company = updateApplicationDTO.Company;
            application.Position = updateApplicationDTO.Position;
            application.Location = updateApplicationDTO.Location;
            application.Link = updateApplicationDTO.Link;
            application.Date = updateApplicationDTO.Date;
            application.Status = updateApplicationDTO.Status;

            _context.Applications.Update(application);
            await _context.SaveChangesAsync();

            var applicationDTO = new ApplicationDTO
            {
                Id = application.Id,
                Company = application.Company,
                Position = application.Position,
                Location = application.Location,
                Link = application.Link,
                Date = application.Date,
                Status = application.Status
            };

            return applicationDTO;
        }
    }
}
