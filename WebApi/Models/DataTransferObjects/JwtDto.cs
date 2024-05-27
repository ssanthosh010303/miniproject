#nullable disable

namespace WebApi.Models.DataTransferObjects;

public class JwtGetDto : BaseDto
{
    public string Token { get; set; }
}
