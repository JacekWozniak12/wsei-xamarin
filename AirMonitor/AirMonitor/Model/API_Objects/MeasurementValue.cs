using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AirMonitor.Model
{
    [JsonObject(
        Id = "value",
        NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MeasurementValue 
    {
        public string Name { get; set; }
        public double Value { get; set; }
    };

}
