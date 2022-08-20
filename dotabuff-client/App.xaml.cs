using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DotabuffVisualizer;

namespace DotabuffClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DotabuffCrawler _dotabuffCrawler;
        protected override async void OnStartup(StartupEventArgs e)
        {
            _dotabuffCrawler = await DotabuffCrawler.StartAsync();
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow(_dotabuffCrawler);
            MainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _dotabuffCrawler.DisposeAsync();
            base.OnExit(e);
        }
    }
}
