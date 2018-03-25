using System;

namespace UnityEngine {
	[Serializable]
	// ReSharper disable once InconsistentNaming
	public struct Vector4_d : IEquatable<Vector4_d> {
		public const float kEpsilon = 1E-05f;

		public static Vector4_d zero {
			get {
				return new Vector4_d();
			}
		}

		public static Vector4_d one {
			get {
				return new Vector4_d(1.0);
			}
		}

		public double x;
		public double y;
		public double z;
		public double w;

		public double this [int index] {
			get {
				switch (index) {
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					case 3:
						return w;
					default:
						throw new IndexOutOfRangeException("Invalid index!");
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
					case 2:
						z = value;
						break;
					case 3:
						w = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid Vector4d index!");
				}
			}
		}

		public Vector4_d normalized {
			get {
				Vector4_d n = this;
				n.Normalize();
				return n;
			}
		}

		public double magnitude {
			get {
				return Math_d.Sqrt(Dot(this, this));
			}
		}

		public double sqrMagnitude {
			get {
				return Dot(this, this);
			}
		}

		public double largestComponent {
			get {
				return Math_d.Max(x, Math_d.Max(y, Math_d.Max(z, w)));
			}
		}

		public Vector4_d xComponentVector {
			get {
				return new Vector4_d(x, 0, 0, 0);
			}
		}

		public Vector4_d yComponentVector {
			get {
				return new Vector4_d(0, y, 0, 0);
			}
		}

		public Vector4_d zComponentVector {
			get {
				return new Vector4_d(0, 0, z, 0);
			}
		}

		public Vector4_d wComponentVector {
			get {
				return new Vector4_d(0, 0, 0, w);
			}
		}

		public Vector4_d (double x, double y, double z, double w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4_d (double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
			w = 0.0;
		}

		public Vector4_d (double x, double y) {
			this.x = x;
			this.y = y;
			z = 0.0;
			w = 0.0;
		}

		public Vector4_d (double x) {
			this.x = x;
			y = x;
			z = x;
			w = x;
		}

		public Vector4_d (Vector4_d v4) {
			x = v4.x;
			y = v4.y;
			z = v4.z;
			w = v4.w;
		}

		public Vector4_d (Vector3_d v3) {
			x = v3.x;
			y = v3.y;
			z = v3.z;
			w = 0.0;
		}

		public Vector4_d (Vector2_d v2) {
			x = v2.x;
			y = v2.y;
			z = 0.0;
			w = 0.0;
		}

		public Vector4_d (Vector4 v4) {
			x = v4.x;
			y = v4.y;
			z = v4.z;
			w = v4.w;
		}

		public Vector4_d (Vector3 v3) {
			x = v3.x;
			y = v3.y;
			z = v3.z;
			w = 0.0;
		}

		public Vector4_d (Vector2 v2) {
			x = v2.x;
			y = v2.y;
			z = 0.0;
			w = 0.0;
		}

		public static implicit operator Vector4_d (Vector4 v4) {
			return new Vector4_d(v4);
		}

		public static implicit operator Vector4_d (Vector3 v3) {
			return new Vector4_d(v3);
		}

		public static implicit operator Vector4_d (Vector2 v2) {
			return new Vector4_d(v2);
		}

		public static Vector4_d operator + (Vector4_d a, Vector4_d b) {
			return new Vector4_d(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4_d operator + (Vector4_d a, Vector4 b) {
			return new Vector4_d(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4_d operator - (Vector4_d a, Vector4_d b) {
			return new Vector4_d(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vector4_d operator - (Vector4_d a, Vector4 b) {
			return new Vector4_d(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vector4_d operator - (Vector4_d a) {
			return new Vector4_d(-a.x, -a.y, -a.z, -a.w);
		}

		public static Vector4_d operator * (Vector4_d a, double d) {
			return new Vector4_d(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Vector4_d operator * (double d, Vector4_d a) {
			return new Vector4_d(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Vector4_d operator / (Vector4_d a, double d) {
			return new Vector4_d(a.x / d, a.y / d, a.z / d, a.w / d);
		}

		public static bool operator == (Vector4_d lhs, Vector4_d rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector4_d lhs, Vector4_d rhs) {
			return !lhs.Equals(rhs);
		}

		public static explicit operator Vector4 (Vector4_d vector4d) {
			return new Vector4((float) vector4d.x, (float) vector4d.y, (float) vector4d.z, (float) vector4d.w);
		}

		public void Set (double new_x, double new_y, double new_z, double new_w) {
			x = new_x;
			y = new_y;
			z = new_z;
			w = new_w;
		}

		public void Scale (double d) {
			x *= d;
			y *= d;
			z *= d;
			w *= d;
		}

		public void Scale (Vector4_d a) {
			x *= a.x;
			y *= a.y;
			z *= a.z;
			w *= a.w;
		}

		public void Zero () {
			x = 0;
			y = 0;
			z = 0;
			w = 0;
		}

		public void Normalize () {
			double num = magnitude;
			if (num > 9.99999974737875E-06)
				Scale(1.0 / num);
			else
				Zero();
		}

		public static Vector4_d Lerp (Vector4_d from, Vector4_d to, double t) {
			t = Math_d.Clamp01(t);
			return new Vector4_d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t, from.w + (to.w - from.w) * t);
		}

		public static Vector4_d MoveTowards (Vector4_d current, Vector4_d target, float maxDistanceDelta) {
			Vector4_d vector4d = target - current;
			double magnitude = vector4d.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == 0.0)
				return target;
			return current + vector4d / magnitude * maxDistanceDelta;
		}

		public static double Dot (Vector4_d a, Vector4_d b) {
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public static double Distance (Vector4_d a, Vector4_d b) {
			return (a - b).magnitude;
		}

		public static double SqrDistance (Vector4_d a, Vector4_d b) {
			return (a - b).sqrMagnitude;
		}

		public static Vector4_d Min (Vector4_d lhs, Vector4_d rhs) {
			return new Vector4_d(Math_d.Min(lhs.x, rhs.x), Math_d.Min(lhs.y, rhs.y), Math_d.Min(lhs.z, rhs.z), Math_d.Min(lhs.w, rhs.w));
		}

		public static Vector4_d Max (Vector4_d lhs, Vector4_d rhs) {
			return new Vector4_d(Math_d.Max(lhs.x, rhs.x), Math_d.Max(lhs.y, rhs.y), Math_d.Max(lhs.z, rhs.z), Math_d.Max(lhs.w, rhs.w));
		}

		public static Vector4_d Project (Vector4_d a, Vector4_d b) {
			return b * Dot(a, b) / Dot(b, b);
		}

		public override int GetHashCode () {
			return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
		}

		public override bool Equals (object other) {
			return (other is Vector4_d) && Equals((Vector4_d) other);
		}

		// non-boxing version
		public bool Equals (Vector4_d vector4d) {
			return x.Equals(vector4d.x) && y.Equals(vector4d.y) && z.Equals(vector4d.z) && w.Equals(vector4d.w);
		}

		public override string ToString () {
			return string.Format("({0:F2}, {1:F2}, {3:F2})", x, y, z, w);
		}

		public string ToString (string format) {
			return string.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
		}
	}
}
