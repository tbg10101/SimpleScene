using System;

namespace UnityEngine {
	// ReSharper disable once InconsistentNaming
	public struct Vector2_d : IEquatable<Vector2_d> {
		public const double kEpsilon = 1E-05d;

		public static Vector2_d zero {
			get {
				return new Vector2_d();
			}
		}

		public static Vector2_d one {
			get {
				return new Vector2_d(1.0);
			}
		}

		public static Vector2_d up {
			get {
				return new Vector2_d(0.0, 1.0);
			}
		}

		public static Vector2_d right {
			get {
				return new Vector2_d(1.0, 0.0);
			}
		}

		public double x;
		public double y;

		public double this [int index] {
			get {
				switch (index) {
					case 0:
						return x;
					case 1:
						return y;
					default:
						throw new IndexOutOfRangeException("Invalid Vector2d index!");
				}
			}
			set {
				switch (index) {
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid Vector2d index!");
				}
			}
		}

		public Vector2_d normalized {
			get {
				Vector2_d n = this;
				n.Normalize();
				return n;
			}
		}

		public double magnitude {
			get {
				return Math_d.Sqrt(x * x + y * y);
			}
		}

		public double sqrMagnitude {
			get {
				return x * x + y * y;
			}
		}

		public double largestComponent {
			get {
				return Math_d.Max(x, y);
			}
		}

		public Vector2_d xComponentVector {
			get {
				return new Vector2_d(x, 0);
			}
		}

		public Vector2_d yComponentVector {
			get {
				return new Vector2_d(0, y);
			}
		}

		public Vector2_d (double x, double y) {
			this.x = x;
			this.y = y;
		}

		public Vector2_d (double x) {
			this.x = x;
			y = x;
		}

		public Vector2_d (Vector2_d v2) {
			x = v2.x;
			y = v2.y;
		}

		public Vector2_d (Vector2 v2) {
			x = v2.x;
			y = v2.y;
		}

		public static implicit operator Vector2_d (Vector2 v2) {
			return new Vector2_d(v2);
		}

		public static implicit operator Vector2_d (Vector3_d v3) {
			return new Vector2_d(v3.x, v3.y);
		}

		public static Vector2_d operator + (Vector2_d a, Vector2_d b) {
			return new Vector2_d(a.x + b.x, a.y + b.y);
		}

		public static Vector2_d operator - (Vector2_d a, Vector2_d b) {
			return new Vector2_d(a.x - b.x, a.y - b.y);
		}

		public static Vector2_d operator - (Vector2_d a) {
			return new Vector2_d(-a.x, -a.y);
		}

		public static Vector2_d operator * (Vector2_d a, double d) {
			return new Vector2_d(a.x * d, a.y * d);
		}

		public static Vector2_d operator * (float d, Vector2_d a) {
			return new Vector2_d(a.x * d, a.y * d);
		}

		public static Vector2_d operator * (Vector2_d a, Vector2 b) {
			return new Vector2_d(a.x * b.x, a.y * b.y);
		}

		public static Vector2_d operator / (Vector2_d a, double d) {
			return new Vector2_d(a.x / d, a.y / d);
		}

		public static bool operator == (Vector2_d lhs, Vector2_d rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector2_d lhs, Vector2_d rhs) {
			return !lhs.Equals(rhs);
		}

		public void Set (double new_x, double new_y) {
			x = new_x;
			y = new_y;
		}

		public void Scale (Vector2_d a) {
			x *= a.x;
			y *= a.y;
		}

		public void Scale (double d) {
			x *= d;
			y *= d;
		}

		public void Zero () {
			x = 0;
			y = 0;
		}

		public void Normalize () {
			double num = magnitude;
			if (num > 9.99999974737875E-06)
				Scale(1.0 / num);
			else
				Zero();
		}

		public static Vector2_d Lerp (Vector2_d from, Vector2_d to, double t) {
			t = Math_d.Clamp01(t);
			return new Vector2_d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t);
		}

		public static Vector2_d MoveTowards (Vector2_d current, Vector2_d target, double maxDistanceDelta) {
			Vector2_d vector2 = target - current;
			double magnitude = vector2.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude.Equals(0.0))
				return target;
			return current + vector2 / magnitude * maxDistanceDelta;
		}

		public static double Dot (Vector2_d lhs, Vector2_d rhs) {
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		public static double Angle (Vector2_d from, Vector2_d to) {
			return Math_d.Acos(Math_d.Clamp(Dot(from.normalized, to.normalized), -1.0, 1.0)) * 57.29578;
		}

		public static double Distance (Vector2_d a, Vector2_d b) {
			return (a - b).magnitude;
		}

		public static double SqrDistance (Vector3_d a, Vector3_d b) {
			return (a - b).sqrMagnitude;
		}

		public static Vector2_d ClampMagnitude (Vector2_d vector, double maxLength) {
			if (vector.sqrMagnitude > maxLength * maxLength)
				return vector.normalized * maxLength;
			return vector;
		}

		public static Vector2_d Min (Vector2_d lhs, Vector2_d rhs) {
			return new Vector2_d(Math_d.Min(lhs.x, rhs.x), Math_d.Min(lhs.y, rhs.y));
		}

		public static Vector2_d Max (Vector2_d lhs, Vector2_d rhs) {
			return new Vector2_d(Math_d.Max(lhs.x, rhs.x), Math_d.Max(lhs.y, rhs.y));
		}

		public override int GetHashCode () {
			return x.GetHashCode() ^ y.GetHashCode() << 2;
		}

		public override bool Equals (object other) {
			return (other is Vector2_d) && Equals((Vector2_d) other);
		}

		// non-boxing version
		public bool Equals (Vector2_d vector2d) {
			return x.Equals(vector2d.x) && y.Equals(vector2d.y);
		}

		public override string ToString () {
			return string.Format("({0:F2}, {1:F2})", x, y);
		}

		public string ToString (string format) {
			return string.Format("({0}, {1})", x.ToString(format), y.ToString(format));
		}
	}
}
