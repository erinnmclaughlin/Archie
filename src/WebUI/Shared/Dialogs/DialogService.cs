using Blazored.Modal;
using Blazored.Modal.Services;

namespace Archie.WebUI.Shared.Dialogs;

public class DialogService : IDialogService
{
    private IModalService Modals { get; }

    private static ModalOptions DefaultOptions
    {
        get
        {
            var options = new ModalOptions
            {
                Animation = new ModalAnimation(ModalAnimationType.FadeIn, 0.25),
                DisableBackgroundCancel = true
            };

            return options;
        }
    }

    public DialogService(IModalService modals)
    {
        Modals = modals;
    }

    public IModalReference Show<TDialog>() where TDialog : IDialog
    {
        return Modals.Show<TDialog>(string.Empty, DefaultOptions);
    }

    public IModalReference Show<TDialog, TParameters>(TParameters parameters) where TDialog : IDialog
    {
        var modalParameters = new ModalParameters();
        modalParameters.Add("Parameters", parameters);

        return Modals.Show<TDialog>(string.Empty, modalParameters, DefaultOptions);
    }
}
