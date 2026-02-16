using AS400PizzaEnterprise.Domain.Common;
using AS400PizzaEnterprise.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AS400PizzaEnterprise.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IAS400Connection _connection;
    private readonly IMediator _mediator;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly List<BaseEntity> _trackedEntities = new();

    public UnitOfWork(IAS400Connection connection, IMediator mediator, ILogger<UnitOfWork> logger)
    {
        _connection = connection;
        _mediator = mediator;
        _logger = logger;
    }

    public void TrackEntity(object entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            _trackedEntities.Add(baseEntity);
            _logger.LogDebug("Entity {EntityType} with ID {EntityId} tracked", 
                entity.GetType().Name, baseEntity.Id);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CommitTransactionAsync(cancellationToken);

            await DispatchDomainEventsAsync(cancellationToken);

            _logger.LogInformation("Changes saved successfully. {Count} entities tracked", _trackedEntities.Count);
            
            _trackedEntities.Clear();
            return _trackedEntities.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _connection.BeginTransactionAsync();
        _logger.LogDebug("Transaction started");
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _connection.CommitTransactionAsync();
        _logger.LogDebug("Transaction committed");
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _connection.RollbackTransactionAsync();
        _trackedEntities.Clear();
        _logger.LogDebug("Transaction rolled back");
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = _trackedEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        if (domainEvents.Any())
        {
            _logger.LogDebug("Dispatching {Count} domain events", domainEvents.Count);

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            foreach (var entity in _trackedEntities)
            {
                entity.ClearDomainEvents();
            }

            _logger.LogDebug("Domain events dispatched and cleared");
        }
    }
}
