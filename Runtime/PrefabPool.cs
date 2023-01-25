using System;
using PEPools.Attributes;
using PEPools.Interfaces;
using PEPools.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PEPools.Runtime
{
	[Serializable]
	public class PrefabPool<TItem> : IPool<TItem> where TItem : Behaviour
	{
		[SerializeField] [ReadOnly] private PrefabFactory factory;
		[SerializeField] [ReadOnly] private ItemPool pool;

		public PrefabPool(TItem prefab, IPoolCallback<TItem> callbackProvider = null,
			int initialCapacity = PoolConstants.DefaultBucketSize,
			int initialMaxSize = PoolConstants.DefaultMaxSize)
		{
			factory = new PrefabFactory(prefab, callbackProvider);
			pool = new ItemPool(factory, factory, initialCapacity, maxSize: initialMaxSize);
		}

		public int FreeCount => pool.FreeCount;
		public int RentedCount => pool.RentedCount;
		public int MaxCount => pool.MaxCount;
		public int BucketSize => pool.BucketSize;

		public TItem Get()
		{
			return pool.Get();
		}

		public void Release(TItem element)
		{
			pool.Release(element);
		}

		public void Clear()
		{
			pool.Clear();
		}

		[Serializable]
		private sealed class PrefabFactory : IPoolFactory<TItem>, IPoolCallback<TItem>
		{
			[SerializeField] [ReadOnly] private TItem prefab;
			[SerializeField] [ReadOnly] private uint count = uint.MinValue;
			private readonly IPoolCallback<TItem> callbackProvider;

			public PrefabFactory(TItem prefab, IPoolCallback<TItem> callbackProvider)
			{
				this.prefab = prefab;
				this.callbackProvider = callbackProvider;
			}

			void IPoolCallback<TItem>.OnItemDestroyed(in TItem item)
			{
				callbackProvider?.OnItemDestroyed(item);
				Object.Destroy(item.gameObject);
			}

			void IPoolCallback<TItem>.OnItemCreated(in TItem item)
			{
				callbackProvider?.OnItemCreated(item);
			}

			void IPoolCallback<TItem>.OnItemRented(in TItem item)
			{
				callbackProvider?.OnItemRented(item);
			}

			void IPoolCallback<TItem>.OnItemReleased(in TItem item)
			{
				callbackProvider?.OnItemReleased(item);
			}

			TItem IPoolFactory<TItem>.Create()
			{
				var item = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				item.name = $"{prefab.name} {++count}";
				return item;
			}
		}

		[Serializable]
		internal sealed class ItemPool : SmartPool<TItem>
		{
			public ItemPool(IPoolFactory<TItem> factory, IPoolCallback<TItem> provider,
				int defaultCapacity = PoolConstants.DefaultBucketSize,
				int bucketSize = PoolConstants.DefaultBucketSize, int maxSize = PoolConstants.DefaultMaxSize) : base(
				factory, provider, defaultCapacity,
				bucketSize, maxSize)
			{
			}
		}
	}
}