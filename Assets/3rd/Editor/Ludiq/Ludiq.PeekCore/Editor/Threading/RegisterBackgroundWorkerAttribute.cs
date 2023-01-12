using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterBackgroundWorkerAttribute : Attribute, ITypeRegistrationAttribute
	{
		public RegisterBackgroundWorkerAttribute(Type type, string methodName = "BackgroundWork")
		{
			this.type = type;
			this.methodName = methodName;
		}

		public Type type { get; }
		public string methodName { get; }
	}
}