﻿using System;
using AirMonitor.Views;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using TabbedPage = Xamarin.Forms.TabbedPage;

namespace AirMonitor
{
    public partial class App : Xamarin.Forms.Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = CreateTabbedPage();
        }

        private TabbedPage CreateTabbedPage()
        {
            var TP = new TabbedPage();
            TP.Title = "Tabbed Page";
            TP.Children.Add(new NavigationPage(new SettingsPage())
            { 
                Title = "Settings", 
                IconImageSource = "settings.png"
            });
            TP.Children.Add(new NavigationPage(new HomePage()) 
            { 
                Title = "Home",
                IconImageSource = "home.png"
            });
            TP.On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            return TP;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
