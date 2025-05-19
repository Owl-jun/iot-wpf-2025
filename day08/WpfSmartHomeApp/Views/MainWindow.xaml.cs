using System.Windows;
using System.Windows.Input;
using WpfSmartHomeApp.ViewModels;

namespace WpfSmartHomeApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Alt + F4 , X 버튼 누를때 호출 기능
            e.Cancel = true;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadedCommand.Execute(null); 
            }
        }
    }
}