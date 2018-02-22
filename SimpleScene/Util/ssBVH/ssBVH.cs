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
		ssBVH<T> BVH { get; set; }
		Vector3d GetObjectPosition (T obj);
		Boundsd GetObjectBounds (T obj);
		void MapObjectToBvhLeaf (T obj, ssBVHNode<T> leaf);
		void UnmapObject (T obj);
		void CheckMap (T obj);
		ssBVHNode<T> GetLeaf (T obj);
	}

	public class ssBVH<T> {
		public readonly ssBVHNode<T> RootBVH;
		public readonly SSBVHNodeAdaptor<T> nAda;
		public readonly int LEAF_OBJ_MAX;
		public int NodeCount = 0;
		public int MaxDepth = 0;

		public readonly HashSet<ssBVHNode<T>> RefitNodes = new HashSet<ssBVHNode<T>>();

		public delegate bool NodeTest (Boundsd box);

		// internal functional traversal...
		private static void _query (ssBVHNode<T> curNode, NodeTest hitTest, ICollection<ssBVHNode<T>> hitlist) {
			if (curNode == null) {
				return;
			}

			if (hitTest(curNode.Bounds)) {
				hitlist.Add(curNode);
				_query(curNode.Left, hitTest, hitlist);
				_query(curNode.Right, hitTest, hitlist);
			}
		}

		// public interface to traversal..
		public List<ssBVHNode<T>> Query (NodeTest hitTest) {
			var hits = new List<ssBVHNode<T>>();
			_query(RootBVH, hitTest, hits);
			return hits;
		}

		public List<ssBVHNode<T>> Query (Rayd ray) {
			return Query(ray.IntersectsBounds);
		}

		public List<ssBVHNode<T>> Query (Boundsd volume) {
			return Query(box => box.Intersects(volume));
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

			while (RefitNodes.Count > 0) {
				int maxdepth = RefitNodes.Max(n => n.Depth);

				var sweepNodes = RefitNodes.Where(n => n.Depth == maxdepth).ToList();
				sweepNodes.ForEach(n => RefitNodes.Remove(n));

				sweepNodes.ForEach(n => n.TryRotate(this));
			}
		}

		public void AddObject (T newOb) {
			Boundsd box = nAda.GetObjectBounds(newOb);
			double boxSAH = ssBVHNode<T>.SA(ref box);
			RootBVH.AddObject(nAda, newOb, ref box, boxSAH);
		}

		public void RemoveObject (T newObj) {
			var leaf = nAda.GetLeaf(newObj);
			leaf.RemoveObject(nAda, newObj);
		}

		public int Count () {
			return RootBVH.CountBvhNodes();
		}

		/// <summary>
		/// initializes a BVH with a given nodeAdaptor, and object list.
		/// </summary>
		/// <param name="nodeAdaptor"></param>
		/// <param name="objects"></param>
		/// <param name="LEAF_OBJ_MAX">WARNING! currently this must be 1 to use dynamic BVH updates</param>
		public ssBVH (SSBVHNodeAdaptor<T> nodeAdaptor, List<T> objects, int LEAF_OBJ_MAX = 1) {
			this.LEAF_OBJ_MAX = LEAF_OBJ_MAX;
			nodeAdaptor.BVH = this;
			nAda = nodeAdaptor;

			if (objects.Count > 0) {
				RootBVH = new ssBVHNode<T>(this, objects);
			} else {
				RootBVH = new ssBVHNode<T>(this) {ContainedObjects = new List<T>()}; // it's a leaf, so give it an empty object list
			}
		}
	}
}
