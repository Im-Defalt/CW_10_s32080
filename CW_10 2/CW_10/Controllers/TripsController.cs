using CW_10.DTOs;
using CW_10.Exceptions;
using CW_10.Models;
using CW_10.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW_10.Controllers;


[ApiController]
[Route("[controller]")]
public class TripsController(IDbService service) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        return Ok(await service.GetTripsAsync());
    }

    [HttpPost("{IdTrip}/clients/{IdClient}")]
    public async Task<IActionResult> RegisterClientToTrip([FromRoute] int idTrip, [FromRoute] int idClient, [FromBody] RegisterClientToTripGetDto registerData)
    {
        try
        {
            await service.RegisterClientToTripAsync(idTrip, idClient, registerData);
            return NoContent();
        }
        catch (ExpiredException e)
        {
            return BadRequest(e.Message);
        }
        catch (ObjectHasAssociationsException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    
    
}