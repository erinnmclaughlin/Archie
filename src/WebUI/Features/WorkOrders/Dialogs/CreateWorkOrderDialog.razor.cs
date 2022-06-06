using Archie.Shared.Customers.GetAll;
using Archie.Shared.WorkOrders.Create;
using Archie.WebUI.Clients;
using Archie.WebUI.Services.Dialogs;
using Blazored.Modal;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Features.WorkOrders.Dialogs;

public partial class CreateWorkOrderDialog : IDialog
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IWorkOrderClient WorkOrderClient { get; set; } = default!;
    [CascadingParameter] private BlazoredModalInstance ModalInstance { get; set; } = default!;

    private GetAllCustomersResponse? Customers { get; set; }
    private bool IsSubmitting { get; set; }
    private Form Model { get; } = new();

    protected override void OnInitialized()
    {
        ModalInstance.SetTitle("Create Work Order");
    }

    public Task Close()
    {
        return ModalInstance.CloseAsync();
    }

    private async Task EnsureCustomersAreLoaded()
    {
        if (Customers != null)
            return;

        var response = await CustomerClient.GetAllAsync();
        Customers = response.Content;
    }

    private async Task Submit()
    {
        if (IsSubmitting)
            return;

        IsSubmitting = true;

        var request = new CreateWorkOrderRequest(Model.CustomerId!.Value, Model.DueDate);
        var response = await WorkOrderClient.CreateAsync(request);

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            Navigation.NavigateTo($"work-orders/{response.Content.Id}");
            await ModalInstance.CloseAsync();
        }

        IsSubmitting = false;
    }

    public class Form
    {
        public long? CustomerId { get; set; }
        public DateTime? DueDate { get; set; }

        public class Validator : AbstractValidator<Form>
        {
            public Validator()
            {
                RuleFor(x => x.CustomerId)
                    .NotNull().WithMessage("You must select a customer.");
            }
        }
    }
}
