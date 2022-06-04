using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Shared.Dialogs;

public interface IDialog : IComponent
{
}

public interface IDialog<TParameters> : IComponent
{
    TParameters Parameters { get; set; }
}
