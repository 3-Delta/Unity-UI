using System;
using System.Collections;

namespace Ludiq.PeekCore
{
	public static class ListUtility
	{
		public static IList Add(IList list, object item)
		{
			if (list is Array)
			{
				var resizableList = new ArrayList(list);
				resizableList.Add(item);
				return resizableList.ToArray(list.GetType().GetElementType());
			}
			else
			{
				list.Add(item);
				return list;
			}
		}

		public static IList Insert(IList list, int index, object item)
		{
			if (list is Array)
			{
				var resizableList = new ArrayList(list);
				resizableList.Insert(index, item);
				return resizableList.ToArray(list.GetType().GetElementType());
			}
			else
			{
				list.Insert(index, item);
				return list;
			}
		}

		public static IList Remove(IList list, object item)
		{
			if (list is Array)
			{
				var resizableList = new ArrayList(list);
				resizableList.Remove(item);
				return resizableList.ToArray(list.GetType().GetElementType());
			}
			else
			{
				list.Remove(item);
				return list;
			}
		}

		public static IList RemoveAt(IList list, int index)
		{
			if (list is Array)
			{
				var resizableList = new ArrayList(list);
				resizableList.RemoveAt(index);
				return resizableList.ToArray(list.GetType().GetElementType());
			}
			else
			{
				list.RemoveAt(index);
				return list;
			}
		}

		public static IList Clear(IList list)
		{
			if (list is Array)
			{
				return Array.CreateInstance(list.GetType().GetElementType(), 0);
			}
			else
			{
				list.Clear();
				return list;
			}
		}
	}
}