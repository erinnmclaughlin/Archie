using Blazored.Modal;

namespace Archie.WebUI.Services.Dialogs;

public interface IDialogService
{
    IModalReference Show<TDialog>() where TDialog : IDialog;
    IModalReference Show<TDialog, TParameters>(TParameters parameters) where TDialog : IDialog<TParameters>;
}
