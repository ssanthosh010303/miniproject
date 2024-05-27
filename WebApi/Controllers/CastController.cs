using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApi.Exceptions;
using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/cast")]
[ApiController]
public class CastController(
    ICastService service, ILogger<CastService> logger) : ControllerBase
{
    private readonly ICastService _service = service;
    private readonly ILogger<CastService> _logger = logger;

    /// <summary>
    /// Adds a new cast member.
    /// </summary>
    /// <param name="dtoEntity">The data transfer object for the cast member.</param>
    /// <returns>The added cast member.</returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(CastAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CastAddUpdateDto dtoEntity)
    {
        try
        {
            _logger.LogInformation("Adding a new cast member.");
            return Ok(await _service.Add(dtoEntity));
        }
        catch (ServiceValidationException ex)
        {
            _logger.LogError(ex, "Validation failed while adding a new cast member.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "ValidationFailed"
            });
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to add a new cast member.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "UpdateError"
            });
        }
    }

    /// <summary>
    /// Deletes a cast member by ID.
    /// </summary>
    /// <param name="id">The ID of the cast member to delete.</param>
    /// <returns>No content if the cast member is successfully deleted.</returns>
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            _logger.LogInformation("Deleting a cast member.");
            await _service.Delete(id);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to delete a cast member.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }

    /// <summary>
    /// Gets all cast members.
    /// </summary>
    /// <returns>A list of all cast members.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CastListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInformation("Fetching all cast members.");
            return Ok(await _service.GetAll());
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to fetch all cast members.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "DatabaseFetchFailed"
            });
        }
    }

    /// <summary>
    /// Gets a cast member by ID.
    /// </summary>
    /// <param name="id">The ID of the cast member to fetch.</param>
    /// <returns>The cast member with the specified ID.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CastGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            _logger.LogInformation("Fetching a cast member by ID.");
            return Ok(await _service.GetById(id));
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to fetch a cast member by ID.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }

    /// <summary>
    /// Updates a cast member by ID.
    /// </summary>
    /// <param name="id">The ID of the cast member to update.</param>
    /// <param name="dtoEntity">The updated data for the cast member.</param>
    /// <returns>The updated cast member.</returns>
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(CastAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] CastAddUpdateDto dtoEntity)
    {
        try
        {
            _logger.LogInformation("Updating a cast member.");
            return Ok(await _service.Update(id, dtoEntity));
        }
        catch (ServiceValidationException ex)
        {
            _logger.LogError(ex, "Validation failed while updating a cast member.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "ValidationFailed"
            });
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to update a cast member.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }
}
