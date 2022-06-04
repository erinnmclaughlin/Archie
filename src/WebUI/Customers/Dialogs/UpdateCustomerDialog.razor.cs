using Archie.Shared.Customers;
using Archie.Shared.Customers.Update;
using Archie.Shared.ValueObjects;
using Archie.WebUI.Shared.Dialogs;
using Blazored.Modal;
using Blazored.Modal.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Dialogs;

public partial class UpdateCustomerDialog : IDialog<UpdateCustomerDialog.DialogParameters>
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;
    [CascadingParameter] BlazoredModalInstance ModalInstance { get; set; } = default!;
    [Parameter] public DialogParameters Parameters { get; set; } = default!;

    private bool IsSubmitting { get; set; }
    private Form Model { get; } = new();

    protected override void OnInitialized()
    {
        ModalInstance.SetTitle("Update Customer");

        Model.CompanyName = Parameters.DefaultForm.CompanyName;
        Model.City = Parameters.DefaultForm.City;
        Model.Country = Parameters.DefaultForm.Country;
        Model.Region = Parameters.DefaultForm.Region;
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

        var request = new UpdateCustomerRequest(Model.CompanyName, new Location(Model.City, Model.Region, Model.Country));
        var response = await CustomerClient.UpdateAsync(Parameters.CustomerId, request);

        if (response.IsSuccessStatusCode)
            await ModalInstance.CloseAsync(ModalResult.Ok(response.Content!));
    }

    public class DialogParameters
    {
        public long CustomerId { get; }
        public Form DefaultForm { get; }

        public DialogParameters(long customerId, string companyName, Location location)
        {
            CustomerId = customerId;
            DefaultForm = new Form(companyName, location);
        }
    }

    public class Form
    {
        public string CompanyName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public Form()
        {
        }

        public Form(string companyName, Location location)
        {
            CompanyName = companyName;
            City = location.City ?? string.Empty;
            Region = location.Region ?? string.Empty;
            Country = location.Country ?? string.Empty;
        }

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
