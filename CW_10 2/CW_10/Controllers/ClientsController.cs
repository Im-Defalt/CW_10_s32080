using CW_10.Exceptions;
using CW_10.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_10.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController(IDbService service) :ControllerBase
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient([FromRoute] int id)
    {
        try
        {
            await service.RemoveClientAsync(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ObjectHasAssociationsException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}