using Castle.Core.Interceptor;
using Castle.Windsor;

namespace Example
{
	public class SecurityInterceptor : StandardInterceptor
	{
		private readonly IWindsorContainer container;

		public SecurityInterceptor(IWindsorContainer container)
		{
			this.container = container;
		}

		protected override void PreProceed(IInvocation invocation)
		{
			Person person = container.Resolve<Person>();
			if (person.UserName != "richard")
			{
				string message = string.Format("No permission for '{0}'.", person.UserName);
				throw new SecurityException(message);
			}
		}

		protected override void PostProceed(IInvocation invocation)
		{
		}
	}
}