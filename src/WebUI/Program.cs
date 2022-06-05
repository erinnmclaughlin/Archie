using Archie.WebUI;
using Archie.WebUI.AuditTrails;
using Archie.WebUI.Customers;
using Archie.WebUI.Services.Dialogs;
using Archie.WebUI.WorkOrders;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredModal();
builder.Services.AddScoped<IDialogService, DialogService>();

builder.Services.AddRefitClient<IAuditTrailClient>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddRefitClient<ICustomerClient>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddRefitClient<IWorkOrderClient>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

await builder.Build().RunAsync();
