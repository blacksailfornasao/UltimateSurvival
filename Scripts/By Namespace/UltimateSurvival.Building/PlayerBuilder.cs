using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateSurvival.GUISystem;

namespace UltimateSurvival.Building
{
	/// <summary>
	/// Used for showing a buildable preview, rotating, snapping and placing.
	/// </summary>
    public class PlayerBuilder : MonoBehaviour
    {
        [SerializeField]
        private BuildingHelpers m_BuildingHelpers;

        [SerializeField]
		[Tooltip("How fast the player can rotate buildables.")]
        private float m_RotationSpeed = 120f;

        [SerializeField]
        private AudioSource m_AudioSource;

		[SerializeField]
		[Tooltip("A shake that will be played when the player places an object.")]
		private GenericShake m_PlaceShake;

		private float m_NextTimeCanPlayAudio;

        private PlayerEventHandler m_Player;
		private ItemContainer m_InventoryContainer;
		private BuildingPiece m_SelectedPiece;


        private void Start()
        {
			m_Player = GameController.LocalPlayer;
            m_BuildingHelpers.Initialize(transform, m_Player, m_AudioSource);

			m_Player.PlaceObject.SetTryer(Try_Place);
			m_Player.PlaceObject.AddListener(m_BuildingHelpers.PlacePiece);

			m_Player.RotateObject.SetTryer(Try_RotateObject);

			m_Player.EquippedItem.AddChangeListener(OnChanged_EquippedItem);
			m_Player.SelectedBuildable.AddChangeListener(OnChanged_SelectedBuildable);

			m_InventoryContainer = GUIController.Instance.GetContainer("Inventory");
        }

		private void OnChanged_EquippedItem()
		{
			var equippedItem = m_Player.EquippedItem.Get();

			if(equippedItem != null && equippedItem.HasProperty("Is Placeable"))
			{
				var placeable = Instantiate(equippedItem.ItemData.WorldObject);
				placeable.transform.position = Vector3.one * 10000f;
				m_Player.SelectedBuildable.Set(placeable.GetComponent<BuildingPiece>());
			}
				
			if(equippedItem != null && equippedItem.HasProperty("Allows Building"))
				m_Player.SelectedBuildable.Set(m_SelectedPiece);

			UpdatePreview();
		}

		private void OnChanged_SelectedBuildable()
		{
			// If we have the building plan in our hands..
			if(m_Player.EquippedItem.Get() != null && m_Player.EquippedItem.Get().HasProperty("Allows Building"))
				m_SelectedPiece = m_Player.SelectedBuildable.Get();

			UpdatePreview();
		}

		private void UpdatePreview()
		{
            if(m_BuildingHelpers.PreviewExists())
				m_BuildingHelpers.ClearPreview();

			var selectedBuildable = m_Player.SelectedBuildable.Get();

			if(selectedBuildable != null && m_Player.EquippedItem.Get() != null && (m_Player.EquippedItem.Get().HasProperty("Allows Building") || m_Player.EquippedItem.Get().HasProperty("Is Placeable")))
			{
				m_BuildingHelpers.SpawnPreview(selectedBuildable.gameObject);
                m_BuildingHelpers.PreviewColor = m_BuildingHelpers.CurrentPreviewPiece.Renderers[0].material.color;
			}
		}

		private bool Try_RotateObject(float rotationSign)
		{
			if(m_Player.ViewLocked.Is(false) && m_BuildingHelpers.PreviewExists())
			{
				m_BuildingHelpers.RotationOffset += m_RotationSpeed * rotationSign;
				return true;
			}

			return false;
		}

        private bool Try_Place()
        {
			if(!m_BuildingHelpers.PreviewExists() || !m_BuildingHelpers.PlacementAllowed)
				return false;

			bool isPlaceable = m_Player.EquippedItem.Get().HasProperty("Is Placeable");

			if(isPlaceable)
			{
				m_BuildingHelpers.PlacePiece();
				m_BuildingHelpers.CurrentPreviewPiece.BuildAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, m_AudioSource);
				m_PlaceShake.Shake(1f);

				var item = m_Player.EquippedItem.Get();
				if(item.CurrentInStack > 1)
					item.CurrentInStack --;
				else
					m_Player.DestroyEquippedItem.Try();

				return true;
			}

			bool hasRequiredItems = HasRequiredItems();

			if(hasRequiredItems)
			{
				m_BuildingHelpers.CurrentPreviewPiece.BuildAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, m_AudioSource);
				m_PlaceShake.Shake(1f);

				// Remove the items from the inventory.
				for(int i = 0;i < m_BuildingHelpers.CurrentPreviewPiece.RequiredItems.Length;i ++)
				{
					var requiredItem = m_BuildingHelpers.CurrentPreviewPiece.RequiredItems[i];
					m_InventoryContainer.RemoveItems(requiredItem.Name, requiredItem.Amount);
				}

				return true;
			}
			else
			{
				string message = "You don't have all the required materials: \n";
				for(int i = 0;i < m_BuildingHelpers.CurrentPreviewPiece.RequiredItems.Length;i ++)
				{
					var requiredItem = m_BuildingHelpers.CurrentPreviewPiece.RequiredItems[i];
					message += string.Format("<color=yellow>{0}</color> x {1}, ", requiredItem.Name, requiredItem.Amount);
				}

				MessageDisplayer.Instance.PushMessage(message, default(Color), 48);
			}

			return false;
        }

		private bool HasRequiredItems()
		{
			for(int i = 0;i < m_BuildingHelpers.CurrentPreviewPiece.RequiredItems.Length;i ++)
			{
				var requiredItem = m_BuildingHelpers.CurrentPreviewPiece.RequiredItems[i];
				if(m_InventoryContainer.GetItemCount(requiredItem.Name) < requiredItem.Amount)
					return false;
			}

			return true;
		}
		
        private void Update()
        {
            m_BuildingHelpers.HasSocket = false;

            if (m_BuildingHelpers.PreviewExists())
                m_BuildingHelpers.LookForSnaps();

            if (m_BuildingHelpers.PreviewExists())
                m_BuildingHelpers.ManagePreview();
        }

        public bool CanPlace() 
		{
			return m_BuildingHelpers.PlacementAllowed; 
		}
    }
}