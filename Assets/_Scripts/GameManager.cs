using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Managers, prefabs, and UI. DO NOT CHANGE")]
	[SerializeField] private FindNearestNeighbourManagerSO m_FindNearestNeighbourManager;
	[SerializeField] private RandomMoverManagerSO m_RandomMoverManager;
	[SerializeField] private FindNearestNeighbour findNearestObjectPrefab;
	[SerializeField] private TextMeshProUGUI m_AmountSpawnedText;
	[SerializeField] private TMP_InputField m_InputField;

	[Space(10)]
	[Header("Number of Objects to spawn in pool. Change freely")]
	[SerializeField] private int numObjectsToSpawn = 30;

	private int _inputInt;
	private int _currentAmountSpawned = 0;
	private CustomPool<FindNearestNeighbour> m_Pool;


	private void Start()
	{
		m_FindNearestNeighbourManager.Init();
		m_RandomMoverManager.Init();

		m_Pool = new CustomPool<FindNearestNeighbour>(findNearestObjectPrefab, numObjectsToSpawn);

		if (numObjectsToSpawn == 0)
		{
			return;
		}

		StartCoroutine(SpawnObjects(numObjectsToSpawn, true));

		_currentAmountSpawned = numObjectsToSpawn;
		UpdateAmountSpawnedText();
	}

	#region UI Interactions
	public void UpdateInputField()
	{
		int.TryParse(m_InputField.text, out _inputInt);
	}

	public void OnSpawnClicked()
	{
		if (_currentAmountSpawned == 0)
		{
			StartCoroutine(SpawnObjects(_inputInt, true));
		}
		else
		{
			StartCoroutine(SpawnObjects(_inputInt));
		}
		
		_currentAmountSpawned += _inputInt;
		UpdateAmountSpawnedText();
	}

	public void OnDespawnClicked()
	{
		StartCoroutine(DespawnObjects(_inputInt));
		_currentAmountSpawned = Math.Max(0, _currentAmountSpawned - _inputInt);
		UpdateAmountSpawnedText();
	}

	private void UpdateAmountSpawnedText()
	{
		m_AmountSpawnedText.text = $"Number Objects Spawned: {_currentAmountSpawned}";
	}
	#endregion

	#region Spawn and Despawn Coroutine
	/// <summary>
	/// Spawns an item every frame so we don't spawn everything in the same frame and lag the game,
	/// and for each object that spawns checks its nearest neighbour after the previous object was spawned.
	/// </summary>
	/// <param name="pool"> The pool the objects are coming from.</param>
	/// <returns></returns>
	private IEnumerator SpawnObjects(int amountToSpawn, bool isStart = false)
	{
		if (m_Pool == null)
		{
			yield break;
		}

		FindNearestNeighbour firstSpawnedObjAtStart = null;

		if (isStart)
		{
			firstSpawnedObjAtStart = m_Pool.GetObject();
			--amountToSpawn;
		}

		for (int i = 0; i < amountToSpawn; ++i)
		{
			yield return null;
			m_Pool.GetObject();
		}

		if (isStart && firstSpawnedObjAtStart != null)
		{
			firstSpawnedObjAtStart.SetNearestNeighbour(m_FindNearestNeighbourManager.GetNearestNeighbour(firstSpawnedObjAtStart));
		}
	}

	/// <summary>
	/// Despawns objects every frame so we dont despawn everything in the same frame and lag game.
	/// For each object despawned, updates the nearest neighbours of neighbouring objects.
	/// </summary>
	/// <param name="amountToSpawn"></param>
	/// <returns></returns>
	private IEnumerator DespawnObjects(int amountToSpawn)
	{
		if (m_Pool == null)
		{
			yield break;
		}

		for (int i = 0; i < amountToSpawn; ++i)
		{
			yield return null;
			m_Pool.ReturnNextObject();
		}
	}

	#endregion
}
