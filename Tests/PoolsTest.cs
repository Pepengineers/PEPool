using System.Collections.Generic;
using NUnit.Framework;
using PEPEngineers.PEPools.Runtime;

namespace PEPEngineers.PEPools.Tests
{
	internal sealed class PoolsTest
	{
		private StringFactory factory;
		private Pool<string> pool;

		[SetUp]
		public void SetUp()
		{
			factory = new StringFactory();
			pool = new Pool<string>(factory, factory);
		}

		[TearDown]
		public void Teardown()
		{
			pool.Clear();
		}

		[Test]
		public void CreatePool()
		{
			Assert.NotNull(pool);
		}

		[Test]
		public void CheckRented()
		{
			for (var i = 0; i < pool.BucketSize; i++) pool.Get();

			Assert.NotNull(pool);
			Assert.AreEqual(pool.BucketSize, factory.RentedCount);
			Assert.AreEqual(pool.BucketSize, factory.CreatedCount);
		}

		[Test]
		public void CheckReleased()
		{
			for (var i = 0; i < pool.BucketSize; i++) pool.Release(pool.Get());

			Assert.NotNull(pool);
			Assert.AreEqual(pool.BucketSize, factory.RentedCount);
			Assert.AreEqual(pool.BucketSize, factory.CreatedCount);
			Assert.AreEqual(pool.BucketSize, factory.ReleasedCount);
		}

		[Test]
		public void CheckClear()
		{
			pool.Release(pool.Get());
			pool.Clear();

			Assert.NotNull(pool);
			Assert.AreEqual(pool.BucketSize, factory.CreatedCount);
			Assert.AreEqual(pool.BucketSize, factory.DestroyedCount);
		}


		[Test]
		public void CheckDeletedWhenIncreasedMaxSize()
		{
			var list = new List<string>();
			for (var i = 0; i < pool.MaxCount * 2; i++) list.Add(pool.Get());

			foreach (var str in list) pool.Release(str);

			Assert.NotNull(pool);
			Assert.AreEqual(pool.MaxCount * 2, factory.RentedCount);
			Assert.AreEqual(pool.MaxCount * 2, factory.CreatedCount);
			Assert.AreEqual(pool.MaxCount * 2, factory.ReleasedCount);
			Assert.AreEqual(pool.MaxCount, factory.DestroyedCount);
		}
	}
}