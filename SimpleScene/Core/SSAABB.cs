// Copyright(C) David W. Jeske, 2013
// Released to the public domain.
// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using System;
using UnityEngine;

namespace SimpleScene {
	public struct SSAABB : IEquatable<SSAABB> {
		public Vector3d Min;
		public Vector3d Max;

		public SSAABB (double min = double.PositiveInfinity, double max = double.NegativeInfinity) {
			Min = new Vector3d(min);
			Max = new Vector3d(max);
		}

		public SSAABB (Vector3d min, Vector3d max) {
			Min = min;
			Max = max;
		}

		public void Combine (ref SSAABB other) {
			Min = Vector3d.Min(Min, other.Min);
			Max = Vector3d.Max(Max, other.Max);
		}

		public bool IntersectsSphere (Vector3d origin, double radius) {
			return (!(origin.x + radius < Min.x)) && (!(origin.y + radius < Min.y)) && (!(origin.z + radius < Min.z)) && (!(origin.x - radius > Max.x)) && (!(origin.y - radius > Max.y)) && (!(origin.z - radius > Max.z));
		}

		public bool IntersectsAABB (SSAABB box) {
			return (Max.x > box.Min.x)
			       && (Min.x < box.Max.x)
			       && (Max.y > box.Min.y)
			       && (Min.y < box.Max.y)
			       && (Max.z > box.Min.z)
			       && (Min.z < box.Max.z);
		}

		public bool Equals (SSAABB other) {
			return (Min.x == other.Min.x)
			       && (Min.y == other.Min.y)
			       && (Min.z == other.Min.z)
			       && (Max.x == other.Max.x)
			       && (Max.y == other.Max.y)
			       && (Max.z == other.Max.z);
		}

		public void UpdateMin (Vector3d localMin) {
			Min = Vector3d.Min(Min, localMin);
		}

		public void UpdateMax (Vector3d localMax) {
			Max = Vector3d.Max(Max, localMax);
		}

		public Vector3d Center () {
			return (Min + Max) / 2f;
		}

		public Vector3d Diff () {
			return Max - Min;
		}

		internal void ExpandToFit (SSAABB b) {
			if (b.Min.x < Min.x) {
				Min.x = b.Min.x;
			}

			if (b.Min.y < Min.y) {
				Min.y = b.Min.y;
			}

			if (b.Min.z < Min.z) {
				Min.z = b.Min.z;
			}

			if (b.Max.x > Max.x) {
				Max.x = b.Max.x;
			}

			if (b.Max.y > Max.y) {
				Max.y = b.Max.y;
			}

			if (b.Max.z > Max.z) {
				Max.z = b.Max.z;
			}
		}

		public SSAABB ExpandedBy (SSAABB b) {
			SSAABB newbox = this;
			if (b.Min.x < newbox.Min.x) {
				newbox.Min.x = b.Min.x;
			}

			if (b.Min.y < newbox.Min.y) {
				newbox.Min.y = b.Min.y;
			}

			if (b.Min.z < newbox.Min.z) {
				newbox.Min.z = b.Min.z;
			}

			if (b.Max.x > newbox.Max.x) {
				newbox.Max.x = b.Max.x;
			}

			if (b.Max.y > newbox.Max.y) {
				newbox.Max.y = b.Max.y;
			}

			if (b.Max.z > newbox.Max.z) {
				newbox.Max.z = b.Max.z;
			}

			return newbox;
		}

		public void ExpandBy (SSAABB b) {
			this = ExpandedBy(b);
		}

		public static SSAABB FromSphere (Vector3d pos, double radius) {
			SSAABB box;

			box.Min.x = pos.x - radius;
			box.Max.x = pos.x + radius;
			box.Min.y = pos.y - radius;
			box.Max.y = pos.y + radius;
			box.Min.z = pos.z - radius;
			box.Max.z = pos.z + radius;

			return box;
		}
	}
}
