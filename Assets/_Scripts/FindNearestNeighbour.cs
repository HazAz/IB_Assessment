using UnityEngine;

public class FindNearestNeighbour : MonoBehaviour
{
	[SerializeField] private FindNearestNeighbourManagerSO m_FindNearestNeighbourManager;
	[SerializeField] private LineRenderer m_LineRenderer;

	private FindNearestNeighbour m_NearestNeighbour;

	private void Awake()
	{
		if (m_LineRenderer == null)
		{
			m_LineRenderer = GetComponent<LineRenderer>();
		}
	}

	private void Update()
	{
		UpdateLineRenderer();
	}

	/// <summary>
	/// Finds the nearest neighbour of the current object
	/// </summary>
	public void FindNearestNeighbouringObject()
	{
		m_NearestNeighbour = m_FindNearestNeighbourManager.GetNearestNeighbour(this);
	}

	/// <summary>
	/// Sets the nearest neighbour of the object.
	/// </summary>
	/// <param name="neighbour"> the nearest neighbour of the current object. </param>
	public void SetNearestNeighbour(FindNearestNeighbour neighbour)
	{
		m_NearestNeighbour = neighbour;
	}

	/// <summary>
	/// Updates the positions of the line renderers.
	/// </summary>
	private void UpdateLineRenderer()
	{
		if (m_NearestNeighbour != null)
		{
			m_LineRenderer.enabled = true;
			m_LineRenderer.SetPosition(0, transform.position);
			m_LineRenderer.SetPosition(1, m_NearestNeighbour.transform.position);
		}
		else
		{
			m_LineRenderer.enabled = false;
		}
	}
}
