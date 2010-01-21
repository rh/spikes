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
		private static void Main(string[] args)
		{
			// Logging makes everything very slow!
			//			foreach (string file in Directory.GetFiles(".", "*.log*"))
			//			{
			//				Console.WriteLine("Deleting '{0}'.", file);
			//				File.Delete(file);
			//			}
			//			Console.WriteLine();
			//
			//			XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

			try
			{
				Dictionary<string, string> properties = new Dictionary<string, string>();
				properties[Environment.ProxyFactoryFactoryClass] = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";

				if (args.Length > 0 && args[0] == "sqlserver")
				{
					Console.WriteLine("Using SQL Server");
					properties[Environment.ConnectionDriver] = "NHibernate.Driver.SqlClientDriver";
					properties[Environment.Dialect] = "NHibernate.Dialect.MsSql2005Dialect";
					properties[Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
					properties[Environment.ConnectionString] = "Server=(local);Initial Catalog=test;Integrated Security=SSPI;";
				}
				else
				{
					Console.WriteLine("Using SQLite");
					properties[Environment.ConnectionDriver] = "NHibernate.Driver.SQLite20Driver";
					properties[Environment.Dialect] = "NHibernate.Dialect.SQLiteDialect";
					properties[Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
					properties[Environment.ConnectionString] = "Data Source=test.db;Version=3;New=False;Compress=True;";
				}

				properties[Environment.CacheProvider] = "NHibernate.Caches.SysCache.SysCacheProvider, NHibernate.Caches.SysCache";
				properties[Environment.CacheDefaultExpiration] = "120";
				properties[Environment.UseSecondLevelCache] = "true";
				properties[Environment.UseQueryCache] = "true";
				properties[Environment.QuerySubstitutions] = "true=1;false=0";
				properties[Environment.ReleaseConnections] = "on_close";
				properties[Environment.BatchSize] = "2000";
				properties[Environment.GenerateStatistics] = "true";
				//properties[Environment.ShowSql] = "true";

				Configuration cfg = new Configuration();
				cfg.Properties = properties;
				cfg.AddAssembly(typeof(Node).Assembly);

				Console.WriteLine();
				new SchemaExport(cfg).Execute(true, true, false);
				Console.WriteLine();

				const int Count = 5;

				ISessionFactory factory = cfg.BuildSessionFactory();

				List<Guid> ids = new List<Guid>();

				Stopwatch stopwatch = Stopwatch.StartNew();
				using (ISession session = factory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						for (int i = 0; i < Count; i++)
						{
							Node inode = new Node(i.ToString());
							session.Save(inode);
							for (int j = 0; j < Count + Count; j++)
							{
								Node jnode = new Node(string.Format("{0}.{1}", i, j));
								jnode.Root = inode;
								inode.Add(jnode);
								for (int k = 0; k < Count + Count; k++)
								{
									Node knode = new Node(string.Format("{0}.{1}.{2}", i, j, k));
									knode.Root = inode;
									jnode.Add(knode);
									for (int l = 0; l < Count + Count; l++)
									{
										Node lnode = new Node(string.Format("{0}.{1}.{2}.{3}", i, j, k, l));
										lnode.Root = inode;
										knode.Add(lnode);
									}
								}
							}
							ids.Add(inode.Id);
						}
						transaction.Commit();
					}
				}

				stopwatch.Stop();
				Console.WriteLine("Inserting:      {0,4} ms", stopwatch.ElapsedMilliseconds);
				Console.WriteLine("Total created:   {0}", Node.Count);
				Console.WriteLine("Total inserted:  {0}", factory.Statistics.EntityInsertCount);

				for (int i = 0; i < 2; i++)
				{
					using (ISession session = factory.OpenSession())
					{
						using (ITransaction transaction = session.BeginTransaction())
						{
							foreach (Guid id in ids)
							{
								stopwatch.Reset();
								stopwatch.Start();

								// NOTE: setting the batch-size in the mapping makes this slower!
								Node node = session.Load<Node>(id);
								int total = 0;
								Loop(node, 0, delegate { total++; });

								stopwatch.Stop();
								Console.WriteLine("{0}: {1,4} ms ({2} records)", id, stopwatch.ElapsedMilliseconds, total);
							}

							transaction.Commit();
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR: " + e);
				return;
			}
		}

		public delegate void Action();

		private static void Loop(Node node, int level, Action action)
		{
			action();
			//Console.WriteLine("{0}  {1}: {2}", new string(' ', level), node.Name, node.Children.Count);
			foreach (Node child in node.Children)
			{
				Loop(child, level + 2, action);
			}
		}
	}
}