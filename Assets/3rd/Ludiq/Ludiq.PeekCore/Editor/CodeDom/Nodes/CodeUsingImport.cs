namespace Ludiq.PeekCore.CodeDom
{
	public struct CodeUsingImport
	{
		public CodeUsingImport(string name)
		{
			Name = name;
		}

		public string Name;

		public void Generate(CodeGenerator generator)
        {
			if (Name != null)
			{
				generator.Write(TokenType.Keyword, "using");
				generator.Write(TokenType.Space, ' ');
				generator.OutputQualifiedName(TokenType.Identifier, Name);
				generator.WriteLine(TokenType.Punctuation, ';');
			}
        }

		public override int GetHashCode() => Name != null ? Name.GetHashCode() : 0;

		public override bool Equals(object other)
		{
			if (other is CodeUsingImport otherUsingDirective)
			{
				return this == otherUsingDirective;
			}
			return false;
		}

		public static bool operator ==(CodeUsingImport a, CodeUsingImport b) => a.Name == b.Name;
		public static bool operator !=(CodeUsingImport a, CodeUsingImport b) => !(a == b);
	}
}