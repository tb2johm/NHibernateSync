using System.Diagnostics;
using System.IO;
using System.Reflection;
using DevToolsDatabaseLayer;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace NHTest.Model
{
    /// <summary>
    /// This part of the NHibernateHelper is to divide the functionality for synchronization
    /// out from the standard file
    /// </summary>
    public static partial class NHibernateHelper
    {

        public static class SyncHelper
        {
            private static ISession _currentSyncSession;
            private static ISessionFactory _syncSessionFactory;
            private const string DBSyncFilePath = @"..\..\Database\MyDBSync.sqlite";

            public static ISession GetCurrentSyncSession()
            {
                ISessionFactory factory = GetSyncSessionFactory();

                if (_currentSyncSession != null) return _currentSyncSession;

                _currentSyncSession = factory.OpenSession();

                return _currentSyncSession;
            }

            private static ISessionFactory GetSyncSessionFactory()
            {
                ISessionFactory factory = null;

                factory = _syncSessionFactory;

                if (factory != null) return factory;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                factory = OpenSyncSession(DBSyncFilePath);

                sw.Stop();

                return factory;
            }

            private static ISessionFactory OpenSyncSession(string fileName)
            {
                Configuration configuration = new Configuration();

                // Register our custom linq extensions
                configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();

                string destinationConStr = "Data Source = " + fileName;

                configuration.SetProperty(Environment.Dialect, "NHibernate.Dialect.SQLiteDialect");
                configuration.SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver");
                configuration.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                configuration.SetProperty(Environment.ConnectionString, destinationConStr);
                //configuration.SetProperty(NHibernate.Cfg.Environment.Isolation, "ReadUncommitted");

                // Display all sql querries from nHibernate. Formatted is nicer but slower
                configuration.SetProperty(Environment.FormatSql, "true");
                configuration.SetProperty(Environment.ShowSql, "true");

                configuration.AddAssembly(Assembly.GetCallingAssembly());

                configuration.SetProperty(Environment.CurrentSessionContextClass,
                    typeof (NHibernate.Context.ThreadStaticSessionContext).AssemblyQualifiedName);

                // Remove all Identity properties
                var tmp = (configuration.GetClassMapping(typeof(Producer)).Key as NHibernate.Mapping.SimpleValue);
                if (tmp != null) tmp.IdentifierGeneratorStrategy = "assigned";
                tmp = (configuration.GetClassMapping(typeof(ProductLinkProducer)).Key as NHibernate.Mapping.SimpleValue);
                if (tmp != null) tmp.IdentifierGeneratorStrategy = "assigned";
                tmp = (configuration.GetClassMapping(typeof(Product)).Key as NHibernate.Mapping.SimpleValue);
                if (tmp != null) tmp.IdentifierGeneratorStrategy = "assigned";

                configuration.BuildMappings();
                _syncSessionFactory = configuration.BuildSessionFactory();

                return _syncSessionFactory;
            }

            public static void CreateOfflineSQLiteDatabase(string filename = DBSyncFilePath)
            {
                if (File.Exists(filename)) File.Delete(filename);

                var configuration = new Configuration();

                string sqLiteConnectionString = "Data Source =" + filename;

                configuration.SetProperty(Environment.Dialect, "NHibernate.Dialect.SQLiteDialect");
                configuration.SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver");
                configuration.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                configuration.SetProperty(Environment.ConnectionString, sqLiteConnectionString);
                configuration.AddAssembly(Assembly.GetCallingAssembly());

                var export = new SchemaExport(configuration);
                export.Execute(false, true, false);
            }
        }
    }
}
