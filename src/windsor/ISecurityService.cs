namespace Example
{
	public interface ISecurityService
	{
		bool IsAllowed { get; }
	}

	public class SecureSecurityService : ISecurityService
	{
		public bool IsAllowed
		{
			get { return false; }
		}
	}

	public class InsecureSecurityService : ISecurityService
	{
		public bool IsAllowed
		{
			get { return true; }
		}
	}
}