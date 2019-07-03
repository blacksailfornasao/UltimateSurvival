using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannibalResetter : MonoBehaviour 
{
	[SerializeField]
	private GameObject m_CannibalPrefab;

	[SerializeField]
	private Transform[] m_SpawnPoints;

	[SerializeField]
	private Transform m_Container;


	private void Start()
	{
		ResetCannibals();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.O))
			ResetCannibals();
	}

	private void ResetCannibals()
	{
		int initialChildCount = m_Container.childCount;
		for(int i = 0;i < initialChildCount;i ++)
			Destroy(m_Container.GetChild(i).gameObject);

		for(int i = 0;i < m_SpawnPoints.Length;i ++)
			Instantiate(m_CannibalPrefab, m_SpawnPoints[i].position, m_SpawnPoints[i].rotation, m_Container);
	}
}
