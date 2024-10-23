using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
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

namespace weatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApiController controller;

        /*
        // test konzole
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32", SetLastError = true)]
        public static extern void FreeConsole();
        */


        public MainWindow()
        {
            InitializeComponent();
            // AllocConsole();
            controller = new ApiController();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Visibility = Visibility.Visible;
            var data = controller.looseSearch(SearchBar.Text);
            SearchBox.ItemsSource = data;
        }


        private void SearchBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchBox.SelectedIndex == -1)
            {
                return;
            }
            var mesto = SearchBox.Items.GetItemAt(SearchBox.SelectedIndex);
            if (mesto == null)
            {
                return;
            }
            string result = Task.Run(() => controller.getWeatherReport((Mesto)mesto)).Result;
            var json = JsonDocument.Parse(result);
            CityWeatherWindow cww = new CityWeatherWindow((Mesto)mesto, json, controller.weatherDescriptor);
            cww.ShowDialog();
        }
    }
}
