using PEPEngineers.PEPools.Interfaces;
using UnityEngine;

namespace PEPEngineers.PEPools.Runtime
{
	public class DefaultComponentPoolCallbacks<TComponent> : IPoolCallback<TComponent> where TComponent : Behaviour
	{
		protected Transform Parent { get; }

		public DefaultComponentPoolCallbacks(Transform parent)
		{
			Parent = parent;
		}
		public virtual void OnItemCreated(in TComponent item)
		{
			ResetItem(item);
		}

		private void ResetItem(TComponent item)
		{
			item.enabled = false;
			var transform = item.transform;
			transform.SetParent(Parent);
			transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			item.gameObject.SetActive(false);
		}

		public virtual void OnItemRented(in TComponent item)
		{
			item.enabled = true;
			var transform = item.transform;
			transform.SetParent(null);
			transform.gameObject.SetActive(true);
		}

		public void OnItemReleased(in TComponent item)
		{
			ResetItem(item);
		}

		public void OnItemDestroyed(in TComponent item)
		{
                
		}
	}
}