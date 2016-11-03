using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NHibernate;
using NHibernate.Linq;
using NHTest.Model;

namespace NHTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init sessions
            ISession session = NHibernateHelper.GetCurrentSession();
            ISession syncSession = NHibernateHelper.SyncHelper.GetCurrentSyncSession();

            Console.WriteLine("Creating new SyncDatabase file...");
            NHibernateHelper.SyncHelper.CreateOfflineSQLiteDatabase();
            Console.WriteLine("...Done\n");

            Sync<Producer>("Producer", session, syncSession);
            Sync<Product>("Product", session, syncSession);
            Sync<ProductLinkProducer>("ProductLinkProducer", session, syncSession);

            Thread.Sleep(1000);
        }

        private static void Sync<T>(string tableName, ISession session, ISession syncSession)
        {
            Console.WriteLine("Fetching data for ####{0}####...", tableName);
            List<T> links;
            var sqlLinks = session.Query<T>();
            links = sqlLinks.ToList();
            Console.WriteLine("...Done");

            Console.WriteLine("Evicting data...");
            links.ForEach(x => session.Evict(x));
            Console.WriteLine("...Done");

            Console.WriteLine("Saving data...");

            links.ForEach(x => syncSession.Save(x));
            Console.WriteLine("...Flushing data...");
            syncSession.Flush();
            Console.WriteLine("...Done");
            Console.WriteLine("\n\n\n");
        }
    }
}
