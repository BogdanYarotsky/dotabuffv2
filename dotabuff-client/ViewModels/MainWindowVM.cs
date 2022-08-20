using DotabuffVisualizer;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DotabuffClient.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private bool _searching = false;
        private string heroName;
        private double winrate;

        public DotabuffCrawler Crawler { private get; init; }
        public string SearchBarText { get; set; }
        public double Winrate
        {
            get => winrate; private set
            {
                winrate = value;
                NotifyPropertyChanged(nameof(Winrate));
            }
        }
        public string HeroName
        {
            get => heroName; private set
            {
                heroName = value;
                NotifyPropertyChanged(nameof(HeroName));
            }
        }
        public ObservableCollection<string> Items { get; private set; }

        public ICommand SearchButtonCommand { get; }

        public MainWindowVM()
        {
            SearchButtonCommand = new RelayCommand(SearchForHero, () => !_searching);
        }

        private async void SearchForHero()
        {
            SearchButtonCommand.CanExecute(false);
            var hero = await Crawler.GetHeroItemsAsync(SearchBarText);
            Winrate = hero.Winrate;
            HeroName = hero.HeroName;
            SearchButtonCommand.CanExecute(true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }
}
