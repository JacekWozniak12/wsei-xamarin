using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AirMonitor.Models.Entities
{
    public class InstallationEntity
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }

        public InstallationEntity() 
        { 
            
        }

        public InstallationEntity(Installation installation)
        {
            Id = installation.Id;
            try
            {
                Address = JsonConvert.SerializeObject(installation.Address);
                Location = JsonConvert.SerializeObject(installation.Location);
            }
            catch (JsonException e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}
