namespace PEPEngineers.PEPools.Interfaces
{
	public interface IPool<T> where T : class
	{
		int FreeCount { get; }
		int RentedCount { get; }
		int MaxCount { get; }
		int BucketSize { get; }

		T Get();
		void Release(T element);
		void Clear();
	}
}