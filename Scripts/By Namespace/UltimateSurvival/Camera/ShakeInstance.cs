using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
    public class ShakeInstance
    {
		/// <summary>If this is true, the rotation on the X axis (up & down) will be only positive (to simulate recoil).</summary>
		public bool IsWeapon { get; set; }

		public float ScaleRoughness { get { return roughMod; } set { roughMod = value; } }

		public float ScaleMagnitude { get { return magnMod; } set { magnMod = value; } }
			
		public float NormalizedFadeTime { get { return currentFadeTime; } }

		bool IsShaking { get { return currentFadeTime > 0 || sustain; } }

		bool IsFadingOut { get { return !sustain && currentFadeTime > 0; } }

		bool IsFadingIn { get { return currentFadeTime < 1 && sustain && fadeInDuration > 0; } }

		public ShakeState CurrentState
		{
			get
			{
				if (IsFadingIn)
					return ShakeState.FadingIn;
				else if (IsFadingOut)
					return ShakeState.FadingOut;
				else if (IsShaking)
					return ShakeState.Sustained;
				else
					return ShakeState.Inactive;
			}
		}
			
        public float Magnitude;
        public float Roughness;
        public Vector3 PositionInfluence;
        public Vector3 RotationInfluence;
        public bool DeleteOnInactive = true;

        float roughMod = 1, magnMod = 1;
        float fadeOutDuration, fadeInDuration;
        bool sustain;
        float currentFadeTime;
        float tick = 0;
        Vector3 amt;


        public ShakeInstance(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
        {
            this.Magnitude = magnitude;
            fadeOutDuration = fadeOutTime;
            fadeInDuration = fadeInTime;
            this.Roughness = roughness;
            if (fadeInTime > 0)
            {
                sustain = true;
                currentFadeTime = 0;
            }
            else
            {
                sustain = false;
                currentFadeTime = 1;
            }

            tick = Random.Range(-100, 100);
        }
			
        public ShakeInstance(float magnitude, float roughness)
        {
            this.Magnitude = magnitude;
            this.Roughness = roughness;
            sustain = true;

            tick = Random.Range(-100, 100);
        }

        public Vector3 UpdateShake()
        {
			if(IsWeapon)
				amt.x = -1f;
			else
				amt.x = Mathf.PerlinNoise(tick, 0) - 0.5f;
			
            amt.y = Mathf.PerlinNoise(0, tick) - 0.5f;
            amt.z = Mathf.PerlinNoise(tick, tick) - 0.5f;

            if (fadeInDuration > 0 && sustain)
            {
                if (currentFadeTime < 1)
					currentFadeTime += Time.fixedDeltaTime / fadeInDuration;
                else if (fadeOutDuration > 0)
                    sustain = false;
            }

            if (!sustain)
				currentFadeTime -= Time.fixedDeltaTime / fadeOutDuration;

            if (sustain)
				tick += Time.fixedDeltaTime * Roughness * roughMod;
            else
				tick += Time.fixedDeltaTime * Roughness * roughMod * currentFadeTime;

            return amt * Magnitude * magnMod * currentFadeTime;
        }

        public void StartFadeOut(float fadeOutTime)
        {
            if (fadeOutTime == 0)
                currentFadeTime = 0;

            fadeOutDuration = fadeOutTime;
            fadeInDuration = 0;
            sustain = false;
        }
			
        public void StartFadeIn(float fadeInTime)
        {
            if (fadeInTime == 0)
                currentFadeTime = 1;

            fadeInDuration = fadeInTime;
            fadeOutDuration = 0;
            sustain = true;
        }
    }

	public enum ShakeState { FadingIn, FadingOut, Sustained, Inactive }
}