using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S1.Runtime.Utils
{
	public static class SOneUtils
	{
		public static Vector3 Average(this IEnumerable<Vector3> vectors)
		{
			var vectorArray = vectors as Vector3[] ?? vectors.ToArray();
			var averageVector = vectorArray.Aggregate(Vector3.zero, (current, vector) => current + vector);
			averageVector /= vectorArray.Length;
			return averageVector;
		}

		public static int Average(this IEnumerable<int> ints)
		{
			var intArray = ints as int[] ?? ints.ToArray();
			var averageInt = intArray.Aggregate(0, (current, i) => current + i);
			averageInt /= intArray.Length;
			return averageInt;
		}

		public static float Average(this IEnumerable<float> floats)
		{
			var floatArray = floats as float[] ?? floats.ToArray();
			var averageFloat = floatArray.Aggregate(0f, (current, f) => current + f);
			averageFloat /= floatArray.Length;
			return averageFloat;
		}
	}
}