using System;
using UnityEngine;

namespace UltimateSurvival
{
	public class ItemProperty
	{
		public enum Type
		{
			None,
			Bool,
			Int,
			IntRange,
			RandomInt,
			Float,
			FloatRange,
			RandomFloat,
			String,
			Sound,
		}

		[Serializable]
		public struct Int
		{
			public int Current { get { return m_Current; } set { m_Current = value; } }

			/// <summary>The default value, that was set up initially, when the item was defined.</summary>
			public int Default { get { return m_Default; } }

			/// <summary>This is equal to Current / Default.</summary>
			public float Ratio { get { return (float)m_Current / m_Default; } }

			[SerializeField]
			private int m_Current;

			[SerializeField]
			private int m_Default;


			public override string ToString()
			{
				return m_Current.ToString();
			}
		}

		[Serializable]
		public struct IntRange
		{
			public int Current { get { return m_Current; } set { m_Current = Mathf.Clamp(value, m_Min, m_Max); } }

			/// <summary>This is equal to Current / Max.</summary>
			public float Ratio { get { return (float)m_Current / m_Max; } }

			public int Min { get { return m_Min; } set { m_Min = value; } }

			public int Max { get { return m_Max; } set { m_Max = value; } }

			[SerializeField]
			private int m_Current, m_Min, m_Max;


			public override string ToString()
			{
				return string.Format("{0} / {1}", Current, Max);
			}
		}

		[Serializable]
		public struct RandomInt
		{
			public int RandomValue { get { return UnityEngine.Random.Range(m_Min, m_Max); } }

			[SerializeField]
			private int m_Min;

			[SerializeField]
			private int m_Max;


			public override string ToString()
			{
				return string.Format("{0} - {1}", m_Min, m_Max);
			}
		}

		[Serializable]
		public struct Float
		{
			public float Current { get { return m_Current; } set { m_Current = value; } }

			/// <summary>The default value, that was set up initially, when the item was defined.</summary>
			public float Default { get { return m_Default; } }

			/// <summary>This is equal to Current / Default.</summary>
			public float Ratio { get { return m_Current / m_Default; } }

			[SerializeField]
			private float m_Current;

			[SerializeField]
			private float m_Default;


			public override string ToString()
			{
				return m_Current.ToString();
			}
		}

		[Serializable]
		public struct FloatRange
		{
			public float Current { get { return m_Current; } set { m_Current = Mathf.Clamp(value, m_Min, m_Max); } }

			/// <summary>This is equal to Current / Max.</summary>
			public float Ratio { get { return m_Current / m_Max; } }

			public float Min { get { return m_Min; } set { m_Min = value; } }

			public float Max { get { return m_Max; } set { m_Max = value; } }

			[SerializeField]
			private float m_Current, m_Min, m_Max;


			public override string ToString()
			{
				return string.Format("{0} / {1}", Current, Max);
			}
		}

		[Serializable]
		public struct RandomFloat
		{
			public float RandomValue { get { return UnityEngine.Random.Range(m_Min, m_Max); } }

			[SerializeField]
			private float m_Min;

			[SerializeField]
			private float m_Max;


			public override string ToString()
			{
				return string.Format("{0} - {1}", m_Min, m_Max);
			}
		}
			
		[Serializable]
		public struct Definition
		{
			public string Name { get { return m_Name; } }

			public Type Type { get { return m_Type; } }

			[SerializeField]
			private string m_Name;

			[SerializeField]
			private Type m_Type;
		}

		[Serializable]
		public class Value
		{
			public Message<Value> Changed = new Message<Value>();

			public string Name { get { return m_Name; } }

			public Type Type { get; private set; }

			public bool Bool { get { return m_Bool; } }

			public Int Int { get { return m_Int; } }

			public IntRange IntRange { get { return m_IntRange; } }

			public RandomInt RandomInt { get { return m_RandomInt; } }

			public Float Float { get { return m_Float; } }

			public FloatRange FloatRange { get { return m_FloatRange; } }

			public RandomFloat RandomFloat { get { return m_RandomFloat; } }

			public string String { get { return m_String; } }

			public AudioClip Sound { get { return m_Sound; } }

			[SerializeField]
			private string m_Name;

			[SerializeField]
			private Type m_Type;

			[SerializeField]
			private bool m_Bool;

			[SerializeField]
			private Int m_Int;

			[SerializeField]
			private IntRange m_IntRange;

			[SerializeField]
			private RandomInt m_RandomInt;

			[SerializeField]
			private Float m_Float;

			[SerializeField]
			private FloatRange m_FloatRange;

			[SerializeField]
			private RandomFloat m_RandomFloat;

			[SerializeField]
			private string m_String;

			[SerializeField]
			private AudioClip m_Sound;


			public Value GetClone()
			{
				return (Value)MemberwiseClone();
			}

			public void SetValue(Type type, object value)
			{
				if(type == Type.Bool)
					m_Bool = (bool)value;
				else if(type == Type.Int)
					m_Int = (Int)value;
				else if(type == Type.Float)
					m_Float = (Float)value;
				else if(type == Type.FloatRange)
					m_FloatRange = (FloatRange)value;
				else if(type == Type.IntRange)
					m_IntRange = (IntRange)value;
				else if(type == Type.RandomFloat)
					m_RandomFloat = (RandomFloat)value;
				else if(type == Type.RandomInt)
					m_RandomInt = (RandomInt)value;
				else if(type == Type.String)
					m_String = (string)value;
				else if(type == Type.None)
					return;

				Changed.Send(this);
			}

			public override string ToString()
			{
				if(m_Type == Type.Bool)
					return m_Bool.ToString();
				else if(m_Type == Type.Int)
					return m_Int.ToString();
				else if(m_Type == Type.Float)
					return m_Float.ToString();
				else if(m_Type == Type.FloatRange)
					return m_FloatRange.ToString();
				else if(m_Type == Type.IntRange)
					return m_IntRange.ToString();
				else if(m_Type == Type.RandomFloat)
					return m_RandomFloat.ToString();
				else if(m_Type == Type.RandomInt)
					return m_RandomInt.ToString();
				else if(m_Type == Type.String)
					return m_String;

				return m_Name;
			}
		}
	}
}
