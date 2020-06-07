using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AirMonitor.Model;
using AirMonitor.Views;
using Xamarin.Forms;

namespace AirMonitor.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        private bool isLoading;
        public bool IsLoading {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        } 

        private readonly INavigation _navigation;

        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
            DataHandler = new DataHandler();
            CollectData();
        }

        private ICommand _goToDetailsCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand ?? (_goToDetailsCommand = new Command<Measurements>(OnGoToDetails));

        private void OnGoToDetails(Measurements item)
        {
            _navigation.PushAsync(new DetailsPage(item));
        }

        DataHandler dataHandler;
        DataHandler DataHandler
        {
            get => dataHandler ?? new DataHandler();
            set => SetProperty(ref dataHandler, value);
        }

        private ObservableCollection<Measurements> items;
        public ObservableCollection<Measurements> Items
        {
            get 
            {
                if (items == null) items = new ObservableCollection<Measurements>();
                return items;
            }
            set
            {
                if (items != value)
                {
                    items = value;
                    RaisePropertyChanged();
                }
            }
        }

        public async Task CollectData()
        {
            IsLoading = true;
            var a = DataHandler.Run();
            await a.ContinueWith(b => SetItems(b.Result));
            IsLoading = false;
        }

        public async Task SetItems(List<Measurements> m)
        {
            Items = new ObservableCollection<Measurements>(m);
            Console.WriteLine("I:" + Items.Count);
        }
    }
}
