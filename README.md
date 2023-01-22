# PEPools
[![Unity 2019.4+](https://img.shields.io/badge/unity-2019.4%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/Pepengineers/PEPools/blob/master/LICENSE)

Unity Pool System


This package adds several kinds of basic classes for working with objects in Unity.
This will save computer resources and increase performance by not creating and deleting objects too often.


## System Requirements
Unity **2019.4** or later versions. Don't forget to include the PEPools namespace and add assemly defenition reference. 

## Installation
You can also install via git url by adding this entry in your **manifest.json**
```
"com.pepools": "https://github.com/Pepengineers/PEPools",
```

# Overview

## SmartPool

General class for pooling.
It takes an `IPoolCallback` reference as input, which allows to remove the delegate call overhead

```csharp
internal sealed class MyBehaviour : MonoBehaviour, IPoolCallback<string>, IPoolFactory<string>
{
	private IPool<string> pool;

	private void Awake()
	{
		pool = new SmartPool<string>(this, this);
	}

	void IPoolCallback<string>.OnItemCreated(in string item){ }
	void IPoolCallback<string>.OnItemRented(in string item) { }
	void IPoolCallback<string>.OnItemReleased(in string item) { }
	void IPoolCallback<string>.OnItemDestroyed(in string item) { }
	string IPoolFactory<string>.Create()
	{
		return Guid.NewGuid().ToString();
	}
}
```
## SimplePool

The class implements callbacks for every event called in the pool. 
Useful if there is no need to override default object creation behavior and not all events are needed. 
`Action` is used, which negates all the bonuses we got in SmartPool, but increases usability 

```csharp
internal sealed class MyBehaviour : MonoBehaviour
	{
		private SimplePool<string> pool;

		private void Awake()
		{
			pool = new SimplePool<string>(Create);
			pool.ItemRented += OnItemRented;
		}

		void OnItemRented(string item)
		{
			Debug.Log($"Rented string {item}");
		}
		string Create()
		{
			return Guid.NewGuid().ToString();
		}
	}
```

## PrefabPool
The class implements the work of creating and removing prefabs.
Useful when using objects that inherit from `Unity.Object` 

```csharp
internal sealed class MyBehaviour : MonoBehaviour
	{
		[SerializeField] private AudioSource prefab;
		private PrefabPool<AudioSource> pool;

		private void Awake()
		{
			pool = new PrefabPool<AudioSource>(prefab);
		}

		private void Update()
		{
			if (Input.anyKeyDown)
			{
				var source = pool.Get();
				StartCoroutine(PlayAndReturnToPool(source));
			}
		}

		private IEnumerator PlayAndReturnToPool(AudioSource source)
		{
			source.Play();
			yield return new WaitForSeconds(source.clip.length);
			pool.Release(source);
		}
		
	}
```

## Editor

All pools support serialization to display debug fields in the editor. 
To do this, you must declare a class inherited from the pool class, specifying the desired type and specifying the `Serializable` attribute


```csharp
internal sealed class MyBehaviour : MonoBehaviour
	{
		[Serializable]
		private class AudioPool : PrefabPool<AudioSource>
		{
			public AudioPool(AudioSource prefab) : base(prefab)
			{
			}
		}
		
		[SerializeField] private AudioSource prefab;
		[SerializeField] private AudioPool pool;

		private void Awake()
		{
			pool = new AudioPool(prefab);
		}

		private void Update()
		{
			if (Input.anyKeyDown)
			{
				var source = pool.Get();
				StartCoroutine(PlayAndReturnToPool(source));
			}
		}

		private IEnumerator PlayAndReturnToPool(AudioSource source)
		{
			source.Play();
			yield return new WaitForSeconds(source.clip.length);
			pool.Release(source);
		}
	}
```
