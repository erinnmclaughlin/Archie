﻿namespace Archie.Shared.Audits;
public enum EventType
{
    // Customers
    CustomerCreated,
    CustomerNameUpdated,
    CustomerLocationUpdated,

    // Work Orders
    WorkOrderCreated,
    WorkOrderPublished,
    WorkOrderApproved,
    WorkOrderRejected,
    WorkOrderCanceled,
    WorkOrderCompleted
}
