using System.Collections.Generic;
using UnityEngine;

public class CustomPool<T> where T : MonoBehaviour
{
	private Dictionary<T, bool> _poolObjectStates = new();

	private T _prefab;
	private bool _shouldUseMaxSize = false;
	private int _currentSize = 0;
	private int _maxSize = 0;
	private Transform _parentTransform;

	/// <summary>
	/// Creates a pool of the object to instantiate.
	/// </summary>
	/// <param name="objectToInstantiate"> The prefab of the object to instantiate. Must be a Monobehaviour</param>
	/// <param name="startSize"> Initial start size of pool. </param>
	/// <param name="shouldUseMaxSize"> true if pool has a maximum size and can't spawn more, false if otherwise</param>
	/// <param name="maxSize"> maximum size of pool. Only used if shouldUseMaxSize is true</param>
	/// <param name="parentTransform"> parent transform all the pooled objects should spawn under. Will instantiate one if none provided.</param>
	public CustomPool(T objectToInstantiate, int startSize = 10, bool shouldUseMaxSize = false, int maxSize = 20, Transform parentTransform = null)
	{
		_prefab = objectToInstantiate;
		_currentSize = startSize;

		if (parentTransform == null)
		{
			_parentTransform = new GameObject("CustomPoolParent").transform;
		}
		else
		{
			_parentTransform = parentTransform;
		}

		_shouldUseMaxSize = shouldUseMaxSize;
		if (shouldUseMaxSize)
		{
			if (maxSize < startSize)
			{
				Debug.LogError("Pool Max Size is smaller than the start size. Setting the max size to be the start size");
				_maxSize = startSize;
			}
			else
			{
				_maxSize = maxSize;
			}
		}

		for (int i = 0; i < startSize; i++)
		{
			InstantiateObjectAndAddToPool();
		}
	}

	/// <summary>
	/// Gets an object from the pool. If all objects are already there, tries to instantiate a new item if not using the maximum size,
	/// or if it's still less than the maximum size.
	/// </summary>
	/// <returns> returns the instantiated object, or null if it can't instantiate more due to maximum size.</returns>
	public T GetObject()
	{
		foreach (var poolObjectStatePair in _poolObjectStates)
		{
			if (!poolObjectStatePair.Value)
			{
				poolObjectStatePair.Key.gameObject.SetActive(true);
				_poolObjectStates[poolObjectStatePair.Key] = true;
				return poolObjectStatePair.Key;
			}
		}

		if (_shouldUseMaxSize && _currentSize >= _maxSize)
		{
			Debug.LogError("Spawned maximum amount allowed for pool. Returning null");
			return null;
		}

		++_currentSize;
		return InstantiateObjectAndAddToPool(true);
	}

	/// <summary>
	/// Returns and deactivates the object
	/// </summary>
	/// <param name="objToReturn"> the object to return to the pool.</param>
	public void ReturnObject(T objToReturn)
	{
		objToReturn.gameObject.SetActive(false);
		_poolObjectStates[objToReturn] = false;
	}

	/// <summary>
	/// Returns first objects in the pool.
	/// </summary>
	public void ReturnNextObject()
	{
		foreach (var poolObjectStatePair in _poolObjectStates)
		{
			if (poolObjectStatePair.Value)
			{
				ReturnObject(poolObjectStatePair.Key);
				return;
			}
		}

		Debug.LogError("No object to release.");
	}

	/// <summary>
	/// Instantiates a new object and adds to the pool.
	/// </summary>
	/// <param name="shouldSpawnEnabled"> shouldSpawn as enabled or disabled. </param>
	/// <returns> the instantiated object.</returns>
	private T InstantiateObjectAndAddToPool(bool shouldSpawnEnabled = false)
	{
		var spawnedObj = Object.Instantiate(_prefab, _parentTransform);
		spawnedObj.gameObject.SetActive(shouldSpawnEnabled);
		_poolObjectStates.Add(spawnedObj, shouldSpawnEnabled);
		return spawnedObj;
	}

}
