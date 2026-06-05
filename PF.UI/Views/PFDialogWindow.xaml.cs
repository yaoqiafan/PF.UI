using Prism.Dialogs;

namespace PF.UI.Views
{
    public partial class PFDialogWindow : PF.UI.Controls.Window, IDialogWindow
    {
        public IDialogAware ViewModel
        {
            get => DataContext as IDialogAware;
            set => DataContext = value;
        }

        public IDialogResult Result { get; set; }

        public PFDialogWindow()
        {
            InitializeComponent();
        }
    }
}
