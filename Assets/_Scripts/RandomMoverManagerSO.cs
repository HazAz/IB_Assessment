using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomMoverManager", menuName = "ScriptableObjects/RandomMoverManagerSO")]
public class RandomMoverManagerSO : ScriptableObject
{
	[Header("Managers")]
	[SerializeField] private FindNearestNeighbourManagerSO m_findNearestNeighbourManager;

	[Space(10)]
	[Header("Game Settings Parameters")]
	[SerializeField] private Vector3 mapBounds = new Vector3(30, 30, 30);
	[SerializeField] private int segmentSize = 3;
	[SerializeField] private float minTimeToWaitBeforeMove = 2f;
	[SerializeField] private float maxTimeToWaitBeforeMove = 10f;
	[SerializeField] private float minMoveDuration = 1f;
	[SerializeField] private float maxMoveDuration = 4f;

	private Vector3 segmentSizeVector;
	private Dictionary<Vector3Int, List<FindNearestNeighbour>> segmentsDict = new();

	public Dictionary<Vector3Int, List<FindNearestNeighbour>> SegmentsDict { get { return segmentsDict; } }

	/// <summary>
	/// Clears the dictionary. Since this is a ScriptableObject, 
	/// it doesnt automatically clear in the editor, but it will automatically clear in build.
	/// </summary>
	public void Init()
	{
		segmentSizeVector = new Vector3(segmentSize, segmentSize, segmentSize);
		segmentsDict.Clear();
	}

	/// <summary>
	/// Add the current object to a segment and update the segment
	/// </summary>
	/// <param name="findNearestNeighbour"> the object to add to the segment. </param>
	public void AddToSegment(FindNearestNeighbour findNearestNeighbour)
	{
		AddToSegment(findNearestNeighbour, GetSegment(findNearestNeighbour.transform.position));
	}

	/// <summary>
	/// Add the current object to a segment and update the segment
	/// </summary>
	/// <param name="findNearestNeighbour"> the object to add to the segment. </param>
	/// <param name="segment"> The segment we add the object to. </param>
	public void AddToSegment(FindNearestNeighbour findNearestNeighbour, Vector3Int segment)
	{
		if (!segmentsDict.ContainsKey(segment))
		{
			segmentsDict[segment] = new List<FindNearestNeighbour>();
		}

		segmentsDict[segment].Add(findNearestNeighbour);
		m_findNearestNeighbourManager.SegmentUpdated(segment);
	}

	/// <summary>
	/// Remove the current object from a segment and update the segment
	/// </summary>
	/// <param name="findNearestNeighbour"> the object to add to the segment. </param>
	public void RemoveFromSegment(FindNearestNeighbour findNearestNeighbour)
	{
		RemoveFromSegment(findNearestNeighbour, GetSegment(findNearestNeighbour.transform.position));
	}

	/// <summary>
	/// Remove the current object from a segment and update the segment
	/// </summary>
	/// <param name="findNearestNeighbour"> the object to add to the segment. </param>
	/// <param name="segment"> The segment we remove the object from. </param>
	public void RemoveFromSegment(FindNearestNeighbour findNearestNeighbour, Vector3Int segment)
	{
		if (segmentsDict.ContainsKey(segment))
		{
			segmentsDict[segment].Remove(findNearestNeighbour);
			m_findNearestNeighbourManager.SegmentUpdated(segment);
		}
	}

	/// <summary>
	/// Gets the segment the current position is a part of.
	/// </summary>
	/// <param name="position"> the position to check </param>
	/// <returns> The segment the input position is a part of. </returns>
	public Vector3Int GetSegment(Vector3 position)
	{
		return new Vector3Int(
			Mathf.FloorToInt(position.x / segmentSize),
			Mathf.FloorToInt(position.y / segmentSize),
			Mathf.FloorToInt(position.z / segmentSize)
		);
	}

	/// <summary>
	/// Gets a random position within the bounds
	/// </summary>
	/// <returns>a random position within the bounds</returns>
	public Vector3 GetRandomPointWithinBounds()
	{
		return new Vector3(Random.Range(-mapBounds.x, mapBounds.x), Random.Range(-mapBounds.y, mapBounds.y), Random.Range(-mapBounds.z, mapBounds.z));
	}

	/// <summary>
	/// Gets a random speed to move at
	/// </summary>
	/// <returns> A random speed to move at</returns>
	public float GetRandomTimeToMove()
	{
		return Random.Range(minMoveDuration, maxMoveDuration);
	}

	/// <summary>
	/// Gets a random amount of time to wait before the object can move again
	/// </summary>
	/// <returns> A random amount of time to wait before the object can move again</returns>
	public float GetRandomTimeToWaitBeforeMove()
	{
		return Random.Range(minTimeToWaitBeforeMove, maxTimeToWaitBeforeMove);
	}

	/// <summary>
	/// Gets the maximum map bound
	/// </summary>
	/// <returns> the maximum map bound </returns>
	public float GetMaxBound()
	{
		return Mathf.Max(mapBounds.x, mapBounds.y, mapBounds.z);
	}

	/// <summary>
	/// Gets the current segment size.
	/// </summary>
	/// <returns> The segment size.</returns>
	public float GetSegmentSize()
	{
		return segmentSize;
	}
}