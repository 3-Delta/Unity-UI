using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class EnumOptionTree : FuzzyOptionTree
	{
		public EnumOptionTree(Type enumType) : base(new GUIContent(enumType.HumanName()))
		{
			Ensure.That(nameof(enumType)).IsNotNull(enumType);

			enums = Enum.GetValues(enumType).Cast<Enum>().ToList();
		}

		public static EnumOptionTree For<T>()
		{
			return new EnumOptionTree(typeof(T));
		}

		private readonly List<Enum> enums;

		public override IEnumerable<IFuzzyOption> Root()
		{
			return enums.Select(x => new EnumOption(x));
		}
	}
}