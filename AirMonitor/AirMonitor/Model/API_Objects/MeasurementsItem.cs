using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace AirMonitor.Model
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class MeasurementsItem
    {
        public DateTime FromDateTime { get; set; }
        public DateTime TillDateTime { get; set; }
        public List<MeasurementValue> Values { get; set; }
        public List<AirQualityIndex> Indexes { get; set; }
        public List<AirQualityStandard> Standards { get; set; }
    };
    
}
