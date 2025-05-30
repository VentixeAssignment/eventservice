using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Transactions;
using WebApi.Data;
using WebApi.Models;
using WebApi.Protos;
using WebApi.Repositories;

namespace WebApi.Services;

public class EventServiceGrpc(EventRepository repository, ILogger<EventInformationReply> logger, ILogger<SeatsReply> loggerSeats, DataContext context) : BookingHandler.BookingHandlerBase
{
    private readonly EventRepository _repository = repository;
    private readonly DataContext _context = context;

    private readonly ILogger<EventInformationReply> _logger = logger;
    private readonly ILogger<SeatsReply> _loggerSeats = loggerSeats;

    public override async Task<EventInformationReply> GetEventInformation(EventInformationRequest request, ServerCallContext context)
    {
        if (request == null)
        {
            _logger.LogWarning("GetEventInformation was called with null Request.");
            return new EventInformationReply
            {
                Success = false,
                Message = "Request can not be null."
            };
        }

        var reply = await _repository.GetOneAsync(x => x.Id == request.Id);

        if (!reply.Success || reply.Data == null)
        {
            _logger.LogWarning($"Failed to get event information for {request.Id}");
            return new EventInformationReply
            {
                Success = false,
                Message = "Failed to get event information."
            };
        }


        return new EventInformationReply
        {
            Success = true,
            Event = new Event
            {
                Id = reply.Data.Id,
                EventName = reply.Data.EventName,
                Start = Timestamp.FromDateTime(reply.Data.Start.ToUniversalTime()),
                End = Timestamp.FromDateTime(reply.Data.End.ToUniversalTime()),
                SeatsLeft = reply.Data.SeatsLeft ?? 0,
                PricePerSeat = (double)reply.Data.PricePerSeat,
                Venue = reply.Data.Venue ?? "",
                StreetAddress = reply.Data.StreetAddress,
                PostalCode = reply.Data.PostalCode,
                City = reply.Data.City,
                Country = reply.Data.Country
            }
        };
    }


    public override async Task<SeatsReply> UpdateSeatsLeft(SeatsRequest request, ServerCallContext context)
    {
        if (request == null)
        {
            return new SeatsReply
            {
                Success = false,
                Message = "Request can not be null."
            };
        }

        var entity = await _repository.GetOneAsync(x => x.Id == request.Id);
        if (!entity.Success || entity.Data == null)
        {
            return new SeatsReply
            {
                Success = false,
                Message = $"No entity found with id {request.Id}. ##### {entity.ErrorMessage}"
            };
        }

        if (request.SeatsOrdered > entity.Data.SeatsLeft)
        {
            return new SeatsReply
            {
                Success = false,
                Message = $"Seats ordered exceeds available seats for entity with id {request.Id}"
            };
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var updated = _repository.UpdateSeatsLeft(entity.Data, request.SeatsOrdered);
            if (!updated.Success)
            {
                await transaction.RollbackAsync();
                return new SeatsReply
                {
                    Success = false,
                    Message = $"Failed to update seats left for entity with id {request.Id}. ##### {updated.ErrorMessage}"
                };
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new SeatsReply
            {
                Success = true,
                Message = $"Seats successfully updated for entity with id {request.Id}"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _loggerSeats.LogError($"Failed to update seats left for entity with id {request.Id}. ##### {ex}");
            return new SeatsReply
            {
                Success = false,
                Message = $"Something failed updating seats on entity with id {request.Id}. ##### {ex}"
            };
        }
    }
}
