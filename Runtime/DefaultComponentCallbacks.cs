using PEPEngineers.PEPools.Interfaces;
using UnityEngine;

namespace PEPEngineers.PEPools.Runtime
{
	public class DefaultComponentCallbacks<TComponent> : IPoolCallback<TComponent> where TComponent : Behaviour
	{
		public DefaultComponentCallbacks(Transform parent)
		{
			Parent = parent;
		}

		protected Transform Parent { get; }

		public virtual void OnItemCreated(in TComponent item)
		{
			ResetItem(item);
		}

		public virtual void OnItemRented(in TComponent item)
		{
			item.enabled = true;
			var transform = item.transform;
			transform.SetParent(null);
			transform.gameObject.SetActive(true);
		}

		public virtual void OnItemReleased(in TComponent item)
		{
			ResetItem(item);
		}

		public virtual void OnItemDestroyed(in TComponent item)
		{
		}

		private void ResetItem(TComponent item)
		{
			item.enabled = false;
			var transform = item.transform;
			transform.SetParent(Parent);
			transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			item.gameObject.SetActive(false);
		}
	}
}