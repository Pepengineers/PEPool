namespace PEPEngineers.PEPools.Interfaces
{
	public interface IPool
	{
		int FreeCount { get; }
		int RentedCount { get; }
		int MaxCount { get; }
		int BucketSize { get; }		
		void Clear();		
	}
	
	public interface IPool<T> : IPool where T : class
	{
		T Get();
		void Release(T element);
	}
}