namespace PEPools.Interfaces
{
	public interface IPool<T> : IReadOnlyPool<T> where T : class
	{
		void Clear();
	}

	public interface IReadOnlyPool<T> where T : class
	{
		int FreeCount { get; }

		int RentedCount { get; }

		int MaxCount { get; }

		int BucketSize { get; }

		T Get();

		void Release(T element);
	}
}