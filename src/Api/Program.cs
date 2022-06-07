using Archie.Api.Services;
using Archie.Application.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddArchieModules<ApiUser>()
    .AddArchieSqlDb(builder.Configuration.GetConnectionString("ArchieDb"))
    .AddControllers()
        .PartManager
        .ApplicationParts
        .Add(new AssemblyPart(Assembly.GetAssembly(typeof(RegisterModules))!));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
