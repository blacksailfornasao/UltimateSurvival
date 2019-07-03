namespace UltimateSurvival
{
    public class ET
    {
        //--------------AI ENUMS-------------------//

        public enum ActionRepeatType
        {
            Single, Repetitive
        }

        public enum PointOrder
        {
            Sequenced, Random
        }

        public enum AIMovementState
        {
            Idle, Walking, Running
        }

        //--------------BUILDING SYSTEM ENUMS-------------------//

        public enum BuildableType
        {
            Foundation, Wall, Floor
        }

        public enum MaterialType
        {
            Wood, Stone, Metal
        }

        /*
        public enum ModelVariationType
        {
            Lucian, Mischa
        }

        public enum SizeVariationType
        {
            OneByOne, TwoByTwo, TwoByThree //...Keeps going
        }*/ //Future Release

        //-------------INPUT RELATED----------------------------//

        public enum InputType
        {
            Standalone, Mobile
        }

        public enum InputMode
        {
            Buttons, Axes
        }

        public enum StandaloneAxisType
        {
            Unity, Custom
        }

        public enum MobileAxisType
        {
            Custom
        }

        public enum ButtonState
        {
            Down,
            Up,
        }

        //---------------------EVENT RELATED----------------------//

        public enum CharacterType
        {
            Player
        }

		//---------------------WEAPONS & COMBAT----------------------//

		/// <summary>
		/// 
		/// </summary>
		public enum FireMode
		{
			/// <summary> </summary>
			SemiAuto,

			/// <summary> </summary>
			Burst,

			/// <summary> </summary>
			FullAuto
		}

        //---------------------Editor----------------------//

        public enum FileCreatorMode
        {
            ScriptableObject, ScriptFile, Both
        }

        //---------------------DAY & NIGHT CYCLE----------------------//
        public enum TimeOfDay
        {
            Day, Night
        }

        //---------------------INVENTORY RELATED----------------------//
        public enum InventoryState
        {
            Closed = 0,
            Normal = 1,
            Loot = 2,
            Furnace = 3,
            Anvil = 4,
            Campfire = 6
        }
    }
}