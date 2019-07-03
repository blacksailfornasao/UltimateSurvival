using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival.Building
{
	/// <summary>
	/// A collection of helpers, used by the PlayerBuilder to spawn previews, snap, place, etc.
	/// </summary>
    [System.Serializable]
    public class BuildingHelpers
    {
        public BuildingPiece CurrentPreviewPiece { get { return m_CurrentPreviewPiece; } }

        public bool HasSocket { get { return m_HasSocket; } set { m_HasSocket = value; } }

        public bool PlacementAllowed { get { return m_PlacementAllowed; } }

        public Color PreviewColor { get { return m_PreviewColor; } set { m_PreviewColor = value; } }

        public float RotationOffset { get { return m_RotationOffset; } set { m_RotationOffset = value; } }

        [SerializeField]
        private LayerMask m_BuildingPieceMask;

        [SerializeField]
        private LayerMask m_FreePlacementMask;

        [SerializeField]
        private int m_BuildRange;

        [Header("Preview Pulsing Effect")]

        [SerializeField]
        private bool m_UsePulseEffect = true;

        [SerializeField]
        private bool m_PulseWhenSnapped = true;

        [SerializeField]
        private float m_PulseEffectDuration = 2f;

        [SerializeField]
        private float m_PulseMin = 0.4f;

        [SerializeField]
        private float m_PulseMax = 0.9f;

        private BuildingPiece m_CurrentPreviewPiece;
        //private BuildingPiece m_LastPlacedPiece;

        private GameObject m_CurrentPreview;
       // private Collider[] m_LastOverlappingPieces;
	
        private Socket m_LastValidSocket;

        private bool m_HasSocket;
        private bool m_PlacementAllowed = true;

        private Color m_PreviewColor;
        private float m_RotationOffset;

        private Transform m_Transform;
        private AlphaPulse m_Pulse;

        //private PlayerEventHandler m_Player;
       // private AudioSource m_Audio;

		private BuildingPiece m_CurrentPrefab;


        public void Initialize(Transform t, PlayerEventHandler pl, AudioSource aS)
        {
            m_Transform = t;
           // m_Player = pl;
            //m_Audio = aS;

           m_Pulse = new AlphaPulse(m_PreviewColor, m_PulseMin, m_PulseMax);
        }

        public void ManagePreview()
        {
            ManageCollision();

            if(m_UsePulseEffect)
                ApplyPulse();

			var renderers = m_CurrentPreviewPiece.Renderers;

			for(int r = 0;r < renderers.Count;r ++)
			{
				Material[] newMats = renderers[r].materials;
				for (int m = 0; m < newMats.Length; m++)
					newMats[m].color = m_PreviewColor;

				renderers[r].materials = newMats;
			} 
        }

        private void ManageCollision()
        {
			m_PlacementAllowed = true;

			if(m_CurrentPreviewPiece.TerrainProtection || m_CurrentPreviewPiece.RequiresSockets)
			{
				bool overlapsStuff = m_CurrentPreviewPiece.IsBlockedByTerrain();

				//Debug.Log("overlaps terrain: " + overlapsStuff);

				if(!overlapsStuff)
				{
					Collider[] overlappingStuff = Physics.OverlapBox(m_CurrentPreviewPiece.Bounds.center, m_CurrentPreviewPiece.Bounds.extents, m_CurrentPreviewPiece.transform.rotation, m_FreePlacementMask, QueryTriggerInteraction.Ignore);

					for (int o = 0; o < overlappingStuff.Length; o ++)
					{
						if(!m_CurrentPreviewPiece.HasCollider(overlappingStuff[o]))
						{
							var terrainCollider = overlappingStuff[o] as TerrainCollider;

							if(terrainCollider == null)
							{
								var piece = overlappingStuff[o].GetComponent<BuildingPiece>();

								bool isSameBuilding = piece && m_HasSocket && piece.Building == m_LastValidSocket.Piece.Building;

								if(!isSameBuilding)
								{
									overlapsStuff = true;
									break;
								}
							}
						}
					}
				}

				if(m_HasSocket)
				{
					//bool isStable = m_CurrentPreviewPiece.DoStabilityCheck(false);
					m_PlacementAllowed = /*isStable && */!overlapsStuff;
				}
				else
					m_PlacementAllowed = !m_CurrentPreviewPiece.RequiresSockets && !overlapsStuff;
			}

			UpdatePreviewColor();
        }

		private void UpdatePreviewColor()
        {
			Color c = m_PlacementAllowed ? new Color(0, 1, 0, m_PreviewColor.a) : new Color(1, 0, 0, m_PreviewColor.a);

            m_PreviewColor = c;
        }

        private void ApplyPulse()
        {
            if (!m_PulseWhenSnapped && m_HasSocket)
            {
                m_PreviewColor.a = 1;

                return;
            }

            m_Pulse.StartPulse(m_PulseEffectDuration);

            m_PreviewColor.a = m_Pulse.UpdatePulse();
        }

        public void LookForSnaps()
        {
            m_CurrentPreview.gameObject.SetActive(GameController.LocalPlayer.CanShowObjectPreview.Get());

            Collider[] buildingPieces = Physics.OverlapSphere(m_Transform.position, m_BuildRange, m_BuildingPieceMask, QueryTriggerInteraction.Ignore);

            if(buildingPieces.Length > 0)
                HandleSnapPreview(buildingPieces);
            else if(!RaycastAndPlace())
                HandleFreePreview();
        }

        private void HandleFreePreview()
        {
            Transform toCurrentPos = (m_CurrentPreviewPiece.OutOfGroundHeight == 0) ? m_Transform : GameController.WorldCamera.transform;
            Vector3 currentPos = toCurrentPos.position + toCurrentPos.forward * m_BuildRange;

            if (m_CurrentPreviewPiece.OutOfGroundHeight == 0)
            {
                RaycastHit hit;
                Vector3 startPos = m_CurrentPreview.transform.position + new Vector3(0, 0.25f, 0);

                bool raycast = Physics.Raycast(startPos, Vector3.down, out hit, 1f, m_FreePlacementMask, QueryTriggerInteraction.Ignore);

                if (raycast)
                    currentPos.y = hit.point.y;
            }
            else
            {
                float minMove = m_CurrentPreviewPiece.AllowUnderTerrainMovement ? (m_Transform.position.y - m_CurrentPreviewPiece.OutOfGroundHeight) : 0;

                currentPos.y = Mathf.Clamp(currentPos.y, minMove, (m_Transform.position.y + m_CurrentPreviewPiece.OutOfGroundHeight));
            }

            m_CurrentPreview.transform.position = currentPos;
			m_CurrentPreview.transform.rotation = m_Transform.rotation * m_CurrentPrefab.transform.localRotation * Quaternion.Euler(m_CurrentPreviewPiece.RotationAxis * m_RotationOffset);

			m_LastValidSocket = null;
			m_HasSocket = false;
        }

        private void HandleSnapPreview(Collider[] buildingPieces)
        {
           // m_LastOverlappingPieces = buildingPieces;

            Camera cam = Camera.main;
            Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

			float smallestAngleToSocket = Mathf.Infinity;
			Socket targetSocket = null;

            for(int bp = 0; bp < buildingPieces.Length; bp++)
            {
                BuildingPiece buildingPiece = buildingPieces[bp].GetComponent<BuildingPiece>();
				if(buildingPiece == null || buildingPiece.Sockets.Length == 0)
					continue;

                for (int s = 0; s < buildingPiece.Sockets.Length; s++)
                {
                    Socket socket = buildingPiece.Sockets[s];

					if(socket.SupportsPiece(m_CurrentPreviewPiece))
                    {
                        if((socket.transform.position - m_Transform.position).sqrMagnitude < Mathf.Pow(m_BuildRange, 2))
                        {
							float angleToSocket = Vector3.Angle(ray.direction, socket.transform.position - ray.origin);

							if(angleToSocket < smallestAngleToSocket && angleToSocket < 35f)
							{
								smallestAngleToSocket = angleToSocket;
								targetSocket = socket;
							}
                        }
                    }
                }
            }

			if(targetSocket != null)
			{
				Socket.PieceOffset pieceOffset;
				if(targetSocket.GetPieceOffsetByName(m_CurrentPrefab.Name, out pieceOffset))
				{                              
					m_CurrentPreview.transform.position = targetSocket.transform.position + targetSocket.transform.TransformVector(pieceOffset.PositionOffset);
					m_CurrentPreview.transform.rotation = targetSocket.transform.rotation * pieceOffset.RotationOffset;
					m_LastValidSocket = targetSocket;
					m_HasSocket = true;

					return;
				}
			}

            if(!RaycastAndPlace())
                HandleFreePreview();
        }

        private bool RaycastAndPlace()
        {
            Camera cam = Camera.main;
            Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo, m_BuildRange, m_FreePlacementMask, QueryTriggerInteraction.Ignore))
            {
                //if(!m_CurrentPreviewPiece.RequiresSockets)
                //{
                    m_CurrentPreview.transform.position = hitInfo.point;
					m_CurrentPreview.transform.rotation = m_Transform.rotation * m_CurrentPrefab.transform.localRotation * Quaternion.Euler(m_CurrentPreviewPiece.RotationAxis * m_RotationOffset);

                    return true;
                //}
            }

            return false;
        }

        public void SpawnPreview(GameObject prefab)
        {
            m_CurrentPreview = GameObject.Instantiate(prefab);
			m_CurrentPreview.transform.position = Vector3.one * 10000f;
            //m_CurrentPreview.transform.SetParent(m_Transform);

            m_CurrentPreviewPiece = m_CurrentPreview.GetComponent<BuildingPiece>();
            m_CurrentPreviewPiece.SetState(PieceState.Preview);

			m_CurrentPrefab = prefab.GetComponent<BuildingPiece>();
        }

        public void PlacePiece()
        {
			if(m_CurrentPreview == null)
				return;

			GameObject placedPiece = GameObject.Instantiate(m_CurrentPrefab.gameObject, m_CurrentPreview.transform.position, m_CurrentPreview.transform.rotation);
            placedPiece.transform.SetParent(null);

            BuildingPiece piece = placedPiece.GetComponent<BuildingPiece>();
			piece.SetState(PieceState.Preview);
			bool isFreeObject = !piece.RequiresSockets && !piece.TerrainProtection;

			if(isFreeObject)
			{
				//Debug.Log("Is free object!");
				bool placed = false;

				Collider[] overlappingStuff = Physics.OverlapBox(m_CurrentPreviewPiece.Bounds.center, m_CurrentPreviewPiece.Bounds.size / 2f, m_CurrentPreviewPiece.transform.rotation, m_FreePlacementMask, QueryTriggerInteraction.Ignore);
				/*Debug.Log(m_CurrentPreviewPiece.Bounds.size);
				var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.transform.position = CurrentPreviewPiece.Bounds.center;
				cube.transform.rotation = CurrentPreviewPiece.transform.rotation;
				cube.transform.localScale = CurrentPreviewPiece.Bounds.size;*/

				foreach(var col in overlappingStuff)
				{
					var bp = col.GetComponent<BuildingPiece>();
					if(bp != null && bp != m_CurrentPreviewPiece)
					{
						piece.transform.SetParent(bp.Building.transform, true);

						piece.AttachedOn = bp;
						piece.SetState(PieceState.Placed);

						piece.Building = bp.Building;
						piece.Building.AddPiece(piece);

						//Debug.Log("attached on something");
						placed = true;
						break;
					}
				}

				if(!placed)
				{
					var newBuilding = new GameObject("Building", typeof(BuildingHolder));

					piece.transform.SetParent(newBuilding.transform, true);
					piece.Building = newBuilding.GetComponent<BuildingHolder>();

					piece.Building.AddPiece(piece);

					Collider[] colliders;
					if(piece.HasSupport(out colliders))
					{
						if (colliders.Length > 0) 
						{
							var colPiece = colliders [0].GetComponent<BuildingPiece> ();

							if (colPiece != null)
								piece.AttachedOn = colPiece;
						}
					}

					piece.SetState(PieceState.Placed);
				}
			}
			else
			{
				if(m_LastValidSocket && m_LastValidSocket.Piece.Building != null)
	            {
					//m_LastPlacedPiece = piece;
					piece.transform.SetParent(m_LastValidSocket.Piece.Building.transform, true);

					piece.AttachedOn = m_LastValidSocket.Piece;
					piece.SetState(PieceState.Placed);

					m_LastValidSocket.OccupyNeighbours(m_FreePlacementMask, m_BuildingPieceMask, piece);

					piece.Building = m_LastValidSocket.Piece.Building;
					piece.Building.AddPiece(piece);
	            }
				else
				{
					var newBuilding = new GameObject("Building", typeof(BuildingHolder));

					piece.transform.SetParent(newBuilding.transform, true);
					piece.Building = newBuilding.GetComponent<BuildingHolder>();

					piece.Building.AddPiece(piece);

					Collider[] colliders;
					if(piece.HasSupport(out colliders))
					{
						if (colliders.Length > 0) 
						{
							var colPiece = colliders [0].GetComponent<BuildingPiece> ();

							if (colPiece != null)
								piece.AttachedOn = colPiece;
						}
					}

					piece.SetState(PieceState.Placed);
				}
			}

            m_RotationOffset = 0f;

            if (piece.PlacementFX)
                GameObject.Instantiate(piece.PlacementFX, piece.transform.position, piece.transform.rotation);

			m_LastValidSocket = null;
			m_HasSocket = false;
        }

        private bool IntersectsSocket(Ray ray, Socket socket)
        {
            Vector3 L = socket.transform.position - ray.origin;
            float tca = Vector3.Dot(L, ray.direction);

            if (tca < 0f)
                return false;

            float d2 = Vector3.Dot(L, L) - tca * tca;
            if (d2 > socket.Radius * socket.Radius)
                return false;

            return true;
        }

        public void ClearPreview()
        {
            if (m_CurrentPreview != null)
            {
                GameObject.Destroy(m_CurrentPreview.gameObject);

                m_CurrentPreview = null;
                m_CurrentPreviewPiece = null;
            }
        }

        public bool PreviewExists()
        {
            if (m_CurrentPreview)
                return true;

            return false;
        }
    }
}