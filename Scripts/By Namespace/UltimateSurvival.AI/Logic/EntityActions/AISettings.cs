using UnityEngine;

namespace UltimateSurvival.AI
{
	public class AISettings : AIBehaviour
    {
        public EntityMovement Movement { get { return m_Movement; } }

        public EntityDetection Detection { get { return m_Detection; } }

        public EntityVitals Vitals { get { return m_Vitals; } }

        public EntityAnimation Animation { get { return m_Animation; } }

		public AudioSource AudioSource { get { return m_AudioSource; } }

        [SerializeField]
        private EntityMovement m_Movement;

        [SerializeField]
        private EntityDetection m_Detection;

        [SerializeField]
        private EntityVitals m_Vitals;

		[Header("Combat")]

		[SerializeField]
		[Clamp(0f, 500f)]
		private float m_HitDamage = 25f;

		[SerializeField]
		[Clamp(0f, 3f)]
		private float m_MaxAttackDistance = 2f;

		[Header("Audio")]

		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField]
		private SoundPlayer m_AttackSounds;

        private EntityAnimation m_Animation;
        private AIBrain m_Brain;


		public void OnAnimationDamage()
		{
			var entity = m_Detection.LastChasedTarget.GetComponent<EntityEventHandler>();

			bool isClose = Vector3.Distance(entity.transform.position, transform.position) < m_MaxAttackDistance;
			bool isFacingTarget = Vector3.Angle(m_Detection.LastChasedTarget.transform.position - transform.position, transform.forward) < 60f;

			if(entity != null && isClose && isFacingTarget)
			{
				entity.ChangeHealth.Try(new HealthEventData(-m_HitDamage, Entity, transform.position + Vector3.up + transform.forward * 0.5f, entity.transform.position - transform.position));

				var col = entity.GetComponent<Collider>();
				if(col != null)
				{
					var data = SurfaceDatabase.Instance.GetSurfaceData(col, entity.transform.position + Vector3.up, 0);
					if(data != null)
						data.PlaySound(ItemSelectionMethod.RandomlyButExcludeLast, SoundType.Hit, 1f, entity.transform.position + Vector3.up * 1.5f);
				}
			}
		}

		public void PlayAttackSounds()
		{
			m_AttackSounds.Play(ItemSelectionMethod.RandomlyButExcludeLast, m_AudioSource, 1f);
		}

        private void Start()
        {
            m_Brain = GetComponent<AIBrain>();

            m_Movement.Initialize(m_Brain);
            m_Detection.Initialize(transform);

            m_Animation = new EntityAnimation();
            m_Animation.Initialize(m_Brain);
        }

        private void Update()
        {
            m_Movement.Update(transform);

            m_Detection.Update(m_Brain);

            m_Vitals.Update(m_Brain);
        }
    }
}