using System;

namespace UnityEngine {
	[Serializable]
	// ReSharper disable once InconsistentNaming
	public struct Vector3_d : IEquatable<Vector3_d> {
		public const float kEpsilon = 1E-05f;

		public static Vector3_d zero {
			get {
				return new Vector3_d();
			}
		}

		public static Vector3_d one {
			get {
				return new Vector3_d(1.0);
			}
		}

		public static Vector3_d forward {
			get {
				return new Vector3_d(0.0, 0.0, 1.0);
			}
		}

		public static Vector3_d back {
			get {
				return new Vector3_d(0.0, 0.0, -1.0);
			}
		}

		public static Vector3_d up {
			get {
				return new Vector3_d(0.0, 1.0, 0.0);
			}
		}

		public static Vector3_d down {
			get {
				return new Vector3_d(0.0, -1.0, 0.0);
			}
		}

		public static Vector3_d left {
			get {
				return new Vector3_d(-1.0, 0.0, 0.0);
			}
		}

		public static Vector3_d right {
			get {
				return new Vector3_d(1.0, 0.0, 0.0);
			}
		}

		public double x;
		public double y;
		public double z;

		public double this [int index] {
			get {
				switch (index) {
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
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
					default:
						throw new IndexOutOfRangeException("Invalid Vector3d index!");
				}
			}
		}

		public Vector3_d normalized {
			get {
				Vector3_d n = this;
				n.Normalize();
				return n;
			}
		}

		public double magnitude {
			get {
				return Math_d.Sqrt(x * x + y * y + z * z);
			}
		}

		public double magnitudeFast {
			get {
				return 1.0 / Math_d.InverseSqrtFast(x * x + y * y + z * z);
			}
		}

		public double sqrMagnitude {
			get {
				return x * x + y * y + z * z;
			}
		}

		public double largestComponent {
			get {
				return Math_d.Max(x, Math_d.Max(y, z));
			}
		}

		public Vector3_d xComponentVector {
			get {
				return new Vector3_d(x, 0, 0);
			}
		}

		public Vector3_d yComponentVector {
			get {
				return new Vector3_d(0, y, 0);
			}
		}

		public Vector3_d zComponentVector {
			get {
				return new Vector3_d(0, 0, z);
			}
		}

		public Vector3_d (double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3_d (double x, double y) {
			this.x = x;
			this.y = y;
			z = 0.0;
		}

		public Vector3_d (double x) {
			this.x = x;
			y = x;
			z = x;
		}

		public Vector3_d (float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3_d (float x, float y) {
			this.x = x;
			this.y = y;
			z = 0.0;
		}

		public Vector3_d (float x) {
			this.x = x;
			y = x;
			z = x;
		}

		public Vector3_d (Vector3_d v3) {
			x = v3.x;
			y = v3.y;
			z = v3.z;
		}

		public Vector3_d (Vector2_d v2) {
			x = v2.x;
			y = v2.y;
			z = 0.0;
		}

		public Vector3_d (Vector3 v3) {
			x = v3.x;
			y = v3.y;
			z = v3.z;
		}

		public Vector3_d (Vector2 v2) {
			x = v2.x;
			y = v2.y;
			z = 0.0;
		}

		public static implicit operator Vector3_d (Vector3 v3) {
			return new Vector3_d(v3);
		}

		public static implicit operator Vector3_d (Vector2 v2) {
			return new Vector3_d(v2);
		}

		public static Vector3_d operator + (Vector3_d a, Vector3_d b) {
			return new Vector3_d(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3_d operator + (Vector3_d a, Vector3 b) {
			return new Vector3_d(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3_d operator - (Vector3_d a, Vector3_d b) {
			return new Vector3_d(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3_d operator - (Vector3_d a, Vector3 b) {
			return new Vector3_d(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3_d operator - (Vector3_d a) {
			return new Vector3_d(-a.x, -a.y, -a.z);
		}

		public static Vector3_d operator * (Vector3_d a, double d) {
			return new Vector3_d(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3_d operator * (double d, Vector3_d a) {
			return new Vector3_d(a.x * d, a.y * d, a.z * d);
		}

		public static Vector3_d operator / (Vector3_d a, double d) {
			return new Vector3_d(a.x / d, a.y / d, a.z / d);
		}

		public static bool operator == (Vector3_d lhs, Vector3_d rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator != (Vector3_d lhs, Vector3_d rhs) {
			return !lhs.Equals(rhs);
		}

		public static explicit operator Vector3 (Vector3_d vector3d) {
			return new Vector3((float) vector3d.x, (float) vector3d.y, (float) vector3d.z);
		}

		public Vector3 ToVector3 () {
			return new Vector3((float) x, (float) y, (float) z);
		}

		public Vector3 ToVector3 (float maxMagnitude) {
			if (sqrMagnitude <= maxMagnitude * maxMagnitude) {
				return ToVector3();
			}

			return (normalized * maxMagnitude).ToVector3();
		}

		public void Set (double new_x, double new_y, double new_z) {
			x = new_x;
			y = new_y;
			z = new_z;
		}

		public void Scale (double d) {
			x *= d;
			y *= d;
			z *= d;
		}

		public void Scale (Vector3_d a) {
			x *= a.x;
			y *= a.y;
			z *= a.z;
		}

		public void Zero () {
			x = 0;
			y = 0;
			z = 0;
		}

		public void Normalize () {
			double num = magnitude;
			if (num > 9.99999974737875E-06)
				Scale(1.0 / num);
			else
				Zero();
		}

		public static Vector3_d Lerp (Vector3_d from, Vector3_d to, double t) {
			t = Math_d.Clamp01(t);
			return new Vector3_d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}

		public static Vector3_d MoveTowards (Vector3_d current, Vector3_d target, double maxDistanceDelta) {
			Vector3_d vector3 = target - current;
			double magnitude = vector3.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == 0.0)
				return target;
			return current + vector3 / magnitude * maxDistanceDelta;
		}

		public static double Dot (Vector3_d lhs, Vector3_d rhs) {
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public static double Angle (Vector3_d from, Vector3_d to) {
			return Math_d.Acos(Math_d.Clamp(Dot(from.normalized, to.normalized), -1.0, 1.0)) * 57.29578;
		}

		public static double Distance (Vector3_d a, Vector3_d b) {
			return (a - b).magnitude;
		}

		public static double SqrDistance (Vector3_d a, Vector3_d b) {
			return (a - b).sqrMagnitude;
		}

		public static Vector3_d ClampMagnitude (Vector3_d vector, double maxLength) {
			if (vector.sqrMagnitude > maxLength * maxLength) {
				return vector.normalized * maxLength;
			}

			return vector;
		}

		public static Vector3_d Min (Vector3_d lhs, Vector3_d rhs) {
			return new Vector3_d(Math_d.Min(lhs.x, rhs.x), Math_d.Min(lhs.y, rhs.y), Math_d.Min(lhs.z, rhs.z));
		}

		public static Vector3_d Max (Vector3_d lhs, Vector3_d rhs) {
			return new Vector3_d(Math_d.Max(lhs.x, rhs.x), Math_d.Max(lhs.y, rhs.y), Math_d.Max(lhs.z, rhs.z));
		}

		public static Vector3_d Project (Vector3_d vector, Vector3_d onNormal) {
			double num = Dot(onNormal, onNormal);
			if (num < 1.40129846432482E-45)
				return zero;
			return onNormal * Dot(vector, onNormal) / num;
		}

		public static Vector3_d SmoothDamp (Vector3_d current, Vector3_d target, ref Vector3_d currentVelocity, double smoothTime, double maxSpeed) {
			double deltaTime = Time.deltaTime;
			return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static Vector3_d SmoothDamp (Vector3_d current, Vector3_d target, ref Vector3_d currentVelocity, double smoothTime) {
			double deltaTime = Time.deltaTime;
			const double maxSpeed = double.PositiveInfinity;
			return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static Vector3_d SmoothDamp (
			Vector3_d current,
			Vector3_d target,
			ref Vector3_d currentVelocity,
			double smoothTime,
			double maxSpeed,
			double deltaTime) {
			smoothTime = Math_d.Max(0.0001, smoothTime);
			double num1 = 2.0 / smoothTime;
			double num2 = num1 * deltaTime;
			double num3 = (1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2));
			Vector3_d vector = current - target;
			Vector3_d vector3_1 = target;
			double maxLength = maxSpeed * smoothTime;
			Vector3_d vector3_2 = ClampMagnitude(vector, maxLength);
			target = current - vector3_2;
			Vector3_d vector3_3 = (currentVelocity + num1 * vector3_2) * deltaTime;
			currentVelocity = (currentVelocity - num1 * vector3_3) * num3;
			Vector3_d vector3_4 = target + (vector3_2 + vector3_3) * num3;
			if (Dot(vector3_1 - current, vector3_4 - vector3_1) > 0.0) {
				vector3_4 = vector3_1;
				currentVelocity = (vector3_4 - vector3_1) / deltaTime;
			}

			return vector3_4;
		}

		public static Vector3_d Cross (Vector3_d lhs, Vector3_d rhs) {
			return new Vector3_d(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		public static Vector3_d Reflect (Vector3_d inDirection, Vector3_d inNormal) {
			return -2d * Dot(inNormal, inDirection) * inNormal + inDirection;
		}

		public static void OrthoNormalize (ref Vector3_d normal, ref Vector3_d tangent) {
			Vector3 v3normal = new Vector3();
			Vector3 v3tangent = new Vector3();
			v3normal = (Vector3) normal;
			v3tangent = (Vector3) tangent;
			Vector3.OrthoNormalize(ref v3normal, ref v3tangent);
			normal = new Vector3_d(v3normal);
			tangent = new Vector3_d(v3tangent);
		}

		public static void OrthoNormalize (ref Vector3_d normal, ref Vector3_d tangent, ref Vector3_d binormal) {
			Vector3 v3normal = new Vector3();
			Vector3 v3tangent = new Vector3();
			Vector3 v3binormal = new Vector3();
			v3normal = (Vector3) normal;
			v3tangent = (Vector3) tangent;
			v3binormal = (Vector3) binormal;
			Vector3.OrthoNormalize(ref v3normal, ref v3tangent, ref v3binormal);
			normal = new Vector3_d(v3normal);
			tangent = new Vector3_d(v3tangent);
			binormal = new Vector3_d(v3binormal);
		}

		public static Vector3_d Exclude (Vector3_d excludeThis, Vector3_d fromThat) {
			return fromThat - Project(fromThat, excludeThis);
		}

		public override int GetHashCode () {
			return x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;
		}

		public override bool Equals (object other) {
			return (other is Vector3_d) && Equals((Vector3_d) other);
		}

		public bool EqualsNormalized (Vector3_d other) {
			return normalized.Equals(other.normalized);
		}

		// non-boxing version
		public bool Equals (Vector3_d vector3d) {
			return x.Equals(vector3d.x) && y.Equals(vector3d.y) && z.Equals(vector3d.z);
		}

		public override string ToString () {
			return string.Format("({0:F2}, {1:F2}, {2:F2})", x, y, z);
		}

		public string ToString (string format) {
			return string.Format("({0}, {1}, {2})", x.ToString(format), y.ToString(format), z.ToString(format));
		}
	}
}
