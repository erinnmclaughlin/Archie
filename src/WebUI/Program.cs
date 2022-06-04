using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Archie.WebUI;
using Refit;
using Archie.WebUI.Customers;
using Blazored.Modal;
using Archie.WebUI.Customers.Dialogs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredModal();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRefitClient<ICustomerClient>();

builder.Services.AddTransient<CreateCustomerDialog>();

await builder.Build().RunAsync();
