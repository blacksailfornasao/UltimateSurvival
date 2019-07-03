using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	[Serializable]
	public class TreeCreator
	{
		public int PrototypeIndex { get { return m_PrototypeIndex; } }

		[SerializeField]
		[ClampAttribute(0, 100)]
		private int m_PrototypeIndex;

		[SerializeField]
		private MineableObject m_Prefab;

		[SerializeField]
		private Vector3 m_PositionOffset;

		[SerializeField]
		private Vector3 m_RotationOffset;


		public MineableObject CreateTree(Terrain terrain, TreeInstance treeInstance, int treeIndex)
		{
			if(!m_Prefab)
			{
				Debug.LogError("[TreeCreator] - This tree creator has no tree prefab assigned! | Prototype Index: " + m_PrototypeIndex);
				return null;
			}

			Transform parent = null;
			if(terrain.transform.FindDeepChild("Trees") != null)
				parent = terrain.transform.FindDeepChild("Trees");
			else
			{
				parent = new GameObject("Trees").transform;
				parent.position = terrain.transform.position;
				parent.SetParent(terrain.transform, true);
			}

			Vector3 position = terrain.transform.position + Vector3.Scale(treeInstance.position, terrain.terrainData.size);
			var tree = GameObject.Instantiate(m_Prefab, position + m_PositionOffset, Quaternion.Euler(m_RotationOffset), parent);

			tree.Destroyed.AddListener(()=> On_TreeDestroyed(terrain, treeInstance, treeIndex));

			return tree;
		}

		private void On_TreeDestroyed(Terrain terrain, TreeInstance treeInstance, int treeIndex)
		{
			treeInstance.widthScale = treeInstance.heightScale = 0f;
			terrain.terrainData.SetTreeInstance(treeIndex, treeInstance);
		}
	}

	[RequireComponent(typeof(Terrain))]
	public class TreeManager : MonoBehaviour 
	{
		[SerializeField]
		private TreeCreator[] m_TreeCreators;

		private Terrain m_Terrain;
		private List<MineableObject> m_Trees = new List<MineableObject>();
		private TreeInstance[] m_InitialTrees;


		private void Awake()
		{
			m_Terrain = GetComponent<Terrain>();
			m_InitialTrees = m_Terrain.terrainData.treeInstances;

			var treeInstances = m_Terrain.terrainData.treeInstances;

			for(int i = 0;i < treeInstances.Length;i ++)
			{
				var creator = GetTreeCreator(treeInstances[i].prototypeIndex);

				if(creator != null)
					m_Trees.Add(creator.CreateTree(m_Terrain, treeInstances[i], i));
			}
		}

		private TreeCreator GetTreeCreator(int prototypeIndex)
		{
			for(int i = 0;i < m_TreeCreators.Length;i ++)
			{
				if(m_TreeCreators[i].PrototypeIndex == prototypeIndex)
					return m_TreeCreators[i];
			}

			return null;
		}

		private void OnApplicationQuit()
		{
			m_Terrain.terrainData.treeInstances = m_InitialTrees;
		}
	}
}