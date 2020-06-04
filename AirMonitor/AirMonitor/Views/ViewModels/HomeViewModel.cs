using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AirMonitor.Views.ViewModels
{
    class HomeViewModel
    {
        public Xamarin.Forms.INavigation Navigation { get; private set; }

        public ICommand Command => 
            new Command(
                async () => {
                    await Navigation.PushAsync(new NavigationPage(new DetailsPage()));
            });

        public HomeViewModel(Xamarin.Forms.INavigation navigation)
        {
            Navigation = navigation;
        }
    }

}
