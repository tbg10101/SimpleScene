// Copyright(C) Tristan Bellman-Greenwood, 2018

using System;
using System.Security.Cryptography;
using UnityEngine;

namespace SimpleScene {
	public struct SSCapsule : IEquatable<SSCapsule> {
		public Vector3d P0;
		public Vector3d P1;
		public double Radius;

		public SSCapsule (Vector3d p0, Vector3d p1, double radius) {
			P0 = p0;
			P1 = p1;
			Radius = radius;
		}

		public bool Equals (SSCapsule other) {
			return P0 == other.P0 && P1 == other.P1 && Radius.Equals(other.Radius);
		}

		public bool IntersectsSphere (SSSphere other) {
			return Mathd.ClosestSegmentToSegmentDistance(P0, P1, other.Position, other.Position) < Radius + other.Radius;
		}

		public bool IntersectsCapsule (SSCapsule other) {
			return Mathd.ClosestSegmentToSegmentDistance(P0, P1, other.P0, other.P1) < Radius + other.Radius;
		}

		public SSAABB ToAABB () {
			Vector3d rvec = new Vector3d(Radius);
			return new SSAABB(Vector3d.Min(P0, P1) - rvec, Vector3d.Max(P0, P1) + rvec);
		}
	}
}
