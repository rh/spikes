using System;

namespace Example
{
	public class SecurityException : Exception
	{
		public SecurityException(string message)
			: base(message)
		{
		}
	}
}