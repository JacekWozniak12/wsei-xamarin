using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AirMonitor.Model
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    };

}
