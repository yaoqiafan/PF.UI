using PF.UI.Controls;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Dialogs;
using System.Windows.Media;

namespace PF.UI.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel : BindableBase, IDialogAware
    {
        public DialogCloseListener RequestClose { get; } = new();

        private string _dialogTitle = "确认";
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        private string _message = "";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private PackIconKind _iconKind = PackIconKind.AlertOutline;
        public PackIconKind IconKind
        {
            get => _iconKind;
            set => SetProperty(ref _iconKind, value);
        }

        private Brush _iconBackground = new SolidColorBrush(Color.FromRgb(255, 152, 0));
        public Brush IconBackground
        {
            get => _iconBackground;
            set => SetProperty(ref _iconBackground, value);
        }

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue("title",   out string title))   DialogTitle = title;
            if (parameters.TryGetValue("message", out string message)) Message = message;
            if (parameters.TryGetValue("icon",    out PackIconKind icon)) IconKind = icon;
            if (parameters.TryGetValue("color",   out string hex))
            {
                try { IconBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)); }
                catch { /* ignore invalid color */ }
            }
        }

        public DelegateCommand OkCommand     => new(() => RequestClose.Invoke(ButtonResult.OK));
        public DelegateCommand CancelCommand => new(() => RequestClose.Invoke(ButtonResult.Cancel));
    }
}
