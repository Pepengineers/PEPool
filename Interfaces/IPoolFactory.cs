namespace PEPEngineers.PEPools.Interfaces
{
	public interface IPoolFactory<out T>
	{
		T Create();
	}
}