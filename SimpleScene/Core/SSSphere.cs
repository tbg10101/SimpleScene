// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using System;
using UnityEngine;

namespace SimpleScene {
	public struct SSSphere : IEquatable<SSSphere> {
		public Vector3d Position;
		public double Radius;

		public SSSphere (Vector3d position, double radius) {
			Position = position;
			Radius = radius;
		}

		public bool Equals (SSSphere other) {
			return Position == other.Position && Radius.Equals(other.Radius);
		}

		public bool IntersectsSphere (SSSphere other) {
			double addedR = Radius + other.Radius;
			double addedRSq = addedR * addedR;
			double distSq = (other.Position - Position).sqrMagnitude;
			return addedRSq >= distSq;
		}

		public bool IntersectsCapsule (SSCapsule other) {
			return Mathd.ClosestSegmentToSegmentDistance(Position, Position, other.P0, other.P1) < Radius + other.Radius;
		}

		public SSAABB ToAABB () {
			Vector3d rvec = new Vector3d(Radius);
			return new SSAABB(Position - rvec, Position + rvec);
		}
	}
}
