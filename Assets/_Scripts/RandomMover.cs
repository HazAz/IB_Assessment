using System.Collections;
using UnityEngine;

public class RandomMover : MonoBehaviour
{
	[SerializeField] private RandomMoverManagerSO m_moverManager;
	[SerializeField] private FindNearestNeighbourManagerSO m_findNearestNeighbourManager;
	[SerializeField] private FindNearestNeighbour m_findNearestNeighbour;

	private void Awake()
	{
		if (m_findNearestNeighbour == null)
		{
			m_findNearestNeighbour = GetComponent<FindNearestNeighbour>();
		}
	}

	// When object is enabled, add it to a segment, check its nearest neighbours,
	// and update the segment so it checks if this object is the nearest neighbour of existing objects
	// in the current or nearby segments.
	private void OnEnable()
	{
		transform.position = m_moverManager.GetRandomPointWithinBounds();
		m_moverManager.AddToSegment(m_findNearestNeighbour);
		StartCoroutine(Move());
	}

	// When object is disabled, remove it from a segment,
	// and update the segment so it checks if this object was the nearest neighbour of existing objects
	// in the current or nearby segments, and find their new nearest neighbour
	private void OnDisable()
	{
		StopCoroutine(Move());
		m_moverManager.RemoveFromSegment(m_findNearestNeighbour);
	}

	/// <summary>
	/// Randomly move to a random position within the bounds at a random speed while it's active
	/// </summary>
	/// <returns></returns>
	private IEnumerator Move()
	{
		while (gameObject.activeInHierarchy)
		{
			yield return new WaitForSeconds(m_moverManager.GetRandomTimeToWaitBeforeMove());

			Vector3 startingPos = transform.position;
			Vector3 finalPos = m_moverManager.GetRandomPointWithinBounds();
			float time = m_moverManager.GetRandomTimeToMove();

			float elapsedTime = 0;
			Vector3Int previousSegment, currentSegment;

			while (elapsedTime < time)
			{
				// Handle movement
				previousSegment = m_moverManager.GetSegment(transform.position);
				transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
				currentSegment = m_moverManager.GetSegment(transform.position);

				if (previousSegment != currentSegment)
				{
					m_moverManager.RemoveFromSegment(this.m_findNearestNeighbour, previousSegment);
					m_moverManager.AddToSegment(this.m_findNearestNeighbour, currentSegment);
				}
				// Edge case where the object randomly moves only within the same segment may return an incorrect result.
				// If the correct nearest neighbour is an absolute must, update the segment the object is in every frame its moving
				// by uncommenting the code below. If optimization is key and not getting the correct result 100% of the time is okay,
				// Keep it commented.
				/*
				else
				{
					m_findNearestNeighbourManager.SegmentUpdated(currentSegment);
				}
				*/

				elapsedTime += Time.deltaTime;
				m_findNearestNeighbour.FindNearestNeighbouringObject();
				yield return null;
			}
		}
	}
}
