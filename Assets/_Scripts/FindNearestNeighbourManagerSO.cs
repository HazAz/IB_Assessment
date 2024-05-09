using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FindNearestNeighbourManager", menuName = "ScriptableObjects/FindNearestNeighbourManagerSO")]
public class FindNearestNeighbourManagerSO : ScriptableObject
{
	[Header("Managers")]
	[SerializeField] private RandomMoverManagerSO m_RandomMoverManager;

	private int maxRangeToCheck = 0;

	public void Init()
	{
		maxRangeToCheck = (int) Math.Ceiling(m_RandomMoverManager.GetMaxBound()/ m_RandomMoverManager.GetSegmentSize()) + 1;
	}

	/// <summary>
	/// Finds the neareest neighbour of the current object
	/// </summary>
	/// <param name="findNearestNeighbourGO"> The object to find the nearest neighbour of.</param>
	/// <returns> the nearest neighbour of the object. Returs null if it doesnt exist. </returns>
	public FindNearestNeighbour GetNearestNeighbour(FindNearestNeighbour findNearestNeighbourGO)
	{
		if (findNearestNeighbourGO == null) return null;

		var closestNeighbours = GetClosestNeighboursList(m_RandomMoverManager.GetSegment(findNearestNeighbourGO.transform.position), 1);

		FindNearestNeighbour result = null;
		float minDistanceSqrd = float.MaxValue;

		foreach (var neighbour in closestNeighbours)
		{
			// Leaving it as squared because no need to do the expensive square root in Vector3.Distance
			var distanceToEnabledObjectSqrd = (findNearestNeighbourGO.transform.position - neighbour.transform.position).sqrMagnitude;

			if (distanceToEnabledObjectSqrd < minDistanceSqrd)
			{
				minDistanceSqrd = distanceToEnabledObjectSqrd;
				result = neighbour;
			}
		}

		return result;
	}

	/// <summary>
	/// Gets a list of the nearest neighbours, either in the same segment or in the nearest segments.
	/// </summary>
	/// <param name="segment"> The current segment the object is in</param>
	/// <param name="range"> The distance from the segment we're checking form. </param>
	/// <returns> a list of all the potential nearest neighbours. Returns an empty list if none exist.</returns>
	private List<FindNearestNeighbour> GetClosestNeighboursList(Vector3Int segment, int range)
	{
		if (range == maxRangeToCheck)
		{
			return new();
		}

		List<FindNearestNeighbour> closestNeighboursList = new();
		var segmentsDict = m_RandomMoverManager.SegmentsDict;

		for (int xOffset = -range; xOffset <= range; xOffset++)
		{
			for (int yOffset = -range; yOffset <= range; yOffset++)
			{
				for (int zOffset = -range; zOffset <= range; zOffset++)
				{
					// Exclude the target cell itself
					if (Math.Abs(xOffset) != range && Math.Abs(yOffset) != range && Math.Abs(zOffset) != range) 
					{
						continue;
					}

					// Calculate the coordinates of the neighboring cell
					Vector3Int neighborCell = segment + new Vector3Int(xOffset, yOffset, zOffset);

					if (segmentsDict.ContainsKey(neighborCell) && segmentsDict[neighborCell] != null)
					{
						closestNeighboursList.AddRange(segmentsDict[neighborCell]);
					}
				}
			}
		}

		return closestNeighboursList.Count != 0 ? closestNeighboursList : GetClosestNeighboursList(segment, range + 1);
	}

	/// <summary>
	/// Checks the nearest neighbour of all objects in that segment.
	/// If there are no objects in the current segment, checks nearest neighbours of the nearest segments with objects.
	/// </summary>
	/// <param name="segment"> current segment that was updated. </param>
	public void SegmentUpdated(Vector3Int segment)
	{
		var segmentsDict = m_RandomMoverManager.SegmentsDict;
		List<FindNearestNeighbour> neighboursList = null;

		if (segmentsDict.ContainsKey(segment))
		{
			neighboursList = segmentsDict[segment];
		}

		if (neighboursList == null || neighboursList.Count == 0)
		{
			neighboursList = GetClosestNeighboursList(segment, 1);
		}

		foreach(var neighbour in neighboursList)
		{
			neighbour.SetNearestNeighbour(GetNearestNeighbour(neighbour));
		}
	}
}