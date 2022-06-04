namespace Archie.Shared.Customers;

public static class CustomerEndpoints
{
    public const string CreateCustomer = "/api/customers";
    public const string GetAllCustomers = "/api/customers";

    public const string GetCustomerDetails = "/api/customers/{id}";
    public const string UpdateCustomer = "/api/customers/{id}";

    public const string GetAuditTrail = "/api/customers/{id}/audit-trail";
}
