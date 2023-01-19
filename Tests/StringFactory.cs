using System;
using PEPools.Interfaces;

namespace PEPools.Tests
{
	internal class StringFactory : IPoolFactory<string>, IPoolCallback<string>
	{
		public int CreatedCount;
		public int DestroyedCount;
		public int ReleasedCount;
		public int RentedCount;

		public void OnItemCreated(in string item)
		{
			CreatedCount++;
		}

		public void OnItemRented(in string item)
		{
			RentedCount++;
		}

		public void OnItemReleased(in string item)
		{
			ReleasedCount++;
		}

		public void OnItemDestroyed(in string item)
		{
			DestroyedCount++;
		}

		public string Create()
		{
			return new string(Guid.NewGuid().ToString());
		}
	}
}