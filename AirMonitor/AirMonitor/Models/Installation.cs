using System;
using AirMonitor.Models.Entities;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace AirMonitor.Models
{
    public class Installation
    {
        public Installation(InstallationEntity entity)
        {
            Id = 
                entity.Id;
            Location = 
                JsonConvert.DeserializeObject<Xamarin.Essentials.Location>(entity.Location);
            Address = 
                JsonConvert.DeserializeObject<Address>(entity.Address);
        }

        public Installation() { }

        public string Id { get; set; }
        public Xamarin.Essentials.Location Location { get; set; }
        public Address Address { get; set; }
        public double Elevation { get; set; }
        [JsonProperty(PropertyName = "airly")]
        public bool IsAirlyInstallation { get; set; }
    }
}
