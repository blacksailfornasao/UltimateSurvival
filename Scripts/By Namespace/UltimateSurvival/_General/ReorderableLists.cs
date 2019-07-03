using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateSurvival.Debugging;

namespace UltimateSurvival
{
	[Serializable]
	public class ReorderableGenericList<T> : IEnumerable<T>
	{
		public T this[int key] { get { return m_List[key]; } set { m_List[key] = value; } }

		public int Count { get { return m_List.Count; } }

		public List<T> List { get { return m_List; } }
	
		[SerializeField]
		private List<T> m_List;


		public IEnumerator<T> GetEnumerator()
		{
			return m_List.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}

	[Serializable]
	public class ReorderableBoolList : ReorderableGenericList<bool> {  }

	[Serializable]
	public class ReorderableIntList : ReorderableGenericList<int> {  }

	[Serializable]
	public class ReorderableFloatList : ReorderableGenericList<float> {  }

	[Serializable]
	public class ReorderableStringList : ReorderableGenericList<string> {  }

	[Serializable]
	public class ReorderableVector2List : ReorderableGenericList<Vector2> {  }

	[Serializable]
	public class ReorderableVector3List : ReorderableGenericList<Vector3> {  }

	[Serializable]
	public class ReorderableQuaternionList : ReorderableGenericList<Quaternion> {  }

	[Serializable]
	public class ReorderableTransformList : ReorderableGenericList<Transform> {  }

	[Serializable]
	public class ReorderableRectTransformList : ReorderableGenericList<RectTransform> {  }

	[Serializable]
	public class ReorderableItemToAddList : ReorderableGenericList<StartupItems.ItemToAdd> {  }
}