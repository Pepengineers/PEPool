using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PEPEngineers.PEPools.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace PEPEngineers.PEPools.Extensions
{
	public static class PoolExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FastRemove<T>(this IList<T> list, int index)
		{
			Assert.IsFalse(index < 0, "index out of range < 0");
			var lastIndex = list.Count - 1;
			Assert.IsFalse(index > lastIndex, "index out of range >= list.Count");
			list[index] = list[lastIndex];
			list.RemoveAt(lastIndex);
		}

		public static IEnumerator ReturnToPoolRoutine<TComponent>(this TComponent component, IPool<TComponent> pool,
			float delay) where TComponent : Behaviour
		{
			yield return new WaitForSeconds(delay);
			pool.Release(component);
		}

		public static IEnumerator ReturnToPoolRoutine<TComponent>(this IPool<TComponent> pool, TComponent component,
			float delay) where TComponent : Behaviour
		{
			return ReturnToPoolRoutine(component, pool, delay);
		}

		public static void ReturnToPool<TComponent>(this TComponent component, IPool<TComponent> pool, float delay)
			where TComponent : MonoBehaviour
		{
			component.StartCoroutine(ReturnToPoolRoutine(component, pool, delay));
		}

		public static void ReturnToPool<TComponent>(this IPool<TComponent> pool, TComponent component,
			float delay) where TComponent : MonoBehaviour
		{
			component.StartCoroutine(ReturnToPoolRoutine(component, pool, delay));
		}

		public static void ReturnToPool<TComponent>(this MonoBehaviour behaviour, TComponent component,
			IPool<TComponent> pool, float delay)
			where TComponent : MonoBehaviour
		{
			behaviour.StartCoroutine(ReturnToPoolRoutine(component, pool, delay));
		}
	}
}
