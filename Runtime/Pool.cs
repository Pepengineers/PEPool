using System;
using System.Collections.Generic;
using PEPEngineers.PEPools.Interfaces;
using PEPEngineers.PEPools.Extensions;
using PEPEngineers.PEPools.Settings;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace PEPEngineers.PEPools.Runtime
{
	[Serializable]
	public class Pool<T> : IPool<T> where T : class
	{
		private readonly IPoolCallback<T> callbacks;
		private readonly IPoolFactory<T> factory;
		private int countAll;


		public Pool(IPoolFactory<T> factory, IPoolCallback<T> callbacks = null,
			int defaultCapacity = PoolConstants.DefaultBucketSize,
			int bucketSize = PoolConstants.DefaultBucketSize,
			int maxSize = PoolConstants.DefaultMaxSize)
		{
#if UNITY_EDITOR
			Assert.IsFalse(maxSize <= 0);
			Assert.IsNotNull(factory);
#endif
			Items = new List<T>(defaultCapacity);
			this.factory = factory;
			this.callbacks = callbacks;
			MaxCount = maxSize;
			BucketSize = bucketSize;
		}

#if ODIN_INSPECTOR
		[ReadOnly]
		[ShowInInspector] 
#endif
		private List<T> Items { get; }

#if ODIN_INSPECTOR
		[ReadOnly]
		[ShowInInspector]
#endif
		public int RentedCount => countAll - FreeCount;

#if ODIN_INSPECTOR
		[ReadOnly]
		[ShowInInspector]
#endif
		public int MaxCount { get; private set; }

#if ODIN_INSPECTOR
		[ReadOnly]
		[ShowInInspector]
#endif
		public int BucketSize { get; private set; }

#if ODIN_INSPECTOR
		[ReadOnly]
		[ShowInInspector]
#endif
		public int FreeCount => Items.Count;

		public T Get()
		{
			if (FreeCount == 0) CreateBucket();

			var item = Items[0];
			callbacks?.OnItemRented(item);
			Items.FastRemove(0);

			return item;
		}

		public void Release(T element)
		{
#if UNITY_EDITOR
			if (Items.Count > 0)
				if (Items.Contains(element))
					throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
#endif

			callbacks?.OnItemReleased(element);
			if (FreeCount >= MaxCount)
			{
				callbacks?.OnItemDestroyed(element);
				factory.Destroy(element);
			}
			else
			{
				Items.Add(element);
			}
		}

		public void Clear()
		{
			var count = Items.Count;
			while (count-- > 0)
			{
				callbacks?.OnItemDestroyed(Items[count]);
				factory.Destroy(Items[count]);
			}

			Items.Clear();
			countAll = 0;
		}

		private void CreateBucket()
		{
			for (var i = 0; i < BucketSize; i++)
			{
				var item = factory.Create();
				callbacks?.OnItemCreated(item);
				Items.Add(item);
			}

			countAll += BucketSize;
		}
	}
}