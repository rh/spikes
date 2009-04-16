using System;
using System.Collections.Generic;
using System.Diagnostics;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
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
				// properties[Environment.ShowSql] = "true";

				Configuration cfg = new Configuration();
				cfg.Properties = properties;
				cfg.AddAssembly(typeof(Node).Assembly);

				Console.WriteLine();
				new SchemaExport(cfg).Execute(true, true, false, true);
				Console.WriteLine();

				const int count = 4;

				Stopwatch stopwatch = Stopwatch.StartNew();

				ISessionFactory factory = cfg.BuildSessionFactory();
				using (ISession session = factory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						for (int i = 0; i < count; i++)
						{
							Node inode = new Node(i.ToString());
							for (int j = 0; j < count; j++)
							{
								Node jnode = new Node(j.ToString());
								inode.Add(jnode);
								for (int k = 0; k < count; k++)
								{
									Node knode = new Node(k.ToString());
									jnode.Add(knode);
									for (int l = 0; l < count; l++)
									{
										Node lnode = new Node(l.ToString());
										knode.Add(lnode);
									}
								}
							}
							session.Save(inode);
						}
						transaction.Commit();
					}
				}

				stopwatch.Stop();
				Console.WriteLine("{0} records: {1} ms", count * count * count * count, stopwatch.ElapsedMilliseconds);
				Console.WriteLine();

				using (ISession session = factory.OpenSession())
				{
					using (ITransaction transaction = session.BeginTransaction())
					{
						stopwatch.Reset();
						stopwatch.Start();

						ICriteria criteria = session.CreateCriteria(typeof(Node));
						criteria.Add(Expression.IsNull("Parent"));

						// This may actually slow down the total time:
						// loading will increase more than looping will decrease
						criteria.SetFetchMode("Children", FetchMode.Eager);
						criteria.SetFetchMode("Children.Children", FetchMode.Eager);
						criteria.SetFetchMode("Children.Children.Children", FetchMode.Eager);
						criteria.SetFetchMode("Children.Children.Children.Children", FetchMode.Eager);
						Node node = criteria.List<Node>()[0];
						stopwatch.Stop();
						Console.WriteLine("Loading: {0} ms", stopwatch.ElapsedMilliseconds);

						stopwatch.Reset();
						stopwatch.Start();
						Loop(node, 0);
						stopwatch.Stop();
						Console.WriteLine("Looping: {0} ms", stopwatch.ElapsedMilliseconds);

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

		private static void Loop(Node node, int level)
		{
			Console.WriteLine("{0}{1}...: {2}", new string(' ', level), node.Id.ToString().Substring(0, 8), node.Name);
			foreach (Node child in node.Children)
			{
				Loop(child, level + 2);
			}
		}
	}
}