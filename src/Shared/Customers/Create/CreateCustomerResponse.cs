namespace Archie.Shared.Customers.Create;

public class CreateCustomerResponse
{
    public long Id { get; set; }

    public CreateCustomerResponse(long id)
    {
        Id = id;
    }
}
