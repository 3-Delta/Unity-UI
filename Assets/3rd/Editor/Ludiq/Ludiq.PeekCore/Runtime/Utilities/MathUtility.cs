using System.Collections.Generic;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class MathUtility
	{
		public static int NegativeFallback(int i, int fallback)
		{
			return i >= 0 ? i : fallback;
		}

		public static int? NegativeNull(int? i)
		{
			return i >= 0 ? i : null;
		}

		public static bool LessThanOrApproximately(this float a, float b)
		{
			return a < b || Mathf.Approximately(a, b);
		}

		public static bool GreaterThanOrApproximately(this float a, float b)
		{
			return a > b || Mathf.Approximately(a, b);
		}

		public static float Normalized(this float input)
		{
			if (input == 0)
			{
				return 0;
			}

			return input / Mathf.Abs(input);
		}

		public static Vector2 Abs(this Vector2 input)
		{
			return new Vector2(Mathf.Abs(input.x), Mathf.Abs(input.y));
		}

		public static Vector3 Abs(this Vector3 input)
		{
			return new Vector3(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));
		}

		public static Vector4 Abs(this Vector4 input)
		{
			return new Vector4(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z), Mathf.Abs(input.w));
		}

		public static Vector2 Multiply(this Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		public static Vector3 Multiply(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector4 Multiply(this Vector4 a, Vector4 b)
		{
			return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		public static Vector2 Divide(this Vector2 a, Vector2 b)
		{
			return new Vector2(a.x / b.x, a.y / b.y);
		}

		public static Vector3 Divide(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static Vector4 Divide(this Vector4 a, Vector4 b)
		{
			return new Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}

		public static Vector2 Modulo(this Vector2 a, Vector2 b)
		{
			return new Vector2(a.x % b.x, a.y % b.y);
		}

		public static Vector3 Modulo(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x % b.x, a.y % b.y, a.z % b.z);
		}

		public static Vector4 Modulo(this Vector4 a, Vector4 b)
		{
			return new Vector4(a.x % b.x, a.y % b.y, a.z % b.z, a.w % b.w);
		}

		public static Vector2 Min(params Vector2[] values) => Min((IEnumerable<Vector2>)values);
		public static Vector2 Min(params Vector3[] values) => Min((IEnumerable<Vector3>)values);
		public static Vector2 Min(params Vector4[] values) => Min((IEnumerable<Vector4>)values);

		public static Vector2 Min(IEnumerable<Vector2> values)
		{
			var first = true;
			var result = Vector2.zero;

			foreach (var value in values)
			{
				if (first)
				{
					result = value;
					first = false;
				}
				else
				{
					result = Vector2.Min(result, value);
				}
			}

			return result;
		}

		public static Vector3 Min(IEnumerable<Vector3> values)
		{
			var first = true;
			var result = Vector3.zero;

			foreach (var value in values)
			{
				if (first)
				{
					result = value;
					first = false;
				}
				else
				{
					result = Vector3.Min(result, value);
				}
			}

			return result;
		}

		public static Vector4 Min(IEnumerable<Vector4> values)
		{
			var first = true;
			var result = Vector4.zero;

			foreach (var value in values)
			{
				if (first)
				{
					result = value;
					first = false;
				}
				else
				{
					result = Vector4.Min(result, value);
				}
			}

			return result;
		}

		public static Vector2 Max(params Vector2[] values) => Max((IEnumerable<Vector2>)values);
		public static Vector3 Max(params Vector3[] values) => Max((IEnumerable<Vector3>)values);
		public static Vector4 Max(params Vector4[] values) => Max((IEnumerable<Vector4>)values);

		public static Vector2 Max(IEnumerable<Vector2> values)
		{
			var first = true;
			var result = Vector2.zero;

			foreach (var value in values)
			{
				if (first)
				{
					result = value;
					first = false;
				}
				else
				{
					result = Vector2.Max(result, value);
				}
			}

			return result;
		}

		public static Vector3 Max(IEnumerable<Vector3> values)
		{
			var first = true;
			var result = Vector3.zero;

			foreach (var value in values)
			{
				if (first)
				{
					result = value;
					first = false;
				}
				else
				{
					result = Vector3.Max(result, value);
				}
			}

			return result;
		}

		public static Vector4 Max(IEnumerable<Vector4> values)
		{
			var first = true;
			var result = Vector4.zero;

			foreach (var value in values)
			{
				if (first)
				{
					result = value;
					first = false;
				}
				else
				{
					result = Vector4.Max(result, value);
				}
			}

			return result;
		}

		public static Vector2 Project(this Vector2 a, Vector2 b)
		{
			return Vector2.Dot(a, b) * b.normalized;
		}

		public static Vector2 Floor(this Vector2 input)
		{
			return new Vector2(Mathf.Floor(input.x), Mathf.Floor(input.y));
		}

		public static Vector3 Floor(this Vector3 input)
		{
			return new Vector3(Mathf.Floor(input.x), Mathf.Floor(input.y), Mathf.Floor(input.z));
		}

		public static Vector4 Floor(this Vector4 input)
		{
			return new Vector4(Mathf.Floor(input.x), Mathf.Floor(input.y), Mathf.Floor(input.z), Mathf.Floor(input.w));
		}

		public static Vector2 Round(this Vector2 input)
		{
			return new Vector2(Mathf.Round(input.x), Mathf.Round(input.y));
		}

		public static Vector3 Round(this Vector3 input)
		{
			return new Vector3(Mathf.Round(input.x), Mathf.Round(input.y), Mathf.Round(input.z));
		}

		public static Vector4 Round(this Vector4 input)
		{
			return new Vector4(Mathf.Round(input.x), Mathf.Round(input.y), Mathf.Round(input.z), Mathf.Round(input.w));
		}

		public static Vector2 Ceil(this Vector2 input)
		{
			return new Vector2(Mathf.Ceil(input.x), Mathf.Ceil(input.y));
		}

		public static Vector3 Ceil(this Vector3 input)
		{
			return new Vector3(Mathf.Ceil(input.x), Mathf.Ceil(input.y), Mathf.Ceil(input.z));
		}

		public static Vector4 Ceil(this Vector4 input)
		{
			return new Vector4(Mathf.Ceil(input.x), Mathf.Ceil(input.y), Mathf.Ceil(input.z), Mathf.Ceil(input.w));
		}
	}
}
