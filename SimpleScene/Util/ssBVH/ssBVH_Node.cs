// Copyright(C) David W. Jeske, 2014, and released to the public domain.
// Converted to Unity 64-bit by Tristan Bellman-Greenwood
//
// Dynamic BVH (Bounding Volume Hierarchy) using incremental refit and tree-rotations
//
// initial BVH build based on: Bounding Volume Hierarchies (BVH) – A brief tutorial on what they are and how to implement them
//              http://www.3dmuve.com/3dmblog/?p=182
//
// Dynamic Updates based on: "Fast, Effective BVH Updates for Animated Scenes" (Kopta, Ize, Spjut, Brunvand, David, Kensler)
//              http://www.cs.utah.edu/~thiago/papers/rotations.pdf
//
// see also:  Space Partitioning: Octree vs. BVH
//            http://thomasdiewald.com/blog/?p=1488
//
// TODO: pick the best axis to split based on SAH, instead of the biggest
// TODO: Switch SAH comparisons to use (SAH(A) * itemCount(A)) currently it just uses SAH(A)
// TODO: when inserting, compare parent node SAH(A) * itemCount to sum of children, to see if it is better to not split at all
// TODO: implement node merge/split, to handle updates when LEAF_OBJ_MAX > 1
//
// TODO: implement SBVH spacial splits
//        http://www.nvidia.com/docs/IO/77714/sbvh.pdf

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace SimpleScene.Util.ssBVH {
	public class ssBVHNode<GO> {
		public Boundsd box;

		public ssBVHNode<GO> parent;
		public ssBVHNode<GO> left;
		public ssBVHNode<GO> right;

		public int depth;
		public int nodeNumber; // for debugging

		public List<GO> gobjects; // only populated in leaf nodes

		public override string ToString () {
			return String.Format("ssBVHNode<{0}>:{1}", typeof(GO), nodeNumber);
		}

		private Axis PickSplitAxis () {
			double axis_x = box.max.x - box.min.x;
			double axis_y = box.max.y - box.min.y;
			double axis_z = box.max.z - box.min.z;

			// return the biggest axis
			if (axis_x > axis_y) {
				if (axis_x > axis_z) {
					return Axis.X;
				}

				return Axis.Z;
			}

			if (axis_y > axis_z) {
				return Axis.Y;
			}

			return Axis.Z;
		}

		public bool IsLeaf {
			get {
				bool isLeaf = (gobjects != null);
				// if we're a leaf, then both left and right should be null..
				if (isLeaf && ((right != null) || (left != null))) {
					throw new Exception("ssBVH Leaf has objects and left/right pointers!");
				}

				return isLeaf;
			}
		}

		private Axis NextAxis (Axis cur) {
			switch (cur) {
				case Axis.X: return Axis.Y;
				case Axis.Y: return Axis.Z;
				case Axis.Z: return Axis.X;
				default: throw new NotSupportedException();
			}
		}

		public void refit_ObjectChanged (SSBVHNodeAdaptor<GO> nAda, GO obj) {
			if (gobjects == null) {
				throw new Exception("dangling leaf!");
			}

			if (RefitVolume(nAda)) {
				// add our parent to the optimize list...
				if (parent != null) {
					nAda.BVH.refitNodes.Add(parent);

					// you can force an optimize every time something moves, but it's not very efficient
					// instead we do this per-frame after a bunch of updates.
					// nAda.BVH.optimize();
				}
			}
		}

		private void ExpandVolume (Boundsd bounds) {
			Boundsd oldBox = box;
			box.ExpandToFit(bounds);

			if (!oldBox.Equals(box) && parent != null) {
				parent.ExpandVolume(box);
			}
		}

		internal void ComputeVolume (SSBVHNodeAdaptor<GO> nAda) {
			box = nAda.GetObjectBounds(gobjects[0]);

			for (int i = 1; i < gobjects.Count; i++) {
				ExpandVolume(nAda.GetObjectBounds(gobjects[i]));
			}
		}

		internal bool RefitVolume (SSBVHNodeAdaptor<GO> nAda) {
			if (gobjects.Count == 0) {
				throw new NotImplementedException();
			} // TODO: fix this... we should never get called in this case...

			Boundsd oldbox = box;

			ComputeVolume(nAda);
			if (!box.Equals(oldbox)) {
				if (parent != null) parent.ChildRefit(nAda);
				return true;
			}

			return false;
		}

		internal static double SA (Boundsd box) {
			double x_size = box.max.x - box.max.x;
			double y_size = box.max.y - box.max.y;
			double z_size = box.max.z - box.max.z;

			return 2.0f * ((x_size * y_size) + (x_size * z_size) + (y_size * z_size));
		}

		internal static double SA (ref Boundsd box) {
			double x_size = box.max.x - box.min.x;
			double y_size = box.max.y - box.min.y;
			double z_size = box.max.z - box.min.z;

			return 2.0f * ((x_size * y_size) + (x_size * z_size) + (y_size * z_size));
		}

		internal static double SA (ssBVHNode<GO> node) {
			double x_size = node.box.max.x - node.box.min.x;
			double y_size = node.box.max.y - node.box.min.y;
			double z_size = node.box.max.z - node.box.min.z;

			return 2.0f * ((x_size * y_size) + (x_size * z_size) + (y_size * z_size));
		}

		internal static Boundsd BoundsOfPair (ssBVHNode<GO> nodea, ssBVHNode<GO> nodeb) {
			Boundsd box = nodea.box;
			box.ExpandToFit(nodeb.box);
			return box;
		}

		internal double SAofList (SSBVHNodeAdaptor<GO> nAda, List<GO> list) {
			Boundsd box = nAda.GetObjectBounds(list[0]);

			for (int i = 1; i < list.Count - 1; i++) {
				box.ExpandToFit(nAda.GetObjectBounds(list[i]));
			}

			return SA(ref box);
		}

		// The list of all candidate rotations, from "Fast, Effective BVH Updates for Animated Scenes", Figure 1.
		internal enum Rot {
			NONE,
			L_RL,
			L_RR,
			R_LL,
			R_LR,
			LL_RR,
			LL_RL
		}

		internal class rotOpt : IComparable<rotOpt> { // rotation option
			public double SAH;
			public Rot rot;

			internal rotOpt (double SAH, Rot rot) {
				this.SAH = SAH;
				this.rot = rot;
			}

			public int CompareTo (rotOpt other) {
				return SAH.CompareTo(other.SAH);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static List<Rot> eachRot {
			get {
				return new List<Rot>((Rot[]) Enum.GetValues(typeof(Rot)));
			}
		}

		/// <summary>
		/// tryRotate looks at all candidate rotations, and executes the rotation with the best resulting SAH (if any)
		/// </summary>
		/// <param name="bvh"></param>
		internal void TryRotate (ssBVH<GO> bvh) {
			SSBVHNodeAdaptor<GO> nAda = bvh.nAda;

			// if we are not a grandparent, then we can't rotate, so queue our parent and bail out
			if (left.IsLeaf && right.IsLeaf) {
				if (parent != null) {
					bvh.refitNodes.Add(parent);
					return;
				}
			}

			// for each rotation, check that there are grandchildren as necessary (aka not a leaf)
			// then compute total SAH cost of our branches after the rotation.

			double mySA = SA(left) + SA(right);

			rotOpt bestRot = eachRot.Min(
				rot => {
					switch (rot) {
						case Rot.NONE: return new rotOpt(mySA, Rot.NONE);
						// child to grandchild rotations
						case Rot.L_RL:
							if (right.IsLeaf) return new rotOpt(double.MaxValue, Rot.NONE);
							else return new rotOpt(SA(right.left) + SA(BoundsOfPair(left, right.right)), rot);
						case Rot.L_RR:
							if (right.IsLeaf) return new rotOpt(double.MaxValue, Rot.NONE);
							else return new rotOpt(SA(right.right) + SA(BoundsOfPair(left, right.left)), rot);
						case Rot.R_LL:
							if (left.IsLeaf) return new rotOpt(double.MaxValue, Rot.NONE);
							else return new rotOpt(SA(BoundsOfPair(right, left.right)) + SA(left.left), rot);
						case Rot.R_LR:
							if (left.IsLeaf) return new rotOpt(double.MaxValue, Rot.NONE);
							else return new rotOpt(SA(BoundsOfPair(right, left.left)) + SA(left.right), rot);
						// grandchild to grandchild rotations
						case Rot.LL_RR:
							if (left.IsLeaf || right.IsLeaf) return new rotOpt(double.MaxValue, Rot.NONE);
							else return new rotOpt(SA(BoundsOfPair(right.right, left.right)) + SA(BoundsOfPair(right.left, left.left)), rot);
						case Rot.LL_RL:
							if (left.IsLeaf || right.IsLeaf) return new rotOpt(double.MaxValue, Rot.NONE);
							else return new rotOpt(SA(BoundsOfPair(right.left, left.right)) + SA(BoundsOfPair(left.left, right.right)), rot);
						// unknown...
						default: throw new NotImplementedException("missing implementation for BVH Rotation SAH Computation .. " + rot.ToString());
					}
				});

			// perform the best rotation...
			if (bestRot.rot != Rot.NONE) {
				// if the best rotation is no-rotation... we check our parents anyhow..
				if (parent != null) {
					// but only do it some random percentage of the time.
					if ((DateTime.Now.Ticks % 100) < 2) {
						bvh.refitNodes.Add(parent);
					}
				}
			} else {
				if (parent != null) {
					bvh.refitNodes.Add(parent);
				}

				if (((mySA - bestRot.SAH) / mySA) < 0.3f) {
					return; // the benefit is not worth the cost
				}

				Console.WriteLine("BVH swap {0} from {1} to {2}", bestRot.rot.ToString(), mySA, bestRot.SAH);

				// in order to swap we need to:
				//  1. swap the node locations
				//  2. update the depth (if child-to-grandchild)
				//  3. update the parent pointers
				//  4. refit the boundary box
				ssBVHNode<GO> swap = null;
				switch (bestRot.rot) {
					case Rot.NONE: break;
					// child to grandchild rotations
					case Rot.L_RL:
						swap = left;
						left = right.left;
						left.parent = this;
						right.left = swap;
						swap.parent = right;
						right.ChildRefit(nAda, propagate: false);
						break;
					case Rot.L_RR:
						swap = left;
						left = right.right;
						left.parent = this;
						right.right = swap;
						swap.parent = right;
						right.ChildRefit(nAda, propagate: false);
						break;
					case Rot.R_LL:
						swap = right;
						right = left.left;
						right.parent = this;
						left.left = swap;
						swap.parent = left;
						left.ChildRefit(nAda, propagate: false);
						break;
					case Rot.R_LR:
						swap = right;
						right = left.right;
						right.parent = this;
						left.right = swap;
						swap.parent = left;
						left.ChildRefit(nAda, propagate: false);
						break;

					// grandchild to grandchild rotations
					case Rot.LL_RR:
						swap = left.left;
						left.left = right.right;
						right.right = swap;
						left.left.parent = left;
						swap.parent = right;
						left.ChildRefit(nAda, propagate: false);
						right.ChildRefit(nAda, propagate: false);
						break;
					case Rot.LL_RL:
						swap = left.left;
						left.left = right.left;
						right.left = swap;
						left.left.parent = left;
						swap.parent = right;
						left.ChildRefit(nAda, propagate: false);
						right.ChildRefit(nAda, propagate: false);
						break;

					// unknown...
					default: throw new NotImplementedException("missing implementation for BVH Rotation .. " + bestRot.rot);
				}

				// fix the depths if necessary....
				switch (bestRot.rot) {
					case Rot.L_RL:
					case Rot.L_RR:
					case Rot.R_LL:
					case Rot.R_LR:
						SetDepth(nAda, depth);
						break;
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static List<Axis> eachAxis {
			get {
				return new List<Axis>((Axis[]) Enum.GetValues(typeof(Axis)));
			}
		}

#pragma warning disable 693
		internal class SplitAxisOpt<GO> : IComparable<SplitAxisOpt<GO>> { // split Axis option
#pragma warning restore 693
			public double SAH;
			public Axis axis;
			public List<GO> left, right;

			internal SplitAxisOpt (double SAH, Axis axis, List<GO> left, List<GO> right) {
				this.SAH = SAH;
				this.axis = axis;
				this.left = left;
				this.right = right;
			}

			public int CompareTo (SplitAxisOpt<GO> other) {
				return SAH.CompareTo(other.SAH);
			}
		}

		private void SplitNode (SSBVHNodeAdaptor<GO> nAda) {
			// second, decide which axis to split on, and sort..
			List<GO> splitlist = gobjects;
			splitlist.ForEach(nAda.UnmapObject);
			int center = splitlist.Count / 2; // find the center object

			SplitAxisOpt<GO> bestSplit = eachAxis.Min(
				axis => {
					var orderedlist = new List<GO>(splitlist);
					switch (axis) {
						case Axis.X:
							orderedlist.Sort((go1, go2) => nAda.GetObjectPosition(go1).x.CompareTo(nAda.GetObjectPosition(go2).x));
							break;
						case Axis.Y:
							orderedlist.Sort((go1, go2) => nAda.GetObjectPosition(go1).y.CompareTo(nAda.GetObjectPosition(go2).y));
							break;
						case Axis.Z:
							orderedlist.Sort((go1, go2) => nAda.GetObjectPosition(go1).z.CompareTo(nAda.GetObjectPosition(go2).z));
							break;
						default:
							throw new NotImplementedException("unknown split axis: " + axis.ToString());
					}

					var left_s = orderedlist.GetRange(0, center);
					var right_s = orderedlist.GetRange(center, splitlist.Count - center);

					double SAH = SAofList(nAda, left_s) * left_s.Count + SAofList(nAda, right_s) * right_s.Count;
					return new SplitAxisOpt<GO>(SAH, axis, left_s, right_s);
				});

			// perform the split
			gobjects = null;
			left = new ssBVHNode<GO>(nAda.BVH, this, bestSplit.left, depth + 1); // Split the Hierarchy to the left
			right = new ssBVHNode<GO>(nAda.BVH, this, bestSplit.right, depth + 1); // Split the Hierarchy to the right
		}

		private void SplitIfNecessary (SSBVHNodeAdaptor<GO> nAda) {
			if (gobjects.Count > nAda.BVH.LEAF_OBJ_MAX) {
				SplitNode(nAda);
			}
		}

		internal void AddObject (SSBVHNodeAdaptor<GO> nAda, GO newOb, ref Boundsd newObBox, double newObSAH) {
			AddObject(nAda, this, newOb, ref newObBox, newObSAH);
		}

		private static void addObject_Pushdown (SSBVHNodeAdaptor<GO> nAda, ssBVHNode<GO> curNode, GO newOb) {
			var left = curNode.left;
			var right = curNode.right;

			// merge and pushdown left and right as a new node..
			var mergedSubnode = new ssBVHNode<GO>(nAda.BVH) {
				left = left,
				right = right,
				parent = curNode,
				gobjects = null
			};
			// we need to be an interior node... so null out our object list..
			left.parent = mergedSubnode;
			right.parent = mergedSubnode;
			mergedSubnode.ChildRefit(nAda, false);

			// make new subnode for obj
			var newSubnode = new ssBVHNode<GO>(nAda.BVH);
			newSubnode.parent = curNode;
			newSubnode.gobjects = new List<GO> {newOb};
			nAda.MapObjectToBvhLeaf(newOb, newSubnode);
			newSubnode.ComputeVolume(nAda);

			// make assignments..
			curNode.left = mergedSubnode;
			curNode.right = newSubnode;
			curNode.SetDepth(nAda, curNode.depth); // propagate new depths to our children.
			curNode.ChildRefit(nAda);
		}

		private static void AddObject (SSBVHNodeAdaptor<GO> nAda, ssBVHNode<GO> curNode, GO newOb, ref Boundsd newObBox, double newObSAH) {
			// 1. first we traverse the node looking for the best leaf
			while (curNode.gobjects == null) {
				// find the best way to add this object.. 3 options..
				// 1. send to left node  (L+N,R)
				// 2. send to right node (L,R+N)
				// 3. merge and pushdown left-and-right node (L+R,N)

				var left = curNode.left;
				var right = curNode.right;

				double leftSAH = SA(left);
				double rightSAH = SA(right);
				double sendLeftSAH = rightSAH + SA(left.box.ExpandedToFit(newObBox)); // (L+N,R)
				double sendRightSAH = leftSAH + SA(right.box.ExpandedToFit(newObBox)); // (L,R+N)
				double mergedLeftAndRightSAH = SA(BoundsOfPair(left, right)) + newObSAH; // (L+R,N)

				// Doing a merge-and-pushdown can be expensive, so we only do it if it's notably better
				const double MERGE_DISCOUNT = 0.3f;

				if (mergedLeftAndRightSAH < (Math.Min(sendLeftSAH, sendRightSAH)) * MERGE_DISCOUNT) {
					addObject_Pushdown(nAda, curNode, newOb);
					return;
				}

				if (sendLeftSAH < sendRightSAH) {
					curNode = left;
				} else {
					curNode = right;
				}
			}

			// 2. then we add the object and map it to our leaf
			curNode.gobjects.Add(newOb);
			nAda.MapObjectToBvhLeaf(newOb, curNode);
			curNode.RefitVolume(nAda);
			// split if necessary...
			curNode.SplitIfNecessary(nAda);
		}

		internal int CountBvhNodes () {
			if (gobjects != null) {
				return 1;
			}

			return left.CountBvhNodes() + right.CountBvhNodes();
		}

		internal void RemoveObject (SSBVHNodeAdaptor<GO> nAda, GO newOb) {
			if (gobjects == null) {
				throw new Exception("removeObject() called on nonLeaf!");
			}

			nAda.UnmapObject(newOb);
			gobjects.Remove(newOb);
			if (gobjects.Count > 0) {
				RefitVolume(nAda);
			} else {
				// our leaf is empty, so collapse it if we are not the root...
				if (parent != null) {
					gobjects = null;
					parent.RemoveLeaf(nAda, this);
					parent = null;
				}
			}
		}

		void SetDepth (SSBVHNodeAdaptor<GO> nAda, int newdepth) {
			depth = newdepth;
			if (newdepth > nAda.BVH.MaxDepth) {
				nAda.BVH.MaxDepth = newdepth;
			}

			if (gobjects == null) {
				left.SetDepth(nAda, newdepth + 1);
				right.SetDepth(nAda, newdepth + 1);
			}
		}

		private void RemoveLeaf (SSBVHNodeAdaptor<GO> nAda, ssBVHNode<GO> removeLeaf) {
			if (left == null || right == null) {
				throw new Exception("bad intermediate node");
			}

			ssBVHNode<GO> keepLeaf;

			if (removeLeaf == left) {
				keepLeaf = right;
			} else if (removeLeaf == right) {
				keepLeaf = left;
			} else {
				throw new Exception("removeLeaf doesn't match any leaf!");
			}

			// "become" the leaf we are keeping.
			box = keepLeaf.box;
			left = keepLeaf.left;
			right = keepLeaf.right;
			gobjects = keepLeaf.gobjects;
			// clear the leaf..
			// keepLeaf.left = null; keepLeaf.right = null; keepLeaf.gobjects = null; keepLeaf.parent = null;

			if (gobjects == null) {
				left.parent = this;
				right.parent = this; // reassign child parents..
				SetDepth(nAda, depth); // this reassigns depth for our children
			} else {
				// map the objects we adopted to us...
				gobjects.ForEach(o => { nAda.MapObjectToBvhLeaf(o, this); });
			}

			// propagate our new volume..
			if (parent != null) {
				parent.ChildRefit(nAda);
			}
		}

		private void ChildRefit (SSBVHNodeAdaptor<GO> nAda, bool propagate = true) {
			ChildRefit(nAda, this, propagate);
		}

		private static void ChildRefit (SSBVHNodeAdaptor<GO> nAda, ssBVHNode<GO> curNode, bool propagate = true) {
			do {
				ssBVHNode<GO> left = curNode.left;
				ssBVHNode<GO> right = curNode.right;

				// start with the left box
				Boundsd newBox = left.box.ExpandedToFit(right.box);

				// now set our box to the newly created box
				curNode.box = newBox;

				// and walk up the tree
				curNode = curNode.parent;
			} while (propagate && curNode != null);
		}

		internal ssBVHNode (ssBVH<GO> bvh) {
			gobjects = new List<GO>();
			left = right = null;
			parent = null;
			nodeNumber = bvh.NodeCount++;
		}

		internal ssBVHNode (ssBVH<GO> bvh, List<GO> gobjectlist) : this(bvh, null, gobjectlist, 0) { }

		private ssBVHNode (ssBVH<GO> bvh, ssBVHNode<GO> lparent, List<GO> gobjectlist, int curdepth) {
			SSBVHNodeAdaptor<GO> nAda = bvh.nAda;
			nodeNumber = bvh.NodeCount++;

			parent = lparent; // save off the parent BVHGObj Node
			depth = curdepth;

			if (bvh.MaxDepth < curdepth) {
				bvh.MaxDepth = curdepth;
			}

			// Early out check due to bad data
			// If the list is empty then we have no BVHGObj, or invalid parameters are passed in
			if (gobjectlist == null || gobjectlist.Count < 1) {
				throw new Exception("ssBVHNode constructed with invalid paramaters");
			}

			// Check if we’re at our LEAF node, and if so, save the objects and stop recursing.  Also store the min/max for the leaf node and update the parent appropriately
			if (gobjectlist.Count <= bvh.LEAF_OBJ_MAX) {
				// once we reach the leaf node, we must set prev/next to null to signify the end
				left = null;
				right = null;
				// at the leaf node we store the remaining objects, so initialize a list
				gobjects = gobjectlist;
				gobjects.ForEach(o => nAda.MapObjectToBvhLeaf(o, this));
				ComputeVolume(nAda);
				SplitIfNecessary(nAda);
			} else {
				// --------------------------------------------------------------------------------------------
				// if we have more than (bvh.LEAF_OBJECT_COUNT) objects, then compute the volume and split
				gobjects = gobjectlist;
				ComputeVolume(nAda);
				SplitNode(nAda);
				ChildRefit(nAda, false);
			}
		}
	}
}
