using System.Net;
using Application_Tracker.Contracts;
using Application_Tracker.Contracts.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application_Tracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("applications")]
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllApplications([FromQuery] Guid userId)
        {
            try
            {
                var applications = await _applicationService.GetAllApplicationsAsync(userId);
                return Ok(applications);
            }
            catch (ArgumentException error)
            {
                return Problem(error.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicationById([FromRoute] Guid id)
        {
            try
            {
                var application = await _applicationService.GetApplicationByIdAsync(id);
                return Ok(application);
            }
            catch (Exception error)
            {
                return Problem(error.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateApplication(
            [FromBody] CreateApplicationDTO createApplicationDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdApplication = await _applicationService.CreateApplicationAsync(
                    createApplicationDto
                );

                return CreatedAtAction(
                    nameof(GetApplicationById),
                    new { id = createdApplication.Id },
                    createdApplication
                );
            }
            catch (ArgumentException error)
            {
                return Problem(error.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateApplication(
            [FromRoute] Guid id,
            [FromBody] UpdateApplicationDTO updateApplicationDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedApplication = await _applicationService.UpdateApplicationAsync(
                    id,
                    updateApplicationDto
                );

                return Ok(updatedApplication);
            }
            catch (ArgumentException error)
            {
                return Problem(error.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteApplication([FromRoute] Guid id)
        {
            try
            {
                await _applicationService.DeleteApplicationAsync(id);
                return NoContent();
            }
            catch (ArgumentException error)
            {
                return Problem(error.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
