using Castle.Core;
using Castle.MicroKernel.Proxy;

namespace Example
{
	// This class decides that any Service will be intercepted by a LoggingInterceptor and a SecurityInterceptor
	public class ModelInterceptorsSelector : IModelInterceptorsSelector
	{
		public bool HasInterceptors(ComponentModel model)
		{
			return model.Service == typeof(Service);
		}

		public InterceptorReference[] SelectInterceptors(ComponentModel model)
		{
			return new InterceptorReference[]
				{
					InterceptorReference.ForType<LoggingInterceptor>(),
					InterceptorReference.ForType<SecurityInterceptor>()
				};
		}
	}
}