using System;
using Castle.Core.Interceptor;

namespace Example
{
	public class LoggingInterceptor : StandardInterceptor
	{
		protected override void PreProceed(IInvocation invocation)
		{
			Console.WriteLine("Before: {0}.{1}", invocation.TargetType.Name, invocation.Method.Name);
		}

		protected override void PostProceed(IInvocation invocation)
		{
			Console.WriteLine("After:  {0}", invocation.Method.Name);
		}
	}
}