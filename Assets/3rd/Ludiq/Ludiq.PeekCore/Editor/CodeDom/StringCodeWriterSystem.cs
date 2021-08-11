using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class StringCodeWriterSystem : CodeWriterSystem<TextCodeWriter>
	{
		private readonly Dictionary<string, StringBuilder> builders = new Dictionary<string, StringBuilder>();

		public override TextCodeWriter OpenWriter(string className)
		{
			var builder = new StringBuilder();
			builders.Add(className, builder);
			return new TextCodeWriter(new StringWriter(builder));
		}
		
		public string GetString(string className)
		{
			return builders[className].ToString();
		}
	}
}
