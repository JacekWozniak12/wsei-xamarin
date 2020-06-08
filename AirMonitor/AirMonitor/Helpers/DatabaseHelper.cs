using AirMonitor.Models;
using AirMonitor.Models.Entities;
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

        public DateTime TillDateTime;

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
                Console.WriteLine("-----DATABASE READY-----");
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
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
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SaveInstallation(IEnumerable<Installation> installations)
        {
            await new Task(
            () => {
                try
                {
                    DB.BeginTransaction();
                    DB.DeleteAll<InstallationEntity>();
                    var Entities = new List<InstallationEntity>();
                    foreach (var e in installations)
                    {
                        Entities.Add(new InstallationEntity(e));
                    }

                    DB.InsertAll(Entities, false);
                    Console.WriteLine(DB.Table<InstallationEntity>().Count());
                }
                catch (SQLiteException e)
                {
                    Trace.WriteLine(e);
                    DB.Rollback();
                }
                DB.Commit();
            });
        }

        public async Task<IEnumerable<Installation>> GetInstallations()
        {
            var List = new List<Installation>();
            await new Task(
            () =>
            {
                try
                {
                    DB.BeginTransaction();
                    var InstallationTable = DB.Table<InstallationEntity>();

                    foreach (var Entity in InstallationTable)
                    {
                        List.Add(new Installation(Entity));
                    }

                    DB.Commit();
                }
                catch (SQLiteException e)
                {
                    Trace.WriteLine(e);
                    DB.Rollback();
                }
            }
            );

            return List;
        }

        public void SaveMeasurements(IEnumerable<Measurement> measurements)
        {
            DB.BeginTransaction();
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
        }

        public void Dispose()
        {
            DB = null;
        }
    }
}
