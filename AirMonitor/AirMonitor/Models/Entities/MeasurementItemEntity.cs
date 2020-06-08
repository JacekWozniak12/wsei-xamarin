using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirMonitor.Models.Entities
{
    public class MeasurementItemEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Values { get; set; }
        public string Indexes { get; set; }
        public string Standards { get; set; }
        public MeasurementItemEntity() { }
    }
}
