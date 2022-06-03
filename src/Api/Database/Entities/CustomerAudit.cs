namespace Archie.Api.Database.Entities;

public class CustomerAudit : Audit
{
    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public class Configuration : Configuration<CustomerAudit>
    {
    }
}
