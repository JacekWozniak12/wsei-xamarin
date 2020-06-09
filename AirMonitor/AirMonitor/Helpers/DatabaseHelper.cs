using AirMonitor.Models;
using AirMonitor.Models.Entities;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace AirMonitor
{

    public class DatabaseHelper : IDisposable
    {
        string path;

        SQLiteConnection DB { get; set; }

        public DatabaseHelper()
        {
            CreateConnection();
        }

        public void CreateConnection()
        {
            try
            {
                Console.WriteLine("-----CREATING DATABASE-----");
                path = 
                    Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "database.db3");
                /* zadanie 3 */
                DB = new SQLiteConnection(
                    path,
                    SQLiteOpenFlags.ReadWrite |
                    SQLiteOpenFlags.Create |
                    SQLiteOpenFlags.FullMutex
                    );               
                InitializeTables(); 
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private void InitializeTables()
        {            
            try
            {
                DB.CreateTable<InstallationEntity>();
                DB.CreateTable<MeasurementItemEntity>();
                DB.CreateTable<MeasurementsEntity>();
                DB.CreateTable<MeasurementValue>();
                DB.CreateTable<AirQualityIndex>();
                DB.CreateTable<AirQualityStandard>();
                Console.WriteLine("-----DATABASE READY-----");
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        public async Task SaveInstallation(IEnumerable<Installation> installations)
        {
            try
            {
                Console.WriteLine("-----SAVING INSTALLATION DATA-----");
                DB.BeginTransaction();
                DB.DeleteAll<InstallationEntity>();
                var Entities = new List<InstallationEntity>();
                foreach (var e in installations)
                {
                    Entities.Add(new InstallationEntity(e));
                }
                DB.InsertAll(Entities, false);
                Console.WriteLine(DB.Table<InstallationEntity>().Count());
                Console.WriteLine("-----SAVED INSTALLATION DATA-----");
            }
            catch (SQLiteException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
                return;
            }
            catch (InvalidCastException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
                return;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine(e);
                DB.Rollback();
                return;
            }
            DB.Commit();        
        }

        public async Task<IEnumerable<Installation>> GetInstallations()
        {
            var List = new List<Installation>();

            try
            {
                Console.WriteLine("-----GETTING INSTALLATION DATA-----");
                DB.BeginTransaction();
                var InstallationTable = DB.Table<InstallationEntity>();

                foreach (var Entity in InstallationTable)
                {
                    List.Add(new Installation(Entity));
                }

                DB.Commit();
                Console.WriteLine("-----GOT INSTALLATION DATA-----");
            }
            catch (SQLiteException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
                return null;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine(e);
                DB.Rollback();
                return null;
            }
            return List;
        }          

        public async Task<MeasurementItem> GetMeasurementItem()
        {
            MeasurementItem Item = null;

            try
            {
                DB.BeginTransaction();
                var Entity = DB.Table<MeasurementsEntity>().FirstOrDefault();
                Item = GetMeasurementItem(Entity);
                DB.Commit();
            }
            catch (SQLiteException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
                return null;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine(e);
                DB.Rollback();
                return null;
            }

            return Item;
        }

        public bool CheckForUpdateRequest(IEnumerable<Measurement> toCheck)
        {
            var currentData = DateTime.UtcNow;
            if (toCheck == null || toCheck.Count() == 0) return true;
            return toCheck.Any(x => x.Current.TillDateTime - currentData > new TimeSpan(1, 0, 0));
        }

        public async Task<IEnumerable<Measurement>> GetMeasurements()
        {
            var List = new List<Measurement>();

            try
            {
                Console.WriteLine("-----GETTING MEASUREMENT DATA-----");
                var MeasurementItemList = new List<MeasurementItem>();
                DB.BeginTransaction();
                var Table = DB.Table<MeasurementsEntity>();
                    
                foreach (var Entity in Table)
                {
                    var ie = DB.Query<InstallationEntity>
                        ("SELECT * FROM InstallationEntity WHERE Id = ?", Entity.Installation)?.FirstOrDefault();
                    var i = new Installation(ie);

                    MeasurementItem mi = GetMeasurementItem(Entity);
                    MeasurementItemList.Add(mi);

                    var m = new Measurement();
                    m.Current = mi;
                    m.Installation = i;

                    List.Add(m);
                }
                DB.Commit();
                Console.WriteLine("-----GOT MEASUREMENT DATA-----");
            }
            catch (SQLiteException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
                return null;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine(e);
                DB.Rollback();
                return null;
            }

            return List;
        }

        private MeasurementItem GetMeasurementItem(MeasurementsEntity Entity)
        {
            MeasurementItem mi = null;
            try
            {
                Console.WriteLine("-----GETTING ENTITY:{0}----", Entity.Id);
                var mie = DB.Query<MeasurementItemEntity>
                 ("SELECT * FROM MeasurementItemEntity WHERE Id = ?", Entity.Current)?.FirstOrDefault();
                
                Console.WriteLine("-----GOT ENTITY:{0}----", Entity.Id);
                var mieVs = JsonConvert.DeserializeObject<List<MeasurementValue>>(mie.Values);
                var mieIs = JsonConvert.DeserializeObject<List<AirQualityIndex>>(mie.Indexes);
                var mieSs = JsonConvert.DeserializeObject<List<AirQualityStandard>>(mie.Standards);
                
                Console.WriteLine("-----GOT DATA:{0}----", Entity.Id);
                mi = new MeasurementItem();
                mi.Indexes = mieIs.ToArray();
                mi.Standards = mieSs.ToArray();
                mi.Values = mieVs.ToArray();
                mi.FromDateTime = mie.FromDateTime;
                mi.FromDateTime = mie.TillDateTime;
                
                Console.WriteLine("-----CREATED ITEM:{0}----", Entity.Id);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
            } 
            catch (JsonReaderException e)
            {
                Console.WriteLine(e);
            }
            return mi;
        }

        public void SaveMeasurements(IEnumerable<Measurement> measurements)
        {
            DB.BeginTransaction();
            Console.WriteLine("-----SAVING MEASUREMENT DATA-----");
            try
            {
                DB.DeleteAll<MeasurementItemEntity>();
                DB.DeleteAll<MeasurementsEntity>();
                DB.DeleteAll<MeasurementValue>();
                DB.DeleteAll<AirQualityIndex>();
                DB.DeleteAll<AirQualityStandard>();
                
                foreach (var e in measurements)
                {
                    DB.InsertAll(e.Current.Indexes, false);
                    DB.InsertAll(e.Current.Values, false);
                    DB.InsertAll(e.Current.Standards, false);
                    var a = new MeasurementItemEntity(e.Current);
                    DB.Insert(a);
                    DB.Insert(new MeasurementsEntity(a.Id, int.Parse(e.Installation.Id)));
                }

            }
            catch (SQLiteException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
            }
            catch (InvalidCastException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
            }
            DB.Commit();
            Console.WriteLine("-----SAVED MEASUREMENT DATA-----");
        }

        public void Dispose()
        {
            DB.Dispose();
            DB = null;
        }
    }
}
