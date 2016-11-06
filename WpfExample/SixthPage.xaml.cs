using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for SixthPage.xaml
    /// </summary>
    public partial class SixthPage : UserControl
    {
        public SixthPage()
        {
            InitializeComponent();
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as ExampleViewModel);
            vm.Items = items;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as ExampleViewModel);
            vm.Items2 = items2;
        }

        ObservableCollection<GridItemViewModel> items;
        ObservableCollection<GridItemViewModel> items2;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as ExampleViewModel);
            items = vm.FillItems(100000);
            items2 = vm.FillItems(100000);
        }
    }
}
