using System;
using System.Collections.Generic;
using PEPools.Attributes;
using PEPools.Extensions;
using PEPools.Interfaces;
using PEPools.Settings;
using UnityEngine;

namespace PEPools.Base
{
	[Serializable]
	public class SmartPool<T> : IPool<T> where T : class
	{
		[SerializeField] [ReadOnly] private int countAll;
		[SerializeField] [ReadOnly] private int bucketSize;
		[SerializeField] [ReadOnly] private List<T> container;
		[SerializeField] [ReadOnly] private int maxSize;
		private readonly IPoolCallback<T> callbacks;
		private readonly IPoolFactory<T> factory;

		public SmartPool(IPoolFactory<T> factory, IPoolCallback<T> callbacks = null,
			int defaultCapacity = PoolConstants.DefaultBucketSize,
			int bucketSize = PoolConstants.DefaultBucketSize,
			int maxSize = PoolConstants.DefaultMaxSize)
		{
			if (maxSize <= 0)
				throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));
			container = new List<T>(defaultCapacity);
			this.factory = factory;
			this.callbacks = callbacks;
			this.maxSize = maxSize;
			this.bucketSize = bucketSize;
		}

		public int RentedCount => countAll - FreeCount;
		public int MaxCount => maxSize;
		public int BucketSize => bucketSize;
		public int FreeCount => container.Count;

		public T Get()
		{
			if (FreeCount == 0) CreateBucket();

			var item = container[0];
			callbacks?.OnItemRented(item);
			container.FastRemove(0);
			return item;
		}

		public void Release(T element)
		{
			callbacks?.OnItemReleased(element);
			if (FreeCount >= maxSize)
				callbacks?.OnItemDestroyed(element);
			else
				container.Add(element);
		}

		public void Clear()
		{
			var count = container.Count;
			for (var i = count - 1; i >= 0; i++) 
				callbacks?.OnItemDestroyed(container[i]);

			container.Clear();
			countAll = 0;
		}

		private void CreateBucket()
		{
			for (var i = 0; i < bucketSize; i++)
			{
				var item = factory.Create();
				callbacks?.OnItemCreated(item);
				container.Add(item);
			}

			countAll += bucketSize;
		}
	}
}