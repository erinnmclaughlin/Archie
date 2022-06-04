using Archie.Shared.Customers;
using Archie.Shared.Customers.Create;
using Archie.Shared.ValueObjects;
using Blazored.Modal;
using Blazored.Modal.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Dialogs;

public class CreateCustomerDialogLauncher : ComponentBase
{
    [Inject] private IModalService Modals { get; set; } = default!;

    public IModalReference Show()
    {
        var options = new ModalOptions
        {
            Animation = ModalAnimation.FadeIn(0.5),
            DisableBackgroundCancel = true
        };

        return Modals.Show<CreateCustomerDialog>("Create Customer", options);
    }
}

public partial class CreateCustomerDialog
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;
    [CascadingParameter] BlazoredModalInstance ModalInstance { get; set; } = default!;

    private bool IsSubmitting { get; set; }
    private Form Model { get; } = new();

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

        if (response.IsSuccessStatusCode)
            await ModalInstance.CloseAsync(ModalResult.Ok(response.Content!));
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
