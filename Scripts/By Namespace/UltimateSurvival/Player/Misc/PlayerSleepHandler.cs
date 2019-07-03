using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
	public class PlayerSleepHandler : PlayerBehaviour 
	{
		[SerializeField]
		private float m_SleepSpeed = 0.33f;

		[SerializeField]
		[Tooltip("How much time to wait after the sleep is done, before getting up.")]
		private float m_SleepFinishPause = 2f;

		[SerializeField]
		[Range(4, 12)]
		private int m_GetUpHour = 8;

		[Header("Stuff To Disable When Sleeping")]

		[SerializeField]
		private Collider[] m_CollidersToDisable;

		[SerializeField]
		private GameObject[] m_ObjectsToDisable;

		[SerializeField]
		private Behaviour[] m_BehavioursToDisable;

		private Vector3 m_BeforeSleepPosition;
		private Quaternion m_BeforeSleepRotation;


		private void Awake()
		{
			Player.StartSleeping.SetTryer(Try_StartSleeping);
		}

		private bool Try_StartSleeping(SleepingBag bag)
		{
			if(Player.Sleep.Active)
				return false;

			StartCoroutine(C_Sleep(bag));

			return true;
		}

		private IEnumerator C_Sleep(SleepingBag bag)
		{
			m_BeforeSleepPosition = transform.position;
			m_BeforeSleepRotation = transform.rotation;

			//transform.position = bag.SleepPosition;
			//transform.rotation = bag.SleepRotation;

			EnableStuff(false);

			int hoursToSleep = 0;
			float hoursSlept = 0f;
			float speed = m_SleepSpeed;

			int currentHour = TimeOfDay.Instance.CurrentHour;

			if(currentHour <= 24 && currentHour > 18)
				hoursToSleep = 24 - currentHour + m_GetUpHour;
			else
				hoursToSleep = m_GetUpHour - currentHour;

			while(Mathf.Abs(TimeOfDay.Instance.CurrentHour - m_GetUpHour) > 0)
			{
				TimeOfDay.Instance.NormalizedTime += Time.deltaTime * speed;

				hoursSlept += Time.deltaTime * speed * 24f;
				speed = Mathf.Lerp(m_SleepSpeed, 0f, hoursSlept / hoursToSleep);
				speed = Mathf.Max(speed, 0.001f);

				transform.position = Vector3.Lerp(transform.position, bag.SleepPosition, Time.deltaTime * 10f);
				transform.rotation = Quaternion.Lerp(transform.rotation, bag.SleepRotation, Time.deltaTime * 10f);

				yield return null;
			}

			yield return new WaitForSeconds(m_SleepFinishPause);

			while((transform.position - m_BeforeSleepPosition).sqrMagnitude > 0.0001f && Quaternion.Angle(transform.rotation, m_BeforeSleepRotation) > 0.001f)
			{
				transform.position = Vector3.Lerp(transform.position, m_BeforeSleepPosition, Time.deltaTime * 10f);
				transform.rotation = Quaternion.Lerp(transform.rotation, m_BeforeSleepRotation, Time.deltaTime * 10f);

				yield return null;
			}

			transform.position = m_BeforeSleepPosition;
			transform.rotation = m_BeforeSleepRotation;

			EnableStuff(true);

			Player.LastSleepPosition.Set(bag.SpawnPosOffset);

			Player.Sleep.ForceStop();
		}

		private void EnableStuff(bool enable)
		{
			foreach(var obj in m_ObjectsToDisable)
				obj.SetActive(enable);

			foreach(var behaviour in m_BehavioursToDisable)
				behaviour.enabled = enable;

			foreach(var collider in m_CollidersToDisable)
				collider.enabled = enable;
		}
	}
}
