using System;

namespace UnityEngine {
	// ReSharper disable once InconsistentNaming
	public static class Math_d {
		public const double PI = 3.14159265358979;
		public const double Infinity = double.PositiveInfinity;
		public const double NegativeInfinity = double.NegativeInfinity;
		public const double Deg2Rad = 0.01745329;
		public const double Rad2Deg = 57.29578;
		public const double Epsilon = 1.401298E-45;

		public static double Sin (double d) {
			return Math.Sin(d);
		}

		public static double Cos (double d) {
			return Math.Cos(d);
		}

		public static double Tan (double d) {
			return Math.Tan(d);
		}

		public static double Asin (double d) {
			return Math.Asin(d);
		}

		public static double Acos (double d) {
			return Math.Acos(d);
		}

		public static double Atan (double d) {
			return Math.Atan(d);
		}

		public static double Atan2 (double y, double x) {
			return Math.Atan2(y, x);
		}

		public static double Sqrt (double d) {
			return Math.Sqrt(d);
		}

		public static double Abs (double d) {
			return Math.Abs(d);
		}

		public static int Abs (int value) {
			return Math.Abs(value);
		}

		public static double Min (double a, double b) {
			if (a < b)
				return a;
			return b;
		}

		public static double Min (params double[] values) {
			int length = values.Length;
			if (length == 0)
				return 0.0;
			double num = values[0];
			for (int index = 1; index < length; ++index) {
				if (values[index] < num)
					num = values[index];
			}

			return num;
		}

		public static int Min (int a, int b) {
			if (a < b)
				return a;
			return b;
		}

		public static int Min (params int[] values) {
			int length = values.Length;
			if (length == 0)
				return 0;
			int num = values[0];
			for (int index = 1; index < length; ++index) {
				if (values[index] < num)
					num = values[index];
			}

			return num;
		}

		public static double Max (double a, double b) {
			if (a > b)
				return a;
			return b;
		}

		public static double Max (params double[] values) {
			int length = values.Length;
			if (length == 0)
				return 0.0;
			double num = values[0];
			for (int index = 1; index < length; ++index) {
				if (values[index] > num)
					num = values[index];
			}

			return num;
		}

		public static int Max (int a, int b) {
			return a > b ? a : b;
		}

		public static int Max (params int[] values) {
			int length = values.Length;
			if (length == 0)
				return 0;
			int num = values[0];
			for (int index = 1; index < length; ++index) {
				if (values[index] > num)
					num = values[index];
			}

			return num;
		}

		public static double Pow (double d, double p) {
			return Math.Pow(d, p);
		}

		public static double Exp (double power) {
			return Math.Exp(power);
		}

		public static double Log (double d, double p) {
			return Math.Log(d, p);
		}

		public static double Log (double d) {
			return Math.Log(d);
		}

		public static double Log10 (double d) {
			return Math.Log10(d);
		}

		public static double Ceil (double d) {
			return Math.Ceiling(d);
		}

		public static double Floor (double d) {
			return Math.Floor(d);
		}

		public static double Round (double d) {
			return Math.Round(d);
		}

		public static int CeilToInt (double d) {
			return (int) Math.Ceiling(d);
		}

		public static int FloorToInt (double d) {
			return (int) Math.Floor(d);
		}

		public static int RoundToInt (double d) {
			return (int) Math.Round(d);
		}

		public static double Sign (double d) {
			return d >= 0.0 ? 1.0 : -1.0;
		}

		public static double Clamp (double value, double min, double max) {
			if (value < min)
				value = min;
			else if (value > max)
				value = max;
			return value;
		}

		public static int Clamp (int value, int min, int max) {
			if (value < min)
				value = min;
			else if (value > max)
				value = max;
			return value;
		}

		public static double Clamp01 (double value) {
			if (value < 0.0)
				return 0.0;
			if (value > 1.0)
				return 1.0;
			return value;
		}

		public static double Lerp (double from, double to, double t) {
			return from + (to - from) * Clamp01(t);
		}

		public static double LerpAngle (double a, double b, double t) {
			double num = Repeat(b - a, 360.0);
			if (num > 180.0)
				num -= 360.0;
			return a + num * Clamp01(t);
		}

		public static double MoveTowards (double current, double target, double maxDelta) {
			if (Abs(target - current) <= maxDelta)
				return target;
			return current + Sign(target - current) * maxDelta;
		}

		public static double MoveTowardsAngle (double current, double target, double maxDelta) {
			target = current + DeltaAngle(current, target);
			return MoveTowards(current, target, maxDelta);
		}

		public static double SmoothStep (double from, double to, double t) {
			t = Clamp01(t);
			t = (-2.0 * t * t * t + 3.0 * t * t);
			return to * t + from * (1.0 - t);
		}

		public static double Gamma (double value, double absmax, double gamma) {
			bool flag = value < 0.0;
			double num1 = Abs(value);
			if (num1 > absmax) {
				if (flag)
					return -num1;
				return num1;
			}

			double num2 = Pow(num1 / absmax, gamma) * absmax;
			if (flag)
				return -num2;
			return num2;
		}

		public static bool Approximately (double a, double b) {
			return Abs(b - a) < Max(1E-06 * Max(Abs(a), Abs(b)), 1.121039E-44);
		}

		public static double SmoothDamp (double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed) {
			double deltaTime = Time.deltaTime;
			return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static double SmoothDamp (double current, double target, ref double currentVelocity, double smoothTime) {
			double deltaTime = Time.deltaTime;
			double maxSpeed = Infinity;
			return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static double SmoothDamp (double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime) {
			smoothTime = Max(0.0001, smoothTime);
			double num1 = 2.0 / smoothTime;
			double num2 = num1 * deltaTime;
			double num3 = (1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2));
			double num4 = current - target;
			double num5 = target;
			double max = maxSpeed * smoothTime;
			double num6 = Clamp(num4, -max, max);
			target = current - num6;
			double num7 = (currentVelocity + num1 * num6) * deltaTime;
			currentVelocity = (currentVelocity - num1 * num7) * num3;
			double num8 = target + (num6 + num7) * num3;
			if (num5 - current > 0.0 == num8 > num5) {
				num8 = num5;
				currentVelocity = (num8 - num5) / deltaTime;
			}

			return num8;
		}

		public static double SmoothDampAngle (double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed) {
			double deltaTime = Time.deltaTime;
			return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static double SmoothDampAngle (double current, double target, ref double currentVelocity, double smoothTime) {
			double deltaTime = Time.deltaTime;
			const double maxSpeed = Infinity;
			return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static double SmoothDampAngle (
			double current,
			double target,
			ref double currentVelocity,
			double smoothTime,
			double maxSpeed,
			double deltaTime) {
			target = current + DeltaAngle(current, target);
			return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		public static double Repeat (double t, double length) {
			return t - Floor(t / length) * length;
		}

		public static double PingPong (double t, double length) {
			t = Repeat(t, length * 2.0);
			return length - Abs(t - length);
		}

		public static double InverseLerp (double from, double to, double value) {
			if (from < to) {
				if (value < from)
					return 0.0;
				if (value > to)
					return 1.0;
				value -= from;
				value /= to - from;
				return value;
			}

			if (from <= to)
				return 0.0;
			if (value < to)
				return 1.0;
			if (value > from)
				return 0.0;
			return (1.0 - (value - to) / (@from - to));
		}

		public static double DeltaAngle (double current, double target) {
			double num = Repeat(target - current, 360.0);
			if (num > 180.0)
				num -= 360.0;
			return num;
		}

		internal static bool LineIntersection (Vector2_d p1, Vector2_d p2, Vector2_d p3, Vector2_d p4, ref Vector2_d result) {
			double num1 = p2.x - p1.x;
			double num2 = p2.y - p1.y;
			double num3 = p4.x - p3.x;
			double num4 = p4.y - p3.y;
			double num5 = num1 * num4 - num2 * num3;
			if (num5 == 0.0)
				return false;
			double num6 = p3.x - p1.x;
			double num7 = p3.y - p1.y;
			double num8 = (num6 * num4 - num7 * num3) / num5;
			result = new Vector2_d(p1.x + num8 * num1, p1.y + num8 * num2);
			return true;
		}

		internal static bool LineSegmentIntersection (Vector2_d p1, Vector2_d p2, Vector2_d p3, Vector2_d p4, ref Vector2_d result) {
			double num1 = p2.x - p1.x;
			double num2 = p2.y - p1.y;
			double num3 = p4.x - p3.x;
			double num4 = p4.y - p3.y;
			double num5 = (num1 * num4 - num2 * num3);
			if (num5 == 0.0)
				return false;
			double num6 = p3.x - p1.x;
			double num7 = p3.y - p1.y;
			double num8 = (num6 * num4 - num7 * num3) / num5;
			if (num8 < 0.0 || num8 > 1.0)
				return false;
			double num9 = (num6 * num2 - num7 * num1) / num5;
			if (num9 < 0.0 || num9 > 1.0)
				return false;
			result = new Vector2_d(p1.x + num8 * num1, p1.y + num8 * num2);
			return true;
		}

		// https://en.wikipedia.org/wiki/Fast_inverse_square_root
		public static double InverseSqrtFast (double x) {
			double xhalf = 0.5 * x;
			long l = BitConverter.DoubleToInt64Bits(x);
			l = 0x5FE6EB50C7B537A9 - (l >> 1);
			double y = BitConverter.Int64BitsToDouble(l);
			y *= 1.5 - xhalf * y * y;
			return y;
		}

		// http://geomalgorithms.com/a07-_distance.html#dist3D_Segment_to_Segment()
		// you an test distance to a point by setting the first two parameters to the same vector (it doesn't work if the point's coordinates are
		// supplied in the last two parameters)
		public static Vector3_d[] ClosestSegmentToSegmentPoints (Vector3_d line1p0, Vector3_d line1p1, Vector3_d line2p0, Vector3_d line2p1) {
			const double smallNumber = 0.00000001;

			if (line1p0 == line1p1 && line2p0 == line2p1) {
				return new[] {line1p0, line2p0};
			}

			Vector3_d u = line1p1 - line1p0;
			Vector3_d v = line2p1 - line2p0;
			Vector3_d w = line1p0 - line2p0;

			double a = Vector3_d.Dot(u, u); // always >= 0
			double b = Vector3_d.Dot(u, v);
			double c = Vector3_d.Dot(v, v); // always >= 0
			double d = Vector3_d.Dot(u, w);
			double e = Vector3_d.Dot(v, w);

			double D = a * c - b * b; // always >= 0

			double sN, sD = D; // sc = sN / sD, default sD = D >= 0
			double tN, tD = D; // tc = tN / tD, default tD = D >= 0

			// compute the line parameters of the two closest points
			if (D < smallNumber) { // the lines are almost parallel
				sN = 0.0; // force using point P0 on segment S1
				sD = 1.0; // to prevent possible division by 0.0 later
				tN = e;
				tD = c;
			} else { // get the closest points on the infinite lines
				sN = (b*e - c*d);
				tN = (a*e - b*d);

				if (sN < 0.0) { // sc < 0 => the s=0 edge is visible
					sN = 0.0;
					tN = e;
					tD = c;
				} else if (sN > sD) { // sc > 1  => the s=1 edge is visible
					sN = sD;
					tN = e + b;
					tD = c;
				}
			}

			if (tN < 0.0) { // tc < 0 => the t=0 edge is visible
				tN = 0.0;

				// recompute sc for this edge
				if (-d < 0.0) {
					sN = 0.0;
				} else if (-d > a) {
					sN = sD;
				}  else {
					sN = -d;
					sD = a;
				}
			} else if (tN > tD) { // tc > 1  => the t=1 edge is visible
				tN = tD;

				// recompute sc for this edge
				if ((-d + b) < 0.0) {
					sN = 0;
				} else if ((-d + b) > a) {
					sN = sD;
				} else {
					sN = (-d +  b);
					sD = a;
				}
			}

			// finally do the division to get sc and tc
			var sc = (Abs(sN) < smallNumber ? 0.0 : sN / sD);
			var tc = (Abs(tN) < smallNumber ? 0.0 : tN / tD);

			return new[] {
				line1p0 + sc * u,
				line2p0 + tc * v
			};
		}

		public static double ClosestSegmentToSegmentDistance (Vector3_d line1p0, Vector3_d line1p1, Vector3_d line2p0, Vector3_d line2p1) {
			Vector3_d[] points = ClosestSegmentToSegmentPoints(line1p0, line1p1, line2p0, line2p1);
			return Vector3_d.Distance(points[0], points[1]);
		}

		public static double ClosestSegmentToSegmentSqrDistance (Vector3_d line1p0, Vector3_d line1p1, Vector3_d line2p0, Vector3_d line2p1) {
			Vector3_d[] points = ClosestSegmentToSegmentPoints(line1p0, line1p1, line2p0, line2p1);
			return Vector3_d.SqrDistance(points[0], points[1]);
		}

		public static Vector3_d ClosestSegmentToSegmentMidPoint (Vector3_d line1p0, Vector3_d line1p1, Vector3_d line2p0, Vector3_d line2p1) {
			Vector3_d[] points = ClosestSegmentToSegmentPoints(line1p0, line1p1, line2p0, line2p1);
			return (points[0] + points[1]) / 2.0;
		}
	}
}
