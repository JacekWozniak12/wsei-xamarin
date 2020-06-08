using AirMonitor.Models;
using AirMonitor.Models.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
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
                Console.WriteLine("-----DATABASE READY-----");
                InitializeTables();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitializeTables()
        {
            Console.WriteLine("-----CREATING TABLES-----");
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

        public void SaveInstallation(IEnumerable<Installation> installations)
        {
            DB.BeginTransaction();
            try
            {
                DB.DeleteAll<InstallationEntity>();
                var Entities = new List<InstallationEntity>();
                foreach (var e in installations)
                {
                    Entities.Add(new InstallationEntity(e));
                }

                DB.InsertAll(Entities);
                Console.WriteLine(DB.Table<InstallationEntity>().Count());
            }
            catch (SQLiteException e)
            {
                Trace.WriteLine(e);
                DB.Rollback();
            }
            DB.Commit();
        }

        public void Insert<T>(SQLiteConnection connection, T insertable)
        {

        }

        public void Transaction()
        {
            DB.BeginTransaction();

            DB.Commit();
        }

        public void Dispose()
        {
            DB = null;
        }
    }
}
