using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Application.Exceptions;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.WorkOrders;
using Archie.Shared.Features.WorkOrders.Create;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.WorkOrders;

[ApiController]
public class CreateWorkOrderFeature : IFeature
{
    private ICurrentUserService CurrentUser { get; }
    private IRepository Repository { get; }

    public CreateWorkOrderFeature(ICurrentUserService currentUser, IRepository repository)
    {
        CurrentUser = currentUser;
        Repository = repository;
    }

    [HttpPost(WorkOrderEndpoints.CreateWorkOrder)]
    public async Task<CreateWorkOrderResponse> Create(CreateWorkOrderRequest request, CancellationToken ct)
    {
        request.Validate();

        var customer = await Repository.FindByIdAsync<Customer, long>(request.CustomerId, ct)
            ?? throw new EntityNotFoundException("Customer not found.");

        var workOrder = new WorkOrder
        {
            CustomerId = request.CustomerId,
            DueDate = request.DueDate,
            ReferenceNumber = GenerateReferenceNumber(),
            Status = WorkOrderStatus.Draft
        };

        workOrder.AuditTrail = new List<Audit>
        {
            WorkOrderCreated(customer, workOrder.ReferenceNumber)
        };

        Repository.Add(workOrder);
        await Repository.SaveChangesAsync(ct);

        return new CreateWorkOrderResponse(workOrder.Id, workOrder.ReferenceNumber);
    }

    [NonAction]
    public Audit WorkOrderCreated(Customer customer, string referenceNumber)
    {
        return new Audit
        {
            AuditType = AuditType.Create,
            EventType = EventType.WorkOrderCreated,
            Description = $"Work order `{referenceNumber}` was created for {customer.CompanyName}.",
            UserId = CurrentUser.Id,
            Customers = new List<Customer> { customer }
        };
    }

    private static string GenerateReferenceNumber()
    {
        var today = DateTime.UtcNow;
        return $"{today.Year}{today.Month}{today.Day}-{today.Millisecond:0000}";
    }

}
