using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PEPools.Extensions
{
	internal static class CollectionUtilities
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SmartRemove<T>(this IList<T> list, int index)
		{
			if (index < 0) return;

			var lastIndex = list.Count - 1;
			list[index] = list[lastIndex];
			list.RemoveAt(lastIndex);
		}
	}
}