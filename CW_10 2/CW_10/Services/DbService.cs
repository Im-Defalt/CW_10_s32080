using CW_10.Data;
using CW_10.DTOs;
using CW_10.Exceptions;
using CW_10.Models;
using Microsoft.EntityFrameworkCore;

namespace CW_10.Services;


public interface IDbService
{
    public Task<ICollection<TripGetDto>> GetTripsAsync();
    public Task RemoveClientAsync(int clientId);
    public Task RegisterClientToTripAsync(int idTrip, int idClient, RegisterClientToTripGetDto registerData);
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


    public async Task RemoveClientAsync(int clientId)
    {
        var client = await data.Clients.FirstOrDefaultAsync(c => c.IdClient == clientId);

        if (client == null)
        {
            throw new NotFoundException($"Client with id {clientId} not found");
        }

        if (data.ClientTrips.Any(ct => ct.IdClient == clientId))
        {
            throw new ObjectHasAssociationsException($"Client with id {clientId} has a booked trip");
        }
        
        data.Clients.Remove(client);
        await data.SaveChangesAsync();
    }


    public async Task RegisterClientToTripAsync(int idTrip, int idClient, RegisterClientToTripGetDto registerData)
    {
        var client = await data.Clients.FirstOrDefaultAsync(c => c.IdClient == idClient);
        if (client == null)
        {
            throw new NotFoundException($"Client with id {idClient} not found");
        }
        
        
        var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        if (trip == null)
        {
            throw new NotFoundException($"Trip with id {idTrip} not found");
        }
        if(trip.DateFrom <= DateTime.Now)
        {
            throw new ExpiredException($"Trip with id {idTrip} has already departed");
        }
        
        
        var clientTrip = await data.ClientTrips.FirstOrDefaultAsync(t => t.IdTrip == idTrip && t.IdClient == idClient);
        if (clientTrip != null)
        {
            throw new ObjectHasAssociationsException($"Client with id {idClient} has already booked trip with id {idTrip}");
        }

        
        var newClientTrip = new ClientTrip
        {
            IdClient = idClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = registerData.PaymentDate,
        };
        await data.ClientTrips.AddAsync(newClientTrip);
        await data.SaveChangesAsync();




    }
    
}