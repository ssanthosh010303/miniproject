#nullable disable

namespace WebApi.Models.DataTransferObjects;

public class ErrorResponseDto
{
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}
