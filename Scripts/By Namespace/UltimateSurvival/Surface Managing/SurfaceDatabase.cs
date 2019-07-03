using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class SurfaceDatabase : ScriptableSingleton<SurfaceDatabase>
	{
		[SerializeField]
		private SurfaceData m_DefaultSurface;

		[SerializeField]
		private SurfaceData[] m_Surfaces;


		/// <summary>
		/// 
		/// </summary>
		public SurfaceData GetSurfaceData(Texture texture)
		{
			for(int i = 0;i < m_Surfaces.Length;i ++)
				if(m_Surfaces[i].HasTexture(texture))
					return m_Surfaces[i];

			return m_DefaultSurface;
		}

		/// <summary>
		/// 
		/// </summary>
		public SurfaceData GetSurfaceData(Ray ray, float maxDistance, LayerMask mask)
		{
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo, maxDistance, mask, QueryTriggerInteraction.Ignore))
				return GetSurfaceData(hitInfo.collider, hitInfo.point, hitInfo.triangleIndex);

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public SurfaceData GetSurfaceData(RaycastHit hitInfo)
		{
			return GetSurfaceData(hitInfo.collider, hitInfo.point, hitInfo.triangleIndex);
		}
			
		public SurfaceData GetSurfaceData(Collider collider, Vector3 position, int triangleIndex)
		{
			Texture texture = null;

			// If the ground is a terrain.
			if(collider.GetType() == typeof(TerrainCollider)) 
			{
				Terrain terrain = collider.GetComponent<Terrain>();
				TerrainData terrainData = terrain.terrainData;
				float[] textureMix = GetTerrainTextureMix(position, terrainData, terrain.GetPosition());
				int textureIndex = GetTerrainTextureIndex(position, textureMix);
				texture = terrainData.splatPrototypes[textureIndex].texture;
			}
			// If the ground is a normal mesh.
			else
				texture = GetMeshTexture(collider, triangleIndex);
		
			if(texture)
			{
				for(int i = 0;i < m_Surfaces.Length;i ++)
					if(m_Surfaces[i].HasTexture(texture))
						return m_Surfaces[i];

				return m_DefaultSurface;
			}
			else
				return m_DefaultSurface;
		}

		/// <summary>
		/// 
		/// </summary>
		private Texture GetMeshTexture(Collider collider, int triangleIndex) 
		{
			// If the object overrides the renderer with it's own texture ... (most probably it doesn't have a Renderer).
			var surfaceID = collider.GetComponent<SurfaceIdentity>();
			if(surfaceID)
				return surfaceID.Texture;

			Renderer renderer = collider.GetComponent<Renderer>();
			MeshCollider meshCollider = collider as MeshCollider;

			if(!renderer|| !renderer.sharedMaterial|| !renderer.sharedMaterial.mainTexture)
				return null;
			else if(!meshCollider || meshCollider.convex)
				return renderer.material.mainTexture;

			Mesh mesh = meshCollider.sharedMesh;
			int materialIndex = -1;
			int lookupIndex1 = mesh.triangles[triangleIndex * 3];
			int lookupIndex2 = mesh.triangles[triangleIndex * 3 + 1];
			int lookupIndex3 = mesh.triangles[triangleIndex * 3 + 2];

			for(int i = 0;i < mesh.subMeshCount;i ++) 
			{
				int[] triangles = mesh.GetTriangles(i);

				for(int j = 0;j < triangles.Length;j += 3)
				{
					if (triangles[j] == lookupIndex1 && triangles[j + 1] == lookupIndex2 && triangles[j + 2] == lookupIndex3) 
					{
						materialIndex = i;
						break;
					}
				}

				if(materialIndex != -1) 
					break;
			}
				
			return renderer.materials[materialIndex].mainTexture;
		}

		/// <summary>
		/// 
		/// </summary>
		private float[] GetTerrainTextureMix(Vector3 worldPos, TerrainData terrainData, Vector3 terrainPos)
		{
			// Returns an array containing the relative mix of textures
			// on the terrain at this world position.

			// The number of values in the array will equal the number
			// of textures added to the terrain.

			// calculate which splat map cell the worldPos falls within (ignoring y)
			int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
			int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

			// Get the splat data for this cell as a 1x1xN 3D array (where N = number of textures)
			float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

			// extract the 3D array data to a 1D array:
			float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

			for(int n = 0;n < cellMix.Length;n++)
				cellMix[n] = splatmapData[0, 0, n];

			return cellMix;
		}

		/// <summary>
		/// 
		/// </summary>
		private int GetTerrainTextureIndex(Vector3 worldPos, float[] textureMix) 
		{
			// Returns the zero-based index of the most dominant texture
			// on the terrain at this world position.
			float maxMix = 0;
			int maxIndex = 0;

			// Loop through each mix value and find the maximum.
			for(int n = 0;n < textureMix.Length;n ++)
			{
				if (textureMix[n] > maxMix)
				{
					maxIndex = n;
					maxMix = textureMix[n];
				}
			}

			return maxIndex;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public enum ItemSelectionMethod
	{
		/// <summary>The items will be selected randomly.</summary>
		Randomly,

		/// <summary>The items will be selected randomly, but will exclude the one that was selected before.</summary>
		RandomlyButExcludeLast,

		/// <summary>The items will be selected in sequence.</summary>
		InSequence
	}

	/// <summary>
	/// 
	/// </summary>
	public enum SoundType
	{
		Footstep,
		BulletImpact,
		Chop,
		Hit,
		Jump,
		Land,
		SpearPenetration,
		ArrowPenetration
	}
}
