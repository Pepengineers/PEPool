namespace PEPEngineers.PEPools.Interfaces
{
	public interface IPoolCallback<T>
	{
		void OnItemCreated(in T item);
		void OnItemRented(in T item);
		void OnItemReleased(in T item);
		void OnItemDestroyed(in T item);
	}
}