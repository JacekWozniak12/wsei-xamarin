using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AirMonitor.Model
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Installation 
    { 
        public int Id { get; set; }
        public Address Address { get; set; }
        public Location Location { get; set; }
    };

}
