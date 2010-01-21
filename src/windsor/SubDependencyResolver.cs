using Castle.Core;
using Castle.MicroKernel;

namespace Example
{
	public class SubDependencyResolver : ISubDependencyResolver
	{
		// The kernel (an implementation of IWindsorContainer would have been fine too)
		// is needed to resolve the 'currently logged-in user'
		private readonly IKernel kernel;

		public SubDependencyResolver(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return dependency.TargetType == typeof(ISecurityService);
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			// This person is the 'currently logged-in user', i.e. the context needed to resolve which implementation
			// of ISecurityService needs to be returned
			Person person = kernel.Resolve<Person>();
			if (person.UserName == "richard")
			{
				return new InsecureSecurityService();
			}
			return new SecureSecurityService();
		}
	}
}