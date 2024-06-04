/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
namespace WebApi.Models.DataTransferObjects;

public class BaseDto
{
}

public class BaseListDto : BaseDto
{
    public int Id { get; set; }
}

public class BaseAddUpdateDto : BaseDto
{
}

public class BaseGetDto : BaseDto
{
    public int Id { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
