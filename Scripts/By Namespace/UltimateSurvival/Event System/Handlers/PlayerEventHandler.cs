using System.Collections.Generic;
using UnityEngine;
using UltimateSurvival.Building;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayerEventHandler : EntityEventHandler
	{
		/// <summary>Used for respawning when dying, if the position is (0, 0, 0), the player will be respawned through other methods.</summary>
		public Value<Vector3> LastSleepPosition = new Value<Vector3>(Vector3.zero);

		/// <summary></summary>
		public Value<float> Stamina = new Value<float>(100f);

		/// <summary></summary>
		public Value<float> Thirst = new Value<float>(100f);

		/// <summary></summary>
		public Value<float> Hunger = new Value<float>(100f);

		/// <summary></summary>
		public Value<int> Defense = new Value<int>(0);

		/// <summary></summary>
		public Value<Vector2> MovementInput = new Value<Vector2>(Vector2.zero);

		/// <summary></summary>
		public Value<Vector2> LookInput	= new Value<Vector2>(Vector2.zero);

		/// <summary></summary>
		public Value<Vector3> LookDirection = new Value<Vector3>(Vector3.zero);

		/// <summary></summary>
		public Value<bool> ViewLocked = new Value<bool>(false);

		/// <summary></summary>
		public Value<float> MovementSpeedFactor = new Value<float>(1f);

		/// <summary></summary>
		public Queue<Transform> NearLadders = new Queue<Transform>();

		/// <summary></summary>
		public Value<RaycastData> RaycastData = new Value<RaycastData>(null);

		/// <summary></summary>
		public Attempt InteractOnce = new Attempt();

		/// <summary></summary>
		public Value<bool> InteractContinuously = new Value<bool>(false);

		/// <summary>Is the player character too close to a wall / object? (and it's facing it)</summary>
		public Value<bool> IsCloseToAnObject = new Value<bool>(false);

		/// <summary>
		/// <para>SavableItem - item to equip</para>
		/// <para>bool - do it instantly?</para>
		/// </summary>
		public Attempt<SavableItem, bool> ChangeEquippedItem = new Attempt<SavableItem, bool>();

		/// <summary></summary>
		public Value<SavableItem> EquippedItem = new Value<SavableItem>(null);

		/// <summary>Mainly used when the durability of an item hits 0, and the equipped item should be destroyed.</summary>
		public Attempt DestroyEquippedItem = new Attempt();

		/// <summary></summary>
		public Attempt<SleepingBag> StartSleeping = new Attempt<SleepingBag>();

		/// <summary></summary>
		public Activity Sleep = new Activity();

		/// <summary></summary>
		public Activity	Walk = new Activity();

		/// <summary></summary>
		public Attempt AttackOnce = new Attempt();

		/// <summary></summary>
		public Attempt AttackContinuously = new Attempt();

		/// <summary></summary>
		public Value<bool> CanShowObjectPreview = new Value<bool>(false);

		/// <summary> </summary>
		public Attempt PlaceObject = new Attempt();

		/// <summary></summary>
		public Value<float> ScrollValue = new Value<float>(0f);

		/// <summary></summary>
		public Value<BuildingPiece> SelectedBuildable = new Value<BuildingPiece>(null);

		/// <summary></summary>
		public Activity SelectBuildable = new Activity();

		/// <summary></summary>
		public Attempt<float> RotateObject = new Attempt<float>();

		/// <summary></summary>
		public Activity Run = new Activity();

		/// <summary></summary>
		public Activity Crouch = new Activity();

		/// <summary></summary>
		public Activity Jump = new Activity();

		/// <summary></summary>
		public Activity Aim = new Activity();
	}
}
