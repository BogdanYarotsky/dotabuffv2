using System;
using System.Collections.Generic;
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
using DotabuffClient.ViewModels;
using DotabuffVisualizer;

namespace DotabuffClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM _mainWindowVM;

        public MainWindow(DotabuffCrawler crawler)
        {
            InitializeComponent();
            _mainWindowVM = new MainWindowVM { Crawler = crawler };
            DataContext = _mainWindowVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
