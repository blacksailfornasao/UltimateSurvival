using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// When this is attached to an object, this is the component that will be searched for a texture, that defines it's surface, it will not search on the Renderer.
	/// It's mostly used for objects that have no Renderer attached and want to define a texture so specific effects, footsteps can be played etc.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class SurfaceIdentity : MonoBehaviour 
	{
		/// <summary>The texture for this surface (useful when there's no renderer attached to check for textures).</summary>
		public Texture Texture { get { return m_Texture; } set { m_Texture = value; } }

		[SerializeField]
		[Tooltip("The texture for this surface (useful when there's no renderer attached to check for textures).")]
		private Texture m_Texture;
	}
}
