using System.Reflection;
using DevToolsDatabaseLayer;
using NHibernate;
using NHibernate.Cfg;

namespace NHTest.Model
{
    public static partial class NHibernateHelper
    {
        private static ISession _currentSession;
        private static ISessionFactory _currentFactory;
        private const string DBFilePath = @"..\..\Database\MyDB.sqlite";

        
        public static ISession GetCurrentSession()
        {
            ISessionFactory factory = GetSessionFactory();

            if (!NHibernate.Context.CurrentSessionContext.HasBind(factory))
            {
                NHibernate.Context.CurrentSessionContext.Bind(factory.OpenSession());
            }

            _currentSession = factory.GetCurrentSession();

            return _currentSession;
        }

        public static ISessionFactory GetSessionFactory()
        {
            ISessionFactory factory = _currentFactory ?? OpenSession(DBFilePath);

            return factory;
        }




        internal static ISessionFactory OpenSession(string filePath)
        {
            var configuration = new Configuration();
            
            //if (!File.Exists(filePath)) throw new FileNotFoundException("Couldn't find file", filePath);

            // Register our custom linq extensions
            configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();

            string destinationConStr = "Data Source =" + filePath;
            configuration.SetProperty(Environment.Dialect, "NHibernate.Dialect.SQLiteDialect");
            configuration.SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver");
            configuration.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider"); 
            configuration.SetProperty(Environment.ConnectionString, destinationConStr);
            configuration.SetProperty(Environment.CurrentSessionContextClass, typeof(NHibernate.Context.ThreadStaticSessionContext).AssemblyQualifiedName);
            //configuration.SetProperty(Environment.Isolation, "ReadUncommitted"); // Make it possible to read during a long running transaction (e.g. import)

            configuration.SetProperty(Environment.FormatSql, "true");
            configuration.SetProperty(Environment.ShowSql, "true");

            configuration.AddAssembly(Assembly.GetCallingAssembly());
            
            configuration.SetProperty(Environment.CurrentSessionContextClass, typeof(NHibernate.Context.CallSessionContext).AssemblyQualifiedName);

            configuration.BuildMappings();
            _currentFactory = configuration.BuildSessionFactory();

            return _currentFactory;

        }
    }
}
