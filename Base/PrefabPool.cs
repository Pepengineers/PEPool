using System;
using PEPools.Attributes;
using PEPools.Interfaces;
using PEPools.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PEPools.Base
{
	[Serializable]
	public class PrefabPool<TItem> : IPool<TItem> where TItem : Behaviour
	{
		[SerializeField] [ReadOnly] private PrefabFactory factory;
		[SerializeField] [ReadOnly] private ItemPool pool;

		public PrefabPool(TItem prefab, Transform parent = null, IPoolCallback<TItem> callbackProvider = null,
			int initialCapacity = PoolConstants.DefaultBucketSize,
			int initialMaxSize = PoolConstants.DefaultMaxSize)
		{
			factory = new PrefabFactory(prefab, callbackProvider, parent);
			pool = new ItemPool(factory, factory, initialCapacity, maxSize: initialMaxSize);
		}

		public ref readonly Transform Parent => ref factory.Parent;
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
			[SerializeField] [ReadOnly] private Transform parent;
			[SerializeField] [ReadOnly] private TItem prefab;
			[SerializeField] [ReadOnly] private uint count = uint.MinValue;
			private readonly IPoolCallback<TItem> callbackProvider;

			public PrefabFactory(TItem prefab, IPoolCallback<TItem> callbackProvider, Transform parent)
			{
				this.prefab = prefab;
				this.callbackProvider = callbackProvider;
				this.parent = parent;
			}

			public ref readonly Transform Parent => ref parent;

			void IPoolCallback<TItem>.OnItemDestroyed(in TItem item)
			{
				callbackProvider?.OnItemDestroyed(item);
				Object.Destroy(item);
			}

			void IPoolCallback<TItem>.OnItemCreated(in TItem item)
			{
				item.enabled = false;
				item.transform.SetParent(parent);
				item.gameObject.SetActive(false);
				callbackProvider?.OnItemCreated(item);
			}

			void IPoolCallback<TItem>.OnItemRented(in TItem item)
			{
				item.gameObject.SetActive(true);
				item.enabled = true;
				callbackProvider?.OnItemRented(item);
			}

			void IPoolCallback<TItem>.OnItemReleased(in TItem item)
			{
				item.enabled = false;
				item.transform.SetParent(parent);
				item.gameObject.SetActive(false);
				callbackProvider?.OnItemReleased(item);
			}

			TItem IPoolFactory<TItem>.Create()
			{
				var item = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
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