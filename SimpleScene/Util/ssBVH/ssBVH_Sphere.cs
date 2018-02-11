// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleScene.Util.ssBVH {
	public class SSSphereBVHNodeAdaptor : SSBVHNodeAdaptor<SSSphere> {
		protected ssBVH<SSSphere> _bvh;

		protected Dictionary<SSSphere, ssBVHNode<SSSphere>> _sphereToLeafMap = new Dictionary<SSSphere, ssBVHNode<SSSphere>>();

		public ssBVH<SSSphere> BVH {
			get {
				return _bvh;
			}
		}

		public void setBVH (ssBVH<SSSphere> bvh) {
			_bvh = bvh;
		}

		public Vector3d objectpos (SSSphere sphere) {
			return sphere.Position;
		}

		public double radius (SSSphere sphere) {
			return sphere.Radius;
		}

		public void checkMap (SSSphere sphere) {
			if (!_sphereToLeafMap.ContainsKey(sphere)) {
				throw new Exception("missing map for a shuffled child");
			}
		}

		public void unmapObject (SSSphere sphere) {
			_sphereToLeafMap.Remove(sphere);
		}

		public void mapObjectToBVHLeaf (SSSphere sphere, ssBVHNode<SSSphere> leaf) {
			_sphereToLeafMap[sphere] = leaf;
		}

		public ssBVHNode<SSSphere> getLeaf (SSSphere sphere) {
			return _sphereToLeafMap[sphere];
		}
	}

	public class SSSphereBVH : ssBVH<SSSphere> {
		public SSSphereBVH (int maxSpheresPerLeaf = 1) : base(
			new SSSphereBVHNodeAdaptor(),
			new List<SSSphere>(),
			maxSpheresPerLeaf) { }
	}
}
