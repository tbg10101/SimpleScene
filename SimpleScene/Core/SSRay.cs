// Copyright(C) David W. Jeske, 2013
// Released to the public domain.
// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using UnityEngine;

namespace SimpleScene {
	public struct SSRay {
		public Vector3d Position;
		public Vector3d Direction;

		public SSRay (Vector3d position, Vector3d direction) {
			Position = position;
			Direction = direction;
		}

		public static SSRay FromTwoPoints (Vector3d p1, Vector3d p2) {
			Vector3d pos = p1;
			Vector3d dir = (p2 - p1).normalized;

			return new SSRay(pos, dir);
		}

		public override string ToString () {
			return string.Format("{0} -> v{1}", Position, Direction);
		}
	}
}
