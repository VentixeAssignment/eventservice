using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WebApi.Models;
using WebApi.Protos;
using WebApi.Repositories;

namespace WebApi.Services;

public class EventServiceGrpc(EventRepository repository, ILogger<EventInformationReply> logger, ILogger<SeatsReply> loggerSeats) : BookingHandler.BookingHandlerBase
{
    private readonly EventRepository _repository = repository;
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
        if(request == null)
        {
            _loggerSeats.LogWarning("SeatsRequest was called with null Request.");
            return new SeatsReply
            {
                Success = false,
                Message = "Request can not be null."
            };
        }

        try
        {

            var entity = await _repository.GetOneAsync(x => x.Id == request.Id);
            if (!entity.Success || entity.Data == null)
            {
                _loggerSeats.LogWarning($"Failed to get entity with id {request.Id}");
                return new SeatsReply
                {
                    Success = false,
                    Message = $"No entity found with id {request.Id}"
                };
            }
            
            if (request.SeatsOrdered > entity.Data.SeatsLeft)
            {
                _loggerSeats.LogWarning($"Seats ordered exceeds available seats for entity with id {request.Id}");
                return new SeatsReply
                {
                    Success = false,
                    Message = $"Seats ordered exceeds available seats for entity with id {request.Id}"
                };
            }

            await _repository.BeginTransactionAsync();

            var updated = _repository.UpdateSeatsLeft(entity.Data, request.SeatsOrdered);
            if (!updated.Success)
            {
                await _repository.RollbackTransactionAsync();
                _loggerSeats.LogWarning($"Failed to update seats left for entity with id {request.Id}.");
                return new SeatsReply
                {
                    Success = false,
                    Message = $"Failed to update seats left for entity with id {request.Id}."
                };
            }

            await _repository.SaveChangesAsync();
            await _repository.CommitTransactionAsync();

            return new SeatsReply
            {
                Success = true,
                Message = $"Seats successfully updated for entity with id {request.Id}"
            };
        }
        catch (Exception ex)
        {
            await _repository.RollbackTransactionAsync();
            _loggerSeats.LogError($"Failed to update seats left for entity with id {request.Id}\n{ex}\n{ex.Message}");
            return new SeatsReply
            {
                Success = false,
                Message = $"Something failed updating seats on entity with id {request.Id}"
            };
        }
    }
}
