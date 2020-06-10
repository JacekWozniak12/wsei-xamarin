using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using AirMonitor.Models;
using AirMonitor.Views;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Globalization;
using System.Web;
using Xamarin.Forms.Internals;
using AirMonitor.Models.Map;
using Xamarin.Forms.Maps;

namespace AirMonitor.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public List<MapLocation> mapLocations;

        private readonly INavigation _navigation;

        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
            Initialize();
        }

        private async Task Initialize()
        {
            IsBusy = true;
            await GetData();
            IsBusy = false;
        }

        private async Task GetData(bool forcedFromWeb = false)
        {
            var location = await GetLocation();
            IEnumerable<Measurement> m = await App.databaseHelper.GetMeasurements();
            IEnumerable<Installation> i = await App.databaseHelper.GetInstallations();

            if (forcedFromWeb || m == null || i == null ||
                App.databaseHelper.CheckForUpdateRequest(m) ||
                IsLocationChangedOrNull(location, i))
            {
                Console.WriteLine("--GETTING DATA FROM API--");
                i = await GetInstallations(location, maxResults: 3);
                m = await GetMeasurementsForInstallations(i);
                if(i != null && m != null) SaveData(i, m);
            }

            if(i != null && m != null) GetItems(m);
        }

        private static Task SaveData(IEnumerable<Installation> i, IEnumerable<Measurement> m)
        {
            return Task.Run(() =>
            {
                App.databaseHelper.SaveInstallation(i);
                App.databaseHelper.SaveMeasurements(m);
            }
            );
        }

        private static bool IsLocationChangedOrNull(Xamarin.Essentials.Location location, IEnumerable<Installation> i)
        {
            bool r = false;
            if (i == null || location == null) return true;
            foreach (var e in i)
            {
                if (
                    Math.Round(e.Location.Latitude, 3)
                    !=
                    Math.Round(location.Latitude, 3)
                    &&
                    Math.Round(e.Location.Longitude, 3)
                    !=
                    Math.Round(location.Longitude, 3)
                    )
                {
                    r = true;
                    break;
                }
            };
            return r;
        }

        public void GetItems(
            IEnumerable<Measurement> measurements
            )
        {
            Items = new List<Measurement>(measurements);
            mapLocations = Items.Select(i => new MapLocation
            {
                Address = i.Installation.Address.Description,
                Description = "CAQI: " + i.CurrentDisplayValue,
                Position = new Position(i.Installation.Location.Latitude, i.Installation.Location.Longitude)
            }).ToList();
        }

        private ICommand _goToDetailsCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand ?? (_goToDetailsCommand = new Command<Measurement>(OnGoToDetails));

        private void OnGoToDetails(Measurement item)
        {
            _navigation.PushAsync(new DetailsPage(item));
        }

        private ICommand _infoWindowClickedCommand;
        public ICommand InfoWindowClickedCommand => _infoWindowClickedCommand ?? (_infoWindowClickedCommand = new Command<Pin>(OnGoToDetails));

        private void OnGoToDetails(Xamarin.Forms.Maps.Pin pin)
        {
            Measurement item = Items.First<Measurement>(i => i.Installation.Address.Description.Equals(pin.Address));
            _navigation.PushAsync(new DetailsPage(item));
        }

        private List<Measurement> _items;
        public List<Measurement> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private bool _isBusy;
        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                SetProperty(ref _isRefreshing, value);
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (IsBusy) return;
                    else
                    {
                        IsRefreshing = true;
                        await GetData(forcedFromWeb: true);
                        IsRefreshing = false;
                    }
                });
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private async Task<IEnumerable<Installation>> GetInstallations
            (Xamarin.Essentials.Location location, double maxDistanceInKm = 50, int maxResults = -1)
        {
            if (location == null)
            {
                System.Diagnostics.Debug.WriteLine("No location data.");
                return null;
            }

            var query = GetQuery(new Dictionary<string, object>
            {
                { "lat", location.Latitude },
                { "lng", location.Longitude },
                { "maxDistanceKM", maxDistanceInKm },
                { "maxResults", maxResults }
            });
            var url = GetAirlyApiUrl(App.AirlyApiInstallationUrl, query);
            
            var response = await GetHttpResponseAsync<IEnumerable<Installation>>(url);
            return response;
        }

        private async Task<IEnumerable<Measurement>> GetMeasurementsForInstallations(IEnumerable<Installation> installations)
        {
            if (installations == null)
            {
                System.Diagnostics.Debug.WriteLine("No installations data.");
                return null;
            }

            var measurements = new List<Measurement>();

            foreach (var installation in installations)
            {
                var query = GetQuery(new Dictionary<string, object>
                {
                    { "installationId", installation.Id }
                });
                var url = GetAirlyApiUrl(App.AirlyApiMeasurementUrl, query);

                var response = await GetHttpResponseAsync<Measurement>(url);

                if (response != null)
                {
                    response.Installation = installation;
                    response.CurrentDisplayValue = (int)Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                    measurements.Add(response);
                }
            }

            return measurements;
        }

        private string GetQuery(IDictionary<string, object> args)
        {
            if (args == null) return null;

            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (var arg in args)
            {
                if (arg.Value is double number)
                {
                    query[arg.Key] = number.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    query[arg.Key] = arg.Value?.ToString();
                }
            }

            return query.ToString();
        }

        private string GetAirlyApiUrl(string path, string query)
        {
            var builder = new UriBuilder(App.AirlyApiUrl);
            builder.Port = -1;
            builder.Path += path;
            builder.Query = query;
            string url = builder.ToString();

            return url;
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(App.AirlyApiUrl);

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Add("apikey", App.AirlyApiKey);
            return client;
        }

        private async Task<T> GetHttpResponseAsync<T>(string url)
        {
            try
            {
                var client = GetHttpClient();
                var response = await client.GetAsync(url);

                if (response.Headers.TryGetValues("X-RateLimit-Limit-day", out var dayLimit) &&
                    response.Headers.TryGetValues("X-RateLimit-Remaining-day", out var dayLimitRemaining))
                {
                    System.Diagnostics.Debug.WriteLine($"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");
                }

                switch ((int)response.StatusCode)
                {
                    case 200:
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(content);
                        return result;
                    case 429: // too many requests
                        System.Diagnostics.Debug.WriteLine("Too many requests");
                        break;
                    default:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Response error: {errorContent}");
                        return default;
                }
            }
            catch (JsonReaderException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return default;
        }

        private async Task<Xamarin.Essentials.Location> GetLocation()
        {
            try
            {
                Xamarin.Essentials.Location location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                }

                return location;
            }
            // Handle different exceptions separately, for example to display different messages to the user
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (FeatureNotEnabledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (PermissionException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }
    }
}
