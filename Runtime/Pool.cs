using System;
using System.Collections.Generic;
using PEPEngineers.PEPools.Attributes;
using PEPEngineers.PEPools.Extensions;
using PEPEngineers.PEPools.Interfaces;
using PEPEngineers.PEPools.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace PEPEngineers.PEPools.Runtime
{
	[Serializable]
	public class Pool<T> : IPool<T> where T : class
	{
		[SerializeField] [ReadOnly] private int countAll;
		[SerializeField] [ReadOnly] private int bucketSize;
		[SerializeReference] [ReadOnly] private List<T> freeList;
		[SerializeField] [ReadOnly] private int maxSize;
		private readonly IPoolCallback<T> callbacks;
		private readonly IPoolFactory<T> factory;

		public Pool(IPoolFactory<T> factory, IPoolCallback<T> callbacks = null,
			int defaultCapacity = PoolConstants.DefaultBucketSize,
			int bucketSize = PoolConstants.DefaultBucketSize,
			int maxSize = PoolConstants.DefaultMaxSize)
		{
			Assert.IsFalse(maxSize <= 0);
			freeList = new List<T>(defaultCapacity);
			this.factory = factory;
			this.callbacks = callbacks;
			this.maxSize = maxSize;
			this.bucketSize = bucketSize;
		}

		public int RentedCount => countAll - FreeCount;
		public int MaxCount => maxSize;
		public int BucketSize => bucketSize;
		public int FreeCount => freeList.Count;

		public T Get()
		{
			if (FreeCount == 0) CreateBucket();

			var item = freeList[0];
			callbacks?.OnItemRented(item);
			freeList.FastRemove(0);
			return item;
		}

		public void Release(T element)
		{
			callbacks?.OnItemReleased(element);
			if (FreeCount >= maxSize)
			{
				callbacks?.OnItemDestroyed(element);
				factory.Destroy(element);
			}
			else
			{
				freeList.Add(element);
			}
		}

		public void Clear()
		{
			var count = freeList.Count;
			for (var i = count - 1; i >= 0; i--)
				callbacks?.OnItemDestroyed(freeList[i]);

			freeList.Clear();
			countAll = 0;
		}

		private void CreateBucket()
		{
			for (var i = 0; i < bucketSize; i++)
			{
				var item = factory.Create();
				freeList.Add(item);
				callbacks?.OnItemCreated(item);
			}

			countAll += bucketSize;
		}
	}
}