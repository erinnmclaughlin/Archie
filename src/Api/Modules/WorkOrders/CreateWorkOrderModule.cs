﻿using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Api.Exceptions;
using Archie.Shared.Audits;
using Archie.Shared.WorkOrders;
using Archie.Shared.WorkOrders.Create;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.WorkOrders;

[ApiController]
public class CreateWorkOrderModule : IModule
{
    private AuditFactory Audits { get; }
    private IRepository Repository { get; }

    public CreateWorkOrderModule(AuditFactory audits, IRepository repository)
    {
        Audits = audits;
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
            Audits.WorkOrderCreated(customer.CompanyName, workOrder.ReferenceNumber)
        };

        Repository.Add(workOrder);
        await Repository.SaveChangesAsync(ct);

        return new CreateWorkOrderResponse(workOrder.Id, workOrder.ReferenceNumber);
    }

    private static string GenerateReferenceNumber()
    {
        var today = DateTime.UtcNow;
        return $"{today.Year}{today.Month}{today.Day}-{today.Millisecond:0000}";
    }

    public class AuditFactory
    { 
        private ICurrentUserService CurrentUser { get; }

        public AuditFactory(ICurrentUserService currentUser)
        {
            CurrentUser = currentUser;
        }

        public virtual Audit WorkOrderCreated(string companyName, string referenceNumber)
        {
            return new Audit
            { 
                AuditType = AuditType.Create,
                EventType = EventType.WorkOrderCreated,
                Description = $"Work order `{referenceNumber}` was created for {companyName}.",
                UserId = CurrentUser.Id
            };
        }
    }
}