/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
#nullable disable

namespace WebApi.Models.DataTransferObjects;

public class JwtGetDto : BaseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class JwtRefreshDto : BaseDto
{
    public string AccessToken { get; set; }
}
