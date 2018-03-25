using System;

namespace UnityEngine {
	// ReSharper disable once InconsistentNaming
	public struct Ray_d : IEquatable<Ray_d> {
		public Vector3_d origin;
		public Vector3_d direction;

		public Ray_d (Vector3_d origin, Vector3_d direction) {
			this.origin = origin;
			this.direction = direction.normalized;
		}

		public Ray_d (Ray ray) {
			origin = ray.origin;
			direction = ray.direction;
		}

		public static implicit operator Ray_d (Ray ray) {
			return new Ray_d(ray);
		}

		public Vector3_d GetPoint (double distance) {
			return origin + direction * distance;
		}

		public bool IntersectsBounds (Bounds_d box) {
			Vector3_d dirfrac = new Vector3_d {
				x = 1.0 / direction.x,
				y = 1.0 / direction.y,
				z = 1.0 / direction.z
			};

			double t1 = (box.min.x - origin.x) * dirfrac.x;
			double t2 = (box.max.x - origin.x) * dirfrac.x;
			double t3 = (box.min.y - origin.y) * dirfrac.y;
			double t4 = (box.max.y - origin.y) * dirfrac.y;
			double t5 = (box.min.z - origin.z) * dirfrac.z;
			double t6 = (box.max.z - origin.z) * dirfrac.z;

			double tmin = Math_d.Max(Math_d.Max(Math_d.Min(t1, t2), Math_d.Min(t3, t4)), Math_d.Min(t5, t6));
			double tmax = Math_d.Min(Math_d.Min(Math_d.Max(t1, t2), Math_d.Max(t3, t4)), Math_d.Max(t5, t6));

			// if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
			if (tmax < 0) {
				return false;
			}

			// if tmin <= tmax, ray intersects AABB
			return tmin <= tmax;
		}

		public bool Equals (Ray_d other) {
			return origin.Equals(other.origin) && direction.EqualsNormalized(other.direction);
		}

		public override string ToString () {
			return string.Format("Origin: {0}, Dir: {1}", origin, direction);
		}

		public string ToString (string format) {
			return string.Format("Origin: {0}, Dir: {1}", origin.ToString(format), direction.ToString(format));
		}
	}
}
