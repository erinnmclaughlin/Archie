namespace Archie.WebUI.Clients;

// Use to auto-register refit clients in DI
[AttributeUsage(AttributeTargets.Interface)]
public class RefitClientAttribute : Attribute
{
}
