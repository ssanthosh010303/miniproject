/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

namespace WebApi.Models.DataTransferObjects;

public class ErrorResponseDto
{
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}
