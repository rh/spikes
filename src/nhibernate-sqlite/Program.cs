using System;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Spikes.Model;
using Environment=NHibernate.Cfg.Environment;

namespace Spikes
{
	internal class Program
	{
		private static void Main()
		{
			try
			{
				Dictionary<string, string> properties = new Dictionary<string, string>();
				properties[Environment.ConnectionDriver] = "NHibernate.Driver.SQLite20Driver";
				properties[Environment.Dialect] = "NHibernate.Dialect.SQLiteDialect";
				properties[Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
				properties[Environment.ConnectionString] = "Data Source=test.db;Version=3;New=False;Compress=True;";
				properties[Environment.QuerySubstitutions] = "true=1;false=0";
				properties[Environment.ReleaseConnections] = "on_close";

				Configuration cfg = new Configuration();
				cfg.Properties = properties;
				cfg.AddAssembly(typeof(Test).Assembly);

				Console.WriteLine();
				new SchemaExport(cfg).Execute(true, true, false, true);
				Console.WriteLine();

				const int count = 20000;

				Stopwatch stopwatch = Stopwatch.StartNew();

				ISessionFactory factory = cfg.BuildSessionFactory();
				using (ISession session = factory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						for (int i = 0; i < count; i++)
						{
							Test test = new Test(i.ToString());
							session.Save(test);
						}
						transaction.Commit();
					}
				}

				stopwatch.Stop();
				Console.WriteLine("{0} records: {1} ms", count, stopwatch.ElapsedMilliseconds);
			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR: " + e);
				return;
			}
		}
	}
}