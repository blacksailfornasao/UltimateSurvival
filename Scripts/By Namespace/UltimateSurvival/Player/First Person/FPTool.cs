using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class FPTool : FPMelee
	{
		public enum ToolPurpose { CutWood, BreakRocks }

		[Header("Tool Settings")]

		[SerializeField]
		[Tooltip("Useful for making the tools gather specific resources (eg. an axe should gather only wood, pickaxe - only stone)")]
		private ToolPurpose[] m_ToolPurposes;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_Efficiency = 0.5f;


		protected override void On_Hit()
		{
			base.On_Hit();

			var raycastData = Player.RaycastData.Get();

			if(!raycastData)
				return;

			var mineable = raycastData.GameObject.GetComponent<MineableObject>();
			if(mineable)
				mineable.OnToolHit(m_ToolPurposes, DamagePerHit, m_Efficiency);
		}
	}
}
