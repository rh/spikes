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
				properties[Environment.ProxyFactoryFactoryClass] = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";
				properties[Environment.ConnectionDriver] = "NHibernate.Driver.SQLite20Driver";
				properties[Environment.Dialect] = "NHibernate.Dialect.SQLiteDialect";
				properties[Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
				properties[Environment.ConnectionString] = "Data Source=test.db;Version=3;New=False;Compress=True;";
				properties[Environment.QuerySubstitutions] = "true=1;false=0";
				properties[Environment.ReleaseConnections] = "on_close";
				properties[Environment.BatchSize] = "1000";

				Configuration cfg = new Configuration();
				cfg.Properties = properties;
				cfg.AddAssembly(typeof(Node).Assembly);

				Console.WriteLine();
				new SchemaExport(cfg).Execute(true, true, false);
				Console.WriteLine();

				const int StatelessCount = 100000;
				const int Count = 4;

				ISessionFactory factory = cfg.BuildSessionFactory();

				using (IStatelessSession session = factory.OpenStatelessSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						Node node = new Node("first");
						session.Insert(node);
						transaction.Commit();
					}
				}

				Stopwatch stopwatch = Stopwatch.StartNew();

				using (IStatelessSession session = factory.OpenStatelessSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						for (int i = 0; i < StatelessCount; i++)
						{
							Node node = new Node(i.ToString());
							session.Insert(node);
						}
						transaction.Commit();
					}
				}
				stopwatch.Stop();
				Console.WriteLine("{0} records (flat)", StatelessCount);
				Console.WriteLine("Inserting: {0,4} ms", stopwatch.ElapsedMilliseconds);

				List<Guid> ids = new List<Guid>();

				stopwatch.Reset();
				stopwatch.Start();
				using (ISession session = factory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						for (int i = 0; i < Count; i++)
						{
							Node inode = new Node(i.ToString());
							for (int j = 0; j < Count; j++)
							{
								Node jnode = new Node(j.ToString());
								inode.Add(jnode);
								for (int k = 0; k < Count; k++)
								{
									Node knode = new Node(k.ToString());
									jnode.Add(knode);
									for (int l = 0; l < Count; l++)
									{
										Node lnode = new Node(l.ToString());
										knode.Add(lnode);
									}
								}
							}
							session.Save(inode);
							ids.Add(inode.Id);
						}
						transaction.Commit();
					}
				}

				stopwatch.Stop();
				Console.WriteLine("{0} records (hierarchical)", Count * Count * Count * Count);
				Console.WriteLine("Inserting: {0,4} ms", stopwatch.ElapsedMilliseconds);

				using (ISession session = factory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						stopwatch.Reset();
						stopwatch.Start();

						IQuery query = session.CreateQuery("from Node n where n.Id in (:ids) and n.Parent is null and size(n.Children) > 0");
						query.SetParameterList("ids", ids);
						IList<Node> nodes = query.List<Node>();
						//Node node = criteria.List<Node>()[0];
						stopwatch.Stop();
						Console.WriteLine("Loading:   {0,4} ms", stopwatch.ElapsedMilliseconds);

						foreach (Node node in nodes)
						{
							stopwatch.Reset();
							stopwatch.Start();
							int total = 0;
							Loop(node, 0, delegate { total++; });
							stopwatch.Stop();
							Console.WriteLine("{0} records (hierarchical)", total);
							Console.WriteLine("Looping:   {0,4} ms", stopwatch.ElapsedMilliseconds);
						}

						transaction.Commit();
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
			//Console.WriteLine("{0}{1}...: {2}", new string(' ', level), node.Id.ToString().Substring(0, 8), node.Name);
			foreach (Node child in node.Children)
			{
				Loop(child, level + 2, action);
			}
		}
	}
}