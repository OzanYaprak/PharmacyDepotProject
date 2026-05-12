using Application.Features.Orders.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Domain.Entities;
using Domain.Entities.Enums;
using Persistence.Repositories.Order;

namespace Application.Features.Orders.Rules;

public class OrderBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields

    private readonly IOrderRepository _orderRepository;
    public OrderBusinessRules(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    #endregion

    /// <summary>
    /// Sipariş tarihi gelecek bir tarih olamaz.
    /// </summary>
    public Task OrderDateCannotBeInTheFuture(DateTime orderDate)
    {
        if (orderDate > DateTime.UtcNow)
            throw new BusinessException(OrderMessages.OrderDateCannotBeInTheFuture);

        return Task.CompletedTask;
    }

    /// <summary>
    /// İptal edilmiş sipariş güncellenemez.
    /// </summary>
    public async Task CancelledOrderCannotBeUpdated(Guid id)
    {
        Order? order = await _orderRepository.GetAsync(predicate: o => o.Id == id);

        if (order?.Status == OrderStatus.Cancelled)
            throw new BusinessException(OrderMessages.CancelledOrderCannotBeUpdated);
    }

    /// <summary>
    /// Teslim edilmiş sipariş güncellenemez.
    /// </summary>
    public async Task DeliveredOrderCannotBeUpdated(Guid id)
    {
        Order? order = await _orderRepository.GetAsync(predicate: o => o.Id == id);

        if (order?.Status == OrderStatus.Delivered)
            throw new BusinessException(OrderMessages.DeliveredOrderCannotBeUpdated);
    }
}
