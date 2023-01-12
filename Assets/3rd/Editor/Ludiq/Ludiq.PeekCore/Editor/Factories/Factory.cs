using System;

namespace Ludiq.PeekCore
{
	public abstract class Factory<T, TConfiguration> : IFactory where TConfiguration : IFactoryConfiguration
	{
		public Type type { get; }

		protected Factory(Type type)
		{
			this.type = type;
		}

		public abstract bool requiresConfiguration { get; }

		public abstract T Create(TConfiguration configuration);

		object IFactory.Create(IFactoryConfiguration configuration) => Create(configuration.CastTo<TConfiguration>());

		public virtual string label => typeof(T).DisplayName();

		public virtual string description => null;

		public virtual EditorTexture icon => typeof(T).Icon();
	}
}
