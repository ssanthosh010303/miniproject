using Microsoft.AspNetCore.Mvc;

using WebApi.Exceptions;
using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/movieshow")]
[ApiController]
public class MovieShowController(IMovieShowService service) : ControllerBase
{
    private readonly IMovieShowService _service = service;

    [HttpPost]
    [ProducesResponseType(typeof(MovieShowAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(
        [FromBody] MovieShowAddUpdateDto dtoEntity)
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
    [ProducesResponseType(typeof(IEnumerable<MovieListDto>), StatusCodes.Status200OK)]
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
    [ProducesResponseType(typeof(MovieShowGetDto), StatusCodes.Status200OK)]
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

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MovieShowAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] MovieShowAddUpdateDto dtoEntity)
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