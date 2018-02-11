// Copyright(C) David W. Jeske, 2014, and released to the public domain.
//
// Dynamic BVH (Bounding Volume Hierarchy) using incremental refit and tree-rotations
//
// initial BVH build based on: Bounding Volume Hierarchies (BVH) – A brief tutorial on what they are and how to implement them
//              http://www.3dmuve.com/3dmblog/?p=182
//
// Dynamic Updates based on: "Fast, Effective BVH Updates for Animated Scenes" (Kopta, Ize, Spjut, Brunvand, David, Kensler)
//              https://github.com/jeske/SimpleScene/blob/master/SimpleScene/Util/ssBVH/docs/BVH_fast_effective_updates_for_animated_scenes.pdf
//
// see also:  Space Partitioning: Octree vs. BVH
//            http://thomasdiewald.com/blog/?p=1488
//
// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: handle merge/split when LEAF_OBJ_MAX > 1 and objects move
// TODO: add sphere traversal

namespace SimpleScene.Util.ssBVH {
	public enum Axis {
		X,
		Y,
		Z
	}

	public interface SSBVHNodeAdaptor<T> {
		ssBVH<T> BVH { get; }
		void setBVH (ssBVH<T> bvh);
		Vector3d objectpos (T obj);
		double radius (T obj);
		void mapObjectToBVHLeaf (T obj, ssBVHNode<T> leaf);
		void unmapObject (T obj);
		void checkMap (T obj);
		ssBVHNode<T> getLeaf (T obj);
	}

	public class ssBVH<T> {
		public readonly ssBVHNode<T> rootBVH;
		public readonly SSBVHNodeAdaptor<T> nAda;
		public readonly int LEAF_OBJ_MAX;
		public int nodeCount = 0;
		public int maxDepth = 0;

		public readonly HashSet<ssBVHNode<T>> refitNodes = new HashSet<ssBVHNode<T>>();

		public delegate bool NodeTest (SSAABB box);

		// internal functional traversal...
		private static void _query (ssBVHNode<T> curNode, NodeTest hitTest, ICollection<ssBVHNode<T>> hitlist) {
			if (curNode == null) {
				return;
			}

			if (hitTest(curNode.box)) {
				hitlist.Add(curNode);
				_query(curNode.left, hitTest, hitlist);
				_query(curNode.right, hitTest, hitlist);
			}
		}

		// public interface to traversal..
		public List<ssBVHNode<T>> Query (NodeTest hitTest) {
			var hits = new List<ssBVHNode<T>>();
			_query(rootBVH, hitTest, hits);
			return hits;
		}

		public List<ssBVHNode<T>> Query (SSRay ray) {
			double tnear = 0f, tfar = 0f;

			return Query(box => OpenTKHelper.IntersectRayAABox1(ray, box, ref tnear, ref tfar));
		}

		public List<ssBVHNode<T>> Query (SSAABB volume) {
			return Query(box => box.IntersectsAABB(volume));
		}

		/// <summary>
		/// Call this to batch-optimize any object-changes notified through
		/// ssBVHNode.refit_ObjectChanged(..). For example, in a game-loop,
		/// call this once per frame.
		/// </summary>
		public void Optimize () {
			if (LEAF_OBJ_MAX != 1) {
				throw new Exception("In order to use optimize, you must set LEAF_OBJ_MAX=1");
			}

			while (refitNodes.Count > 0) {
				int maxdepth = refitNodes.Max(n => n.depth);

				var sweepNodes = refitNodes.Where(n => n.depth == maxdepth).ToList();
				sweepNodes.ForEach(n => refitNodes.Remove(n));

				sweepNodes.ForEach(n => n.tryRotate(this));
			}
		}

		public void AddObject (T newOb) {
			SSAABB box = SSAABB.FromSphere(nAda.objectpos(newOb), nAda.radius(newOb));
			double boxSAH = ssBVHNode<T>.SA(ref box);
			rootBVH.addObject(nAda, newOb, ref box, boxSAH);
		}

		public void RemoveObject (T newObj) {
			var leaf = nAda.getLeaf(newObj);
			leaf.removeObject(nAda, newObj);
		}

		public int Count () {
			return rootBVH.countBVHNodes();
		}

		/// <summary>
		/// initializes a BVH with a given nodeAdaptor, and object list.
		/// </summary>
		/// <param name="nodeAdaptor"></param>
		/// <param name="objects"></param>
		/// <param name="LEAF_OBJ_MAX">WARNING! currently this must be 1 to use dynamic BVH updates</param>
		public ssBVH (SSBVHNodeAdaptor<T> nodeAdaptor, List<T> objects, int LEAF_OBJ_MAX = 1) {
			this.LEAF_OBJ_MAX = LEAF_OBJ_MAX;
			nodeAdaptor.setBVH(this);
			nAda = nodeAdaptor;

			if (objects.Count > 0) {
				rootBVH = new ssBVHNode<T>(this, objects);
			} else {
				rootBVH = new ssBVHNode<T>(this);
				rootBVH.gobjects = new List<T>(); // it's a leaf, so give it an empty object list
			}
		}
	}
}
