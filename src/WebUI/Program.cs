using Archie.WebUI;
using Archie.WebUI.Clients;
using Archie.WebUI.Services.Dialogs;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredModal();
builder.Services.AddScoped<IDialogService, DialogService>();

// Auto-register refit clients
Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(RefitClientAttribute)))
    .ToList()
    .ForEach(client =>
    {
        builder.Services.AddRefitClient(client)
            .ConfigureHttpClient(x => x.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
    });

await builder.Build().RunAsync();
