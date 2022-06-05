using Archie.Shared.Customers;
using Archie.Shared.Customers.Create;
using Archie.Shared.ValueObjects;
using Archie.WebUI.Services.Dialogs;
using Blazored.Modal;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Dialogs;

public partial class CreateCustomerDialog : IDialog
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [CascadingParameter] private BlazoredModalInstance ModalInstance { get; set; } = default!;

    private bool IsSubmitting { get; set; }
    private Form Model { get; } = new();

    protected override void OnInitialized()
    {
        ModalInstance.SetTitle("Create Customer");
    }

    public Task Close()
    {
        return ModalInstance.CloseAsync();
    }

    private async Task Submit()
    {
        if (IsSubmitting)
            return;

        IsSubmitting = true;

        var request = new CreateCustomerRequest(Model.CompanyName, new Location(Model.City, Model.Region, Model.Country));
        var response = await CustomerClient.CreateAsync(request);

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            Navigation.NavigateTo($"customers/{response.Content.Id}");
            await ModalInstance.CloseAsync();
        }

        IsSubmitting = false;
    }

    public class Form
    {
        public string CompanyName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public class Validator : AbstractValidator<Form>
        {
            public Validator()
            {
                RuleFor(x => x.CompanyName).IsValidCompanyName();
                RuleFor(x => x.City).IsValidCity();
                RuleFor(x => x.Region).IsValidRegion();
                RuleFor(x => x.Country).IsValidCountry();
            }
        }
    }
}
