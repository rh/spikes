using System;

namespace Example
{
	public class Service
	{
		// Which implementation of ISecurityService will be injected is decided by SubDependencyResolver
		private readonly ISecurityService security;

		public Service(ISecurityService security)
		{
			this.security = security;
		}

		/// <remarks>This method will only be intercepted if it's virtual.</remarks>
		public virtual void Do()
		{
			if (security.IsAllowed)
			{
				Console.WriteLine("FooService.Do: '{0}' says OK...", security.GetType().Name);
			}
			else
			{
				Console.WriteLine("FooService.Do: '{0}' says no...", security.GetType().Name);
			}
		}
	}
}