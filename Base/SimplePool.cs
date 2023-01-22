using System;
using PEPools.Interfaces;
using PEPools.Settings;

namespace PEPools.Base
{
	public class SimplePool<TItem> : IPool<TItem>, IPoolCallback<TItem>, IPoolFactory<TItem> where TItem : class
	{
		private readonly Func<TItem> createFactory;
		private readonly IPool<TItem> pool;

		public SimplePool(Func<TItem> factory, int defaultCapacity = PoolConstants.DefaultBucketSize,
			int bucketSize = PoolConstants.DefaultBucketSize,
			int maxSize = PoolConstants.DefaultMaxSize)
		{
			createFactory = factory;
			pool = new SmartPool<TItem>(this, this);
		}

		public Action<TItem> ItemCreated { get; } = null;
		public Action<TItem> ItemRented { get; } = null;
		public Action<TItem> ItemReleased { get; } = null;
		public Action<TItem> ItemDestroyed { get; } = null;

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

		void IPoolCallback<TItem>.OnItemCreated(in TItem item)
		{
			ItemCreated?.Invoke(item);
		}

		void IPoolCallback<TItem>.OnItemRented(in TItem item)
		{
			ItemRented?.Invoke(item);
		}

		void IPoolCallback<TItem>.OnItemReleased(in TItem item)
		{
			ItemReleased?.Invoke(item);
		}

		void IPoolCallback<TItem>.OnItemDestroyed(in TItem item)
		{
			ItemDestroyed?.Invoke(item);
		}

		TItem IPoolFactory<TItem>.Create()
		{
			return createFactory();
		}
	}
}