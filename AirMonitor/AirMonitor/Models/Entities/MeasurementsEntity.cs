using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirMonitor.Models.Entities
{
    public class MeasurementsEntity
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public int Current { get; set; }
        public int Installation { get; set; }

        public MeasurementsEntity() 
        {
            
        }

    }
}
