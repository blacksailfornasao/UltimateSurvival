using System;
using UnityEngine;

namespace UltimateSurvival
{
	[AttributeUsage (AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class ReorderableAttribute : PropertyAttribute
	{
		RangeAttribute g;
	}
}
