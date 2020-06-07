using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace AirMonitor.Model
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Measurements {
        public int CurrentDisplayValue { get; set; }
        public MeasurementsItem Current { get; set; }
        public List<MeasurementsItem> History { get; set; }
        public List<MeasurementsItem> Forecast { get; set; }
        public Installation Installation { get; set; }
    };
    
}
