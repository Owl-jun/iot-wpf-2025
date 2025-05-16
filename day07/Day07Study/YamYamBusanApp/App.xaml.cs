using MahApps.Metro.Controls.Dialogs;
using System.Configuration;
using System.Data;
using System.Windows;
using YamYamBusanApp.ViewModels;
using YamYamBusanApp.Views;

namespace YamYamBusanApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var view = make_view();
            view.ShowDialog();
        }
        private YamYamBusanView make_view()
        {
            IDialogCoordinator cd = DialogCoordinator.Instance;
            var viewModel = new YamYamBusanViewModel(cd);
            var view = new YamYamBusanView()
            {
                DataContext = viewModel,
            };
            return view;
        }
    }

}
