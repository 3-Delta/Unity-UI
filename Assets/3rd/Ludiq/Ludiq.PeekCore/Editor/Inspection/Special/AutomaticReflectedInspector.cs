namespace Ludiq.PeekCore
{
	public sealed class AutomaticReflectedInspector : ReflectedEditor
	{
		public AutomaticReflectedInspector(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			accessor.instantiate = true;

			base.Initialize();
		}

		protected override bool showTitle => false;

		protected override bool showSummary => false;

		protected override bool showIcon => false;
	}
}