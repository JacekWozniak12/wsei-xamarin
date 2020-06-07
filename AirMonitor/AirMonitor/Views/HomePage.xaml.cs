﻿using System;
using System.Collections.Generic;
using AirMonitor.Model;
using AirMonitor.ViewModels;
using Xamarin.Forms;

namespace AirMonitor.Views
{
    public partial class HomePage : ContentPage
    {
        private HomeViewModel viewModel => BindingContext as HomeViewModel;

        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel(Navigation);
        }

        void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            viewModel.GoToDetailsCommand.Execute(e.Item as Measurements);
        }
    }


}
