using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AirMonitor.Model
{
    [JsonObject(
        Id = "standard",
        NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class AirQualityStandard 
    {
        public string Name { get; set; }
        public string Pollutant { get; set; }
        public string Limit { get; set; }
        public double Percent { get; set; }
    };
}
