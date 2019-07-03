using UnityEngine;
using UltimateSurvival;

namespace UltimateSurvival.Editor
{
	using UnityEditor;

	public class TerrainObjectSpawner : EditorWindow 
	{
		[SerializeField]
		private DetailSpawnData m_SpawnData;

		[SerializeField]
		private Terrain m_Terrain;

		[SerializeField]
		[Range(1, 1000)]
		private int m_SpawnOdds = 350;

		private SerializedObject m_SO;


		[MenuItem("Tools/Ultimate Survival/Terrain Object Spawner", false, -7)]
		public static void Init()
		{
			EditorWindow.GetWindowWithRect<TerrainObjectSpawner>(new Rect(Screen.width / 2f, Screen.height / 2f, 512f, 256f), true, "Terrain Object Spawner");
		}

		private void OnEnable()
		{
			m_SO = new SerializedObject(this);
		}

		private void OnGUI()
		{
			m_SO.Update();

			GUIStyle style = new GUIStyle()
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleLeft
			};

			DoHeader("Setup", style);

			EditorGUILayout.PropertyField(m_SO.FindProperty("m_SpawnData"));
			EditorGUILayout.PropertyField(m_SO.FindProperty("m_Terrain"));

			DoHeader("Settings", style);

			EditorGUILayout.PropertyField(m_SO.FindProperty("m_SpawnOdds"));

			EditorGUILayout.Space();
			if(GUILayout.Button("Generate"))
				Generate();

			EditorGUILayout.Space();
			if(GUILayout.Button("Remove"))
				Remove();

			EditorGUILayout.HelpBox("WARNING! Will destroy all the current rocks and trees!", MessageType.Warning);

			m_SO.ApplyModifiedProperties();
		}

		private void Generate()
		{
			if(!m_Terrain || !m_SpawnData)
				return;

			Transform treesRoot = m_Terrain.transform.Find("Trees");
			Transform rocksRoot = m_Terrain.transform.Find("Rocks");

			if(m_SpawnData.m_TreeList.Length > 0 && !treesRoot)
				treesRoot = CreateEmptyUnder("Trees", m_Terrain.transform);
			if(!rocksRoot)
				rocksRoot = CreateEmptyUnder("Rocks", m_Terrain.transform);

			TerrainCollider terrainCollider = m_Terrain.GetComponent<TerrainCollider>();
			Vector3 terrainSize = m_Terrain.terrainData.size;
			Vector3 terrainPosition = m_Terrain.GetPosition();

			for(int x = 0;x < terrainSize.x; x ++)
			{
				for(int y = 0; y < terrainSize.z; y ++)
				{
					bool spawn = Random.Range(0, m_SpawnOdds) == (m_SpawnOdds / 2);
					if(!spawn)
						continue;

					bool spawnTree = Random.Range(0, 2) == 0;
					if(m_SpawnData.m_TreeList.Length == 0)
						spawnTree = false;

					DetailSpawnData.DetailSpawn prefab = spawnTree ? m_SpawnData.GetTreePrefab() : m_SpawnData.GetRockPrefab();

					if(prefab != null)
					{
						float raycastHeight = terrainPosition.y + terrainSize.y + 1;
						Ray ray = new Ray(new Vector3(x, raycastHeight, y), Vector3.down);
						RaycastHit hitInfo;

						if(terrainCollider.Raycast(ray, out hitInfo, terrainSize.y * 2))
							Instantiate(prefab.Object, hitInfo.point + prefab.PositionOffset, Quaternion.Euler(prefab.RandomRotation), spawnTree ? treesRoot : rocksRoot);
					}
				}
			}
		}

		private void Remove()
		{
			if(!m_Terrain)
				return;

			Transform treesRoot = m_Terrain.transform.Find("Trees");
			Transform rocksRoot = m_Terrain.transform.Find("Rocks");

			if(!treesRoot)
				treesRoot = CreateEmptyUnder("Trees", m_Terrain.transform);
			if(!rocksRoot)
				rocksRoot = CreateEmptyUnder("Rocks", m_Terrain.transform);

			int treeCount = treesRoot.childCount;

			for(int i = 0;i < treeCount;i ++)
				DestroyImmediate(treesRoot.GetChild(treesRoot.childCount - 1).gameObject);

			int rocksCount = rocksRoot.childCount;

			for(int i = 0;i < rocksCount;i ++)
				DestroyImmediate(rocksRoot.GetChild(rocksRoot.childCount - 1).gameObject);
		}

		private Transform CreateEmptyUnder(string name, Transform parent)
		{
			var empty = new GameObject(name);
			empty.transform.SetParent(parent);

			return empty.transform;
		}

		private void DoHeader(string name, GUIStyle style)
		{
			EditorGUILayout.Space();
			GUILayout.Label(name, style);
		}
	}
}
