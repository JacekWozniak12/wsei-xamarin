using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace AirMonitor.Model
{
    // https://www.newtonsoft.com/json/help/html/NamingStrategyAttributes.htm
    public class DataHandler
    {
        HttpClient httpClient;

        public List<Installation> Installations;
        public List<Measurements> Measurements;

        public DataHandler() 
        {                  
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("apikey", App.AirlyApiKey);    
        }

        public async Task<string> GetInstallationsAddress
            (bool useGeo, double distance = 5, int amount = 1)
        {
            var result = 
                $"{App.AirlyApiUrl}{App.AirlyApiInstallationUrl}?lat={50.062006}&lng={19.940984}&maxDistanceKM={distance}&maxResults=1";

            if (useGeo)
            {               
                var cts = new CancellationTokenSource(3000);
                Task.Run<string>(async () =>
                {                 
                    while (!cts.Token.IsCancellationRequested)
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();
                        if (location != null)
                        {
                            Console.WriteLine("USING GEO LOG");
                            Console.WriteLine("FINISHED");
                            return result =
                                $"{App.AirlyApiUrl}" +
                                $"{App.AirlyApiInstallationUrl}?lat=" +
                                $"{location.Latitude}&lng=" +
                                $"{location.Longitude}" +
                                $"&maxDistanceKM={distance}" +
                                $"&maxResults={amount}";                           
                       }                       
                    }
                    return result;
                }
                , cts.Token);               
            }
            return result;
        }

        public string GetMeasurementsAddress(int id)
        {
            return $"{App.AirlyApiUrl}{App.AirlyApiMeasurementUrl}?installationId={id}";
        }

        public async Task<List<Measurements>> Run()
        {
            Measurements = new List<Measurements>();
            Installations = new List<Installation>();

            string response = 
                await CollectData(new Uri(await GetInstallationsAddress(true, 225, 5))
                );
            Installations = await ParseData<List<Installation>>(response);

            return await GetMeasurements(Installations);
        }

        public async Task<List<Measurements>> GetMeasurements(List<Installation> installations)
        {
            foreach(var e in installations)
            {
                await GetAndSetMeasurement(e);
            }
            return Measurements;
        }

        private async Task GetAndSetMeasurement(Installation e)
        {
            var r = await CollectData(new Uri(GetMeasurementsAddress(e.Id)));
            var s = await ParseData<Measurements>(r, e.Id.ToString());
            s.Installation = e;
            s.CurrentDisplayValue = (int)Math.Round(s.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
            Measurements.Add(s);
        }

        public async Task<T> ParseData<T>(string data, string additional = null)
        {
            try
            {
                T x = await Task.FromResult<T>(JsonConvert.DeserializeObject<T>(data));
                Console.WriteLine($"{{PARSED}} {additional}");
                return x;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine("\nException Caught!\n Message:{0} ", e.Message);
                return default;
            }
        }

        public async Task<string> CollectData(Uri address)
        {
            try
            {
                var x = await httpClient.GetAsync(address);
                ShowRateLimits(x);
                switch ((int)x.StatusCode)
                {
                    case 200:
                        var content = await x.Content.ReadAsStringAsync();
                        return content;
                    case 429: // too many requests
                        System.Diagnostics.Debug.WriteLine("Too many requests");
                        break;
                    default:
                        var errorContent = await x.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Response error: {errorContent}");
                        return default;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!\n Message:{0} ", e.Message);
            }

            return default;
        }

        private static void ShowRateLimits(HttpResponseMessage x)
        {
            if (x.Headers.TryGetValues("X-RateLimit-Limit-day", out var dayLimit) &&
                x.Headers.TryGetValues("X-RateLimit-Remaining-day", out var dayLimitRemaining))
            {
                Debug.WriteLine($"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");
            }
        }
    }
}
