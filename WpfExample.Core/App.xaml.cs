using System.Windows;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new MainWindow
            {
                DataContext = new ExampleViewModel()
            }
            .Show();
        }
    }
}
