using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public class DropdownOption : IDropdownOption
	{
		public DropdownOption(object value)
		{
			this.value = value;
			label = value != null ? value.ToString() : "(null)";
		}

		public DropdownOption(object value, string label)
		{
			this.value = value;
			this.label = label;
		}

		public DropdownOption(object value, string label, bool on)
		{
			this.value = value;
			this.label = label;
			this.on = on;
		}

		public string label { get; set; }
		
		public object value { get; set; }

		public bool? on { get; set; }

		private static IEnumerable<DropdownOption> GetEnumOptions<T>(bool nicify)
		{
			foreach (var enumValue in Enum.GetValues(typeof(T)).Cast<T>())
			{
				yield return new DropdownOption(enumValue, nicify ? ObjectNames.NicifyVariableName(enumValue.ToString()) : enumValue.ToString());
			}
		}

		public static IEnumerable<DropdownOption> GetEnumOptions<T>()
		{
			return GetEnumOptions<T>(false);
		}

		public static IEnumerable<DropdownOption> GetEnumOptionsNicified<T>()
		{
			return GetEnumOptions<T>(true);
		}
	}
}