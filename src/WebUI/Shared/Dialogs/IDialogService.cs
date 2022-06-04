using Blazored.Modal;

namespace Archie.WebUI.Shared.Dialogs;

public interface IDialogService
{
    IModalReference Show<TDialog>() where TDialog : IDialog;
    IModalReference Show<TDialog, TParameters>(TParameters parameters) where TDialog : IDialog;
}
