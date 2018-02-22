using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleScene {
	public class Shape : IEquatable<Shape> {
		private static int _nextId = 0;

		private readonly int _id = _nextId++;

		public enum ShapeType {
			Sphere,
			Capsule
		}

		private static readonly Dictionary<Tuple<ShapeType, ShapeType>, Func<Shape, Shape, bool>> IntersectionDetectors
			= new Dictionary<Tuple<ShapeType, ShapeType>, Func<Shape, Shape, bool>> {
				{new Tuple<ShapeType, ShapeType>(ShapeType.Sphere, ShapeType.Sphere), CheckSphereIntersection},
				{new Tuple<ShapeType, ShapeType>(ShapeType.Capsule, ShapeType.Capsule), CheckCapsuleIntersection},
				{new Tuple<ShapeType, ShapeType>(ShapeType.Sphere, ShapeType.Capsule), CheckSphereCapsuleIntersection},
				{new Tuple<ShapeType, ShapeType>(ShapeType.Capsule, ShapeType.Sphere), CheckSphereCapsuleIntersection}
			};

		private ShapeType _shapeTypeValue;
		public ShapeType ShapeTypeValue {
			get {
				return _shapeTypeValue;
			}
			set {
				if (_shapeTypeValue == value) {
					return;
				}

				_shapeTypeValue = value;
				NotifyPositionOrSizeChanged();
			}
		}

		private Vector3d _position0;
		public Vector3d Position0 {
			get {
				return _position0;
			}
			set {
				if (_position0.Equals(value)) {
					return;
				}

				_position0 = value;
				NotifyPositionOrSizeChanged();
			}
		}

		private Vector3d _position1;
		public Vector3d Position1 {
			get {
				return _position1;
			}
			set {
				if (_position1.Equals(value)) {
					return;
				}

				_position1 = value;
				NotifyPositionOrSizeChanged();
			}
		}


		private double _extends0;
		public double Extends0 {
			get {
				return _extends0;
			}
			set {
				if (_extends0.Equals(value)) {
					return;
				}

				_extends0 = value;
				NotifyPositionOrSizeChanged();
			}
		}

		public delegate void PositionOrSizeChangedHandler(Shape sender);
		public event PositionOrSizeChangedHandler OnPositionOrSizeChanged;

		public bool Equals (Shape other) {
			if (other == null) {
				return false;
			}

			bool result = _shapeTypeValue == other._shapeTypeValue && _position0 == other._position0;

			if (_shapeTypeValue == ShapeType.Sphere || _shapeTypeValue == ShapeType.Capsule) {
				result &= _extends0.Equals(other._extends0);
			}

			if (_shapeTypeValue == ShapeType.Capsule) {
				result &= _position1 == other._position1;
			}

			return result;
		}

		override
		public bool Equals (object o) {
			return o is Shape && Equals((Shape) o);
		}

		override
		public int GetHashCode () {
			return 17 * _id;
		}

		public Boundsd Bounds {
			get {
				Vector3d extendsVector = new Vector3d(_extends0);

				switch (_shapeTypeValue) {
					case ShapeType.Sphere:
						return Boundsd.FromCenterAndExtents(_position0, extendsVector);
					case ShapeType.Capsule:
						return Boundsd.FromPoints(Vector3d.Min(_position0, _position1) - extendsVector, Vector3d.Max(_position0, _position1) + extendsVector);
					default:
						return new Boundsd();
				}
			}
		}

		public Vector3d Centroid {
			get {
				if (ShapeTypeValue == ShapeType.Capsule) {
					return (_position0 + _position1) / 2.0;
				}

				return _position0;
			}
		}

		private void NotifyPositionOrSizeChanged () {
			if (OnPositionOrSizeChanged != null) {
				OnPositionOrSizeChanged(this);
			}
		}

		public static bool Intersects (ref Shape s0, ref Shape s1) {
			Func<Shape, Shape, bool> function = IntersectionDetectors[new Tuple<ShapeType, ShapeType>(s0._shapeTypeValue, s1._shapeTypeValue)];
			return function != null && function(s0, s1);
		}

		private static bool CheckSphereIntersection (Shape s0, Shape s1) {
			if (s0._shapeTypeValue != ShapeType.Sphere || s1._shapeTypeValue != ShapeType.Sphere) {
				throw new ArgumentException(
					string.Format(
						"CheckSphereIntersection must take exactly two Spheres. s0 is {0} and s1 is {1}.",
						s0._shapeTypeValue.ToString(),
						s1._shapeTypeValue.ToString()));
			}

			return Vector3d.SqrDistance(s1._position0, s0._position0) < (s0._extends0 + s1._extends0) * (s0._extends0 + s1._extends0);
		}

		private static bool CheckCapsuleIntersection (Shape s0, Shape s1) {
			if (s0._shapeTypeValue != ShapeType.Capsule || s1._shapeTypeValue != ShapeType.Capsule) {
				throw new ArgumentException(
					string.Format(
						"CheckCapsuleIntersection must take exactly two Capsules. s0 is {0} and s1 is {1}.",
						s0._shapeTypeValue.ToString(),
						s1._shapeTypeValue.ToString()));
			}

			return Mathd.ClosestSegmentToSegmentSqrDistance(s0._position0, s0._position1, s1._position0, s1._position1)
			       < (s0._extends0 + s1._extends0) * (s0._extends0 + s1._extends0);
		}

		private static bool CheckSphereCapsuleIntersection (Shape s0, Shape s1) {
			Shape sphere;
			Shape capsule;

			if (s0._shapeTypeValue == ShapeType.Sphere && s1._shapeTypeValue == ShapeType.Capsule) {
				sphere = s0;
				capsule = s1;
			} else if (s0._shapeTypeValue == ShapeType.Capsule && s1._shapeTypeValue == ShapeType.Sphere) {
				sphere = s1;
				capsule = s0;
			} else {
				throw new ArgumentException(
					string.Format(
						"CheckSphereCapsuleIntersection must take exactly one Sphere and one Capsule. s0 is {0} and s1 is {1}.",
						s0._shapeTypeValue.ToString(),
						s1._shapeTypeValue.ToString()));
			}

			// the sphere's coordinates must be in the first two parameters because the algorithm defaults to one of them if the lines are parallel
			// (or one is a point)
			return Mathd.ClosestSegmentToSegmentSqrDistance(sphere._position0, sphere._position0, capsule._position0, capsule._position1)
			       < (capsule._extends0 + sphere._extends0) * (capsule._extends0 + sphere._extends0);
		}
	}
}
