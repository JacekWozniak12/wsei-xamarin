using AirMonitor.Views.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AirMonitor.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage()
        {
            InitializeComponent();
            BindingContext = new DetailsViewModel();
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Co to jest tex?", "Lorem ipsum.", "Zamknij");
        }
    }

    internal class RoutedPropertyChangedEventArgs<T>
    {
    }
}
