using System;
using Castle.Core;
using Castle.Windsor;

namespace Example
{
	internal class Program
	{
		private static void Main()
		{
			IWindsorContainer container = new WindsorContainer();
			// The container is needed by SecurityInterceptor
			container.Kernel.AddComponentInstance("container", typeof(IWindsorContainer), container);

			// SubDependencyResolver decides which implementation of ISecurityService should be returned,
			// but both implementations have to be registered
			container.Kernel.Resolver.AddSubResolver(new SubDependencyResolver(container.Kernel));
			container.AddComponent<ISecurityService, SecureSecurityService>("security1");
			container.AddComponent<ISecurityService, InsecureSecurityService>("security2");

			// ModelInterceptorsSelector selects the interceptors to be added, but these have to be registered
			container.Kernel.ProxyFactory.AddInterceptorSelector(new ModelInterceptorsSelector());
			container.AddComponent("logging.interceptor", typeof(LoggingInterceptor));
			container.AddComponent("security.interceptor", typeof(SecurityInterceptor));

			// Every resolved instance of Person will be the same. It represents the 'currently logged-in user'
			container.AddComponentLifeStyle("user", typeof(Person), LifestyleType.Singleton);
			// This service needs to be transient or the SubDependencyResolver will only be called once
			container.AddComponentLifeStyle("foo", typeof(Service), LifestyleType.Transient);

			Person person = container.Resolve<Person>();

			person.UserName = "richard";
			// This FooService will be instantiated with an InsecureSecurityService
			Service service1 = container.Resolve<Service>();
			// Notice the output of LoggingInterceptor and the output of Service itself,
			// which depends upon the implementation of ISecurityService
			service1.Do();

			// Pretend another user has logged in
			person.UserName = "someone_else";
			// This FooService will be instantiated with an SecureSecurityService
			Service service2 = container.Resolve<Service>();

			try
			{
				// SecurityInterceptor will throw here
				service2.Do();
			}
			catch (SecurityException e)
			{
				Console.WriteLine(e.Message);
			}

			Console.WriteLine();
			Console.WriteLine("Press [Enter] to continue...");
			Console.ReadLine();
		}
	}
}