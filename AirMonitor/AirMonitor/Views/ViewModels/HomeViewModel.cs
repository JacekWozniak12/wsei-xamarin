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
        public ICommand Command { get; private set; }

        public HomeViewModel(Xamarin.Forms.INavigation navigation)
        {
            Navigation = navigation;
            Command.Execute(navigation.PushAsync(new DetailsPage()));
        }
    }

    class HomeCommand
    {
        
    }
}
