using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterPluginDependencyAttribute : Attribute
	{
		public RegisterPluginDependencyAttribute(string dependerId, string dependencyId)
		{
			Ensure.That(nameof(dependerId)).IsNotNull(dependerId);
			Ensure.That(nameof(dependencyId)).IsNotNull(dependencyId);

			this.dependencyId = dependencyId;
			this.dependerId = dependerId;
		}
		
		public string dependencyId { get; }
		public string dependerId { get; }
	}
}