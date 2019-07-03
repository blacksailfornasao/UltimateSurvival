using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAtStart : MonoBehaviour 
{
	[SerializeField]
	private GameObject[] m_Objects;
	
	
	private void Start()
	{
		foreach(var obj in m_Objects)
			obj.SetActive(false);
	}
}
