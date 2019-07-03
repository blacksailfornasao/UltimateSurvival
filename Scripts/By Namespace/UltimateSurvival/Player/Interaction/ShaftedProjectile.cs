using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class ShaftedProjectile : MonoBehaviour
	{
		[Header("Setup")]

		[SerializeField]
		private Transform m_Model;

		[SerializeField]
		private Transform m_Pivot;

		[SerializeField]
		private LayerMask m_Mask;

		[SerializeField]
		private float m_MaxDistance = 2f;

		[Header("Launch")]

		[SerializeField]
		private float m_LaunchSpeed = 50f;

		[Header("Damage")]

		[SerializeField]
		private float m_MaxDamage = 100f;

		[SerializeField]
		[Tooltip("How the damage changes, when the speed gets lower.")]
		private AnimationCurve m_DamageCurve = new AnimationCurve(
			new Keyframe(0f, 1f),
			new Keyframe(0.8f, 0.5f),
			new Keyframe(1f, 0f));

		[Header("Penetration")]

		[SerializeField]
		[Tooltip("The speed under which the projectile will not penetrate objects.")]
		private float m_PenetrationThreeshold = 20f;

		[SerializeField]
		private float m_PenetrationOffset = 0.2f;

		[SerializeField]
		private Vector2 m_RandomRotation;

		[Header("Twang")]

		[SerializeField]
		private float m_TwangDuration = 1f;

		[SerializeField]
		private float m_TwangRange = 18f;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SoundPlayer m_HitAudio;

		[SerializeField]
		private SoundPlayer m_TwangAudio;

		[SerializeField]
		private SoundType m_PenetrationType;

		private EntityEventHandler m_EntityThatLaunched;
		private Collider m_Collider;
		private Rigidbody m_Rigidbody;
		private bool m_Done;
		private bool m_Launched;


		public void Launch(EntityEventHandler entityThatLaunched)
		{
			if(m_Launched)
			{
				Debug.LogWarningFormat(this, "Already launched this projectile!", name);
				return;
			}

			m_EntityThatLaunched = entityThatLaunched;
			m_Rigidbody.velocity = transform.forward * m_LaunchSpeed;
			m_Launched = true;
		}

		private void Awake()
		{
			m_Collider = GetComponent<Collider>();
			m_Rigidbody = GetComponent<Rigidbody>();

			m_Collider.enabled = false;
		}

		private void FixedUpdate()
		{
			if(m_Done)
				return;

			RaycastHit hitInfo;
			Ray ray = new Ray(transform.position, transform.forward);

			if(Physics.Raycast(ray, out hitInfo, m_MaxDistance, m_Mask, QueryTriggerInteraction.Ignore))
			{
				var data = SurfaceDatabase.Instance.GetSurfaceData(hitInfo);

				float currentSpeed = m_Rigidbody.velocity.magnitude;

				if(currentSpeed >= m_PenetrationThreeshold && data != null)
				{
					data.PlaySound(ItemSelectionMethod.RandomlyButExcludeLast, m_PenetrationType, 1f, m_AudioSource);

					transform.SetParent(hitInfo.collider.transform, true);

					// Stick the projectile in the object.
					transform.position = hitInfo.point + transform.forward * m_PenetrationOffset;
					m_Pivot.localPosition = Vector3.back * m_PenetrationOffset;

					m_Model.SetParent(m_Pivot, true);

					float impulse = m_Rigidbody.mass * currentSpeed;

					var damageable = hitInfo.collider.GetComponent<IDamageable>();

					// If the object is damageable...
					if(damageable != null)
					{
						float damage = m_MaxDamage * m_DamageCurve.Evaluate(1f - currentSpeed / m_LaunchSpeed);

						var damageData = new HealthEventData(-damage, m_EntityThatLaunched, hitInfo.point, ray.direction, impulse);
						damageable.ReceiveDamage(damageData);
					}
					// If the object is not damageable, but it's a rigidbody, apply the impact impulse.
					else if(hitInfo.rigidbody)
						hitInfo.rigidbody.AddForceAtPosition(transform.forward * impulse, transform.position, ForceMode.Impulse);

					StartCoroutine(C_DoTwang());

					var hitbox = hitInfo.collider.GetComponent<HitBox>();
					if(hitbox)
					{
						var cols = Physics.OverlapSphere(transform.position, 1.5f, m_Mask, QueryTriggerInteraction.Ignore);
						for(int i = 0;i < cols.Length;i ++)
						{
							var colHitbox = cols[i].GetComponent<HitBox>();
							if(colHitbox)
								Physics.IgnoreCollision(colHitbox.Collider, m_Collider);
						}
					}
					Physics.IgnoreCollision(m_Collider, hitInfo.collider);
					m_Rigidbody.isKinematic = true;
				}
				else
				{
					m_HitAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);
					m_Rigidbody.isKinematic = false;
				}
					
				m_Collider.enabled = true;
				m_Collider.isTrigger = true;

				m_Done = true;
			}
		}

		private IEnumerator C_DoTwang()
		{
			float stopTime = Time.time + m_TwangDuration;
			float range = m_TwangRange;
			float currentVelocity = 0f;

			Quaternion randomRotation = Quaternion.Euler(new Vector2(
				Random.Range(-m_RandomRotation.x, m_RandomRotation.x), 
				Random.Range(-m_RandomRotation.y, m_RandomRotation.y)));

			while(Time.time < stopTime)
			{
				m_Pivot.localRotation = randomRotation * Quaternion.Euler(Random.Range(-range, range), Random.Range(-range, range), 0f);
				range = Mathf.SmoothDamp(range, 0f, ref currentVelocity, stopTime - Time.time);

				yield return null;
			}
		}
	}
}