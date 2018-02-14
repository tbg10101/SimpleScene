// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleScene.Util.ssBVH {
	public class ShapeBVHNodeAdaptor : SSBVHNodeAdaptor<Shape> {
		private ssBVH<Shape> _bvh;
		private readonly Dictionary<Shape, ssBVHNode<Shape>> _shapeToLeafMap = new Dictionary<Shape, ssBVHNode<Shape>>();

		public ssBVH<Shape> BVH {
			get {
				return _bvh;
			}
			set {
				_bvh = value;
			}
		}

		public Boundsd GetObjectBounds (Shape shape) {
			return shape.Bounds;
		}

		public Vector3d GetObjectPosition (Shape shape) {
			return shape.Centroid;
		}

		public void CheckMap (Shape shape) {
			if (!_shapeToLeafMap.ContainsKey(shape)) {
				throw new Exception("missing map for a shuffled child");
			}
		}

		public void UnmapObject (Shape shape) {
			_shapeToLeafMap.Remove(shape);
		}

		public void MapObjectToBvhLeaf (Shape shape, ssBVHNode<Shape> leaf) {
			_shapeToLeafMap[shape] = leaf;
		}

		public ssBVHNode<Shape> GetLeaf (Shape shape) {
			return _shapeToLeafMap[shape];
		}
	}

	public class ShapeBVH : ssBVH<Shape> {
		public ShapeBVH (int maxShapesPerLeaf = 1) : base(new ShapeBVHNodeAdaptor(), new List<Shape>(), maxShapesPerLeaf) { }
	}
}
