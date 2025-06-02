using CW_10.Data;
using CW_10.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CW_10.Services;


public interface IDbService
{
    public Task<ICollection<TripGetDto>> GetTripsAsync();
}

public class DbService(MasterContext data) : IDbService
{
    public async Task<ICollection<TripGetDto>> GetTripsAsync()
    {
        return await data.Trips.OrderBy(t=> t.DateFrom).Select(t => new TripGetDto
        {
            IdTrip = t.IdTrip,
            Name = t.Name,
            Description = t.Description,
            DateFrom = t.DateFrom,
            DateTo = t.DateTo,
            MaxPeople = t.MaxPeople,
            Countries = t.IdCountries.Select(c => new CountryGetDto
            {
                IdCountry = c.IdCountry,
                Name = c.Name
            }).ToList()
        }).ToListAsync();
    }
}