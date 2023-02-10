namespace PEPEngineers.PEPools.Interfaces
{
	public interface IPoolFactory<T>
	{
		T Create();

		void Destroy(T item);
	}
}