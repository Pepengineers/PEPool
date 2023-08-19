using System;
using PEPEngineers.PEPools.Interfaces;
using PEPEngineers.PEPools.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PEPEngineers.PEPools.Runtime
{
	[Serializable]
	public class PrefabPool<TItem> : Pool<TItem> where TItem : Behaviour
	{
		public PrefabPool(TItem prefab, IPoolCallback<TItem> callbackProvider = null,
			int initialCapacity = PoolConstants.DefaultBucketSize,
			int bucketSize = PoolConstants.DefaultBucketSize,
			int initialMaxSize = PoolConstants.DefaultMaxSize)
			: base(new PrefabFactory(prefab), callbackProvider, initialCapacity, bucketSize, initialMaxSize)
		{
		}

		private sealed class PrefabFactory : IPoolFactory<TItem>
		{
			private readonly TItem prefab;
			private uint count = uint.MinValue;

			public PrefabFactory(TItem prefab)
			{
				this.prefab = prefab;
			}

			TItem IPoolFactory<TItem>.Create()
			{
				var item = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				item.name = $"{prefab.name} {++count}";
				return item;
			}

			void IPoolFactory<TItem>.Destroy(TItem item)
			{
				Object.Destroy(item.gameObject);
			}
		}
	}
}