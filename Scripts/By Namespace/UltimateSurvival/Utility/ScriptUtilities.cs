using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
    public static class ScriptUtilities
    {
		/// <summary>
		/// 
		/// </summary>
        public static List<Transform> GetTransformsByTag(string tag)
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);

            List<Transform> trans = new List<Transform>();

            for(int i = 0; i < gos.Length; i++)
                trans.Add(gos[i].transform);

            return trans;
        }

        public static bool GetTransformsPositionsByTag(string tag, out List<Vector3> posS)
        {
            bool toReturn = false;

            GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);

            posS = new List<Vector3>();

            for (int i = 0; i < gos.Length; i++)
                posS.Add(gos[i].transform.position);

            if (posS.Count > 0)
                toReturn = true;

            return toReturn;
        }

        public static List<Vector3> GetRandomPositionsAroundTransform(Transform transform, int amount = 5, int radius = 5, float distanceBtwPoints = 5)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < amount; i++)
            {
                Vector3 posToAdd = (Random.insideUnitSphere * radius) + transform.position;
                posToAdd.y = transform.position.y;

                if (points.IndexIsValid(i - 1))
                {
                    float dst = Vector3.Distance(posToAdd, points[i - 1]);

                    if (dst < distanceBtwPoints)
                        posToAdd += new Vector3(dst, 0, dst);
                }

                points.Add(posToAdd);
            }

            return points;
        }
    }
}