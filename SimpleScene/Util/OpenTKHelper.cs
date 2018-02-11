// Copyright(C) David W. Jeske, 2013
// Released to the public domain.
// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using System;
using UnityEngine;

namespace SimpleScene {
	public static class OpenTKHelper {
		public static bool IntersectRayAABox1 (SSRay ray, SSAABB box, ref double tnear, ref double tfar) {
			// r.dir is unit direction vector of ray
			Vector3d dirfrac = new Vector3d {
				x = 1.0f / ray.Direction.x,
				y = 1.0f / ray.Direction.y,
				z = 1.0f / ray.Direction.z
			};
			// lb is the corner of AABB with minimal coordinates - left bottom, rt is maximal corner
			// r.org is origin of ray
			double t1 = (box.Min.x - ray.Position.x) * dirfrac.x;
			double t2 = (box.Max.x - ray.Position.x) * dirfrac.x;
			double t3 = (box.Min.y - ray.Position.y) * dirfrac.y;
			double t4 = (box.Max.y - ray.Position.y) * dirfrac.y;
			double t5 = (box.Min.z - ray.Position.z) * dirfrac.z;
			double t6 = (box.Max.z - ray.Position.z) * dirfrac.z;

			double tmin = Math.Max(Math.Max(Math.Min(t1, t2), Math.Min(t3, t4)), Math.Min(t5, t6));
			double tmax = Math.Min(Math.Min(Math.Max(t1, t2), Math.Max(t3, t4)), Math.Max(t5, t6));

			// if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
			if (tmax < 0) {
				return false;
			}

			// if tmin > tmax, ray doesn't intersect AABB
			if (tmin > tmax) {
				return false;
			}

			return true;
		}

		// Ray to AABB (AxisAlignedBoundingBox)
		// http://gamedev.stackexchange.com/questions/18436/most-efficient-aabb-vs-ray-collision-algorithms
		public static bool IntersectRayAABox2 (SSRay ray, SSAABB box, ref double tnear, ref double tfar) {
			Vector3d T_1 = new Vector3d();
			Vector3d T_2 = new Vector3d(); // vectors to hold the T-values for every direction
			double t_near = double.MinValue; // maximums defined in double.h
			double t_far = double.MaxValue;

			for (int i = 0; i < 3; i++) { //we test slabs in every direction
				if (ray.Direction[i] == 0) { // ray parallel to planes in this direction
					if ((ray.Position[i] < box.Min[i]) || (ray.Position[i] > box.Max[i])) {
						return false; // parallel AND outside box : no intersection possible
					}
				} else { // ray not parallel to planes in this direction
					T_1[i] = (box.Min[i] - ray.Position[i]) / ray.Direction[i];
					T_2[i] = (box.Max[i] - ray.Position[i]) / ray.Direction[i];

					if (T_1[i] > T_2[i]) { // we want T_1 to hold values for intersection with near plane
						var swp = T_2; // swap
						T_1 = swp;
						T_2 = T_1;
					}

					if (T_1[i] > t_near) {
						t_near = T_1[i];
					}

					if (T_2[i] < t_far) {
						t_far = T_2[i];
					}

					if ((t_near > t_far) || (t_far < 0)) {
						return false;
					}
				}
			}

			tnear = t_near;
			tfar = t_far; // put return values in place
			return true; // if we made it here, there was an intersection - YAY
		}

		public static void TwoPerpAxes (Vector3d zAxis, out Vector3d xAxis, out Vector3d yAxis, double delta = 0.01f) {
			// pick two perpendicular axes to an axis
			zAxis.Normalize();
			if (Math.Abs(zAxis.x) < delta
			    && Math.Abs(zAxis.y) < delta) { // special case
				xAxis = Vector3d.right;
			} else {
				xAxis = new Vector3d(zAxis.y, -zAxis.x, 0.0f).normalized;
			}

			yAxis = Vector3d.Cross(zAxis, xAxis);
		}

		public static Vector3d ProjectCoord (Vector3d pt, Vector3d dirX, Vector3d dirY, Vector3d dirZ) {
			// projects a point onto 3 axes
			// (assumes dir vectors are unit length)
			Vector3d ret;
			ret.x = Vector3d.Dot(pt, dirX);
			ret.y = Vector3d.Dot(pt, dirY);
			ret.z = Vector3d.Dot(pt, dirZ);
			return ret;
		}

		public static bool RectsOverlap (Vector2 r1Min, Vector2 r1Max, Vector2 r2Min, Vector2 r2Max) {
			// return true when two rectangles overlap in 2D
			return !(r1Max.x < r2Min.x || r2Max.x < r1Min.x || r1Max.y < r2Min.y || r2Max.y < r1Min.y);
		}
	}
}
