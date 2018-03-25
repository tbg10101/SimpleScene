using System;

namespace UnityEngine {
	// ReSharper disable once InconsistentNaming
	public struct Bounds_d : IEquatable<Bounds_d> {
		private Vector3_d _center;
		public Vector3_d center {
			get {
				return _center;
			}
			set {
				_center = value;

				_min = _center - _extents;
				_max = _center + _extents;
			}
		}

		private Vector3_d _extents;
		public Vector3_d extents {
			get {
				return _extents;
			}
			set {
				_extents = value;

				_size = 2.0 * _extents;

				_min = _center - _extents;
				_max = _center + _extents;
			}
		}

		private Vector3_d _size;
		public Vector3_d size {
			get {
				return _size;
			}
			set {
				_size = value;

				_extents = _size / 2.0;

				_min = _center - _extents;
				_max = _center + _extents;
			}
		}

		private Vector3_d _min;
		public Vector3_d min {
			get {
				return _min;
			}
		}

		private Vector3_d _max;
		public Vector3_d max {
			get {
				return _max;
			}
		}

		public Bounds_d (Bounds bounds) {
			_center = bounds.center;
			_extents = bounds.extents;
			_size = 2.0 * _extents;
			_min = _center - _extents;
			_max = _center + _extents;
		}

		public static implicit operator Bounds_d (Bounds bounds) {
			return new Bounds_d(bounds);
		}

		public void ConformTo (Vector3_d v0, Vector3_d v1) {
			_center = (v0 + v1) / 2.0;
			size = Vector3_d.Max(v0, v1) - Vector3_d.Min(v0, v1);
		}

		public void ExpandToFit (Bounds_d other) {
			ConformTo(Vector3_d.Min(_min, other.min), Vector3_d.Max(_max, other.max));
		}

		public Bounds_d ExpandedToFit (Bounds_d other) {
			Bounds_d newBoundsD = this;
			newBoundsD.ExpandToFit(other);
			return newBoundsD;
		}

		public bool Intersects (Bounds_d other) {
			return Intersects(this, other);
		}

		public bool Equals (Bounds_d other) {
			return _center.Equals(other._center) && _extents.Equals(other._extents);
		}

		public static Bounds_d FromCenterAndSize (Vector3_d center, Vector3_d size) {
			return new Bounds_d {
				_center = center,
				size = size
			};
		}

		public static Bounds_d FromCenterAndExtents (Vector3_d center, Vector3_d extents) {
			return new Bounds_d {
				_center = center,
				extents = extents
			};
		}

		public static Bounds_d FromPoints (Vector3_d v0, Vector3_d v1) {
			Bounds_d newBounds = new Bounds_d();
			newBounds.ConformTo(v0, v1);
			return newBounds;
		}

		public static bool Intersects (Bounds_d b0, Bounds_d b1) {
			return b0._max.x > b1._min.x
			       && b0._min.x < b1._max.x
			       && b0._max.y > b1._min.y
			       && b0._min.y < b1._max.y
			       && b0._max.z > b1._min.z
			       && b0._min.z < b1._max.z;
		}
	}
}
