namespace Archie.Shared.Features.WorkOrders.Create;

public class CreateWorkOrderResponse
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; }

    public CreateWorkOrderResponse(long id, string referenceNumber)
    {
        Id = id;
        ReferenceNumber = referenceNumber;
    }
}
