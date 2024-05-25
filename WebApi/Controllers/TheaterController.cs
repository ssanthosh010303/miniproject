using Microsoft.AspNetCore.Mvc;

using WebApi.Exceptions;
using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/theater")]
[ApiController]
public class TheaterController : ControllerBase
{
    private readonly ITheaterService _service;

    public TheaterController(ITheaterService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TheaterAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] TheaterAddUpdateDto dtoEntity)
    {
        try
        {
            return Ok(await _service.Add(dtoEntity));
        }
        catch (ServiceValidationException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "ValidationFailed"
            });
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "UpdateError"
            });
        }
    }

    [HttpPost("{id}/facility")]
    [ProducesResponseType(typeof(TheaterAddRemoveFacilitiesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFacility([FromRoute] int id, [FromBody] TheaterAddRemoveFacilitiesDto dtoEntity)
    {
        try
        {
            return Ok(await _service.AddFacility(id, dtoEntity));
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFoundOrExists"
            });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            await _service.Delete(id);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TheaterListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _service.GetAll());
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "DatabaseFetchFailed"
            });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TheaterGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            return Ok(await _service.GetById(id));
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }

    [HttpDelete("{id}/facility")]
    [ProducesResponseType(typeof(TheaterAddRemoveFacilitiesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFacility([FromRoute] int id, [FromBody] TheaterAddRemoveFacilitiesDto dtoEntity)
    {
        try
        {
            return Ok(await _service.RemoveFacility(id, dtoEntity));
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFoundOrExists"
            });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TheaterAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TheaterAddUpdateDto dtoEntity)
    {
        try
        {
            return Ok(await _service.Update(id, dtoEntity));
        }
        catch (ServiceValidationException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "ValidationFailed"
            });
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }
}
