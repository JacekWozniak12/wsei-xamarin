using System;
using System.Collections.Generic;
using System.Text;

namespace AirMonitor.Models.Map
{
    public class MapLocation
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public  Xamarin.Forms.Maps.Position Position { get; set; }
        
    }
}
