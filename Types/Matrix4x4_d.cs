using System;

namespace UnityEngine {
	// ReSharper disable once InconsistentNaming
	public struct Matrix4x4_d {
		public static Matrix4x4 identity {
			get {
				return new Matrix4x4 {
					m00 = 1f,
					m01 = 0.0f,
					m02 = 0.0f,
					m03 = 0.0f,
					m10 = 0.0f,
					m11 = 1f,
					m12 = 0.0f,
					m13 = 0.0f,
					m20 = 0.0f,
					m21 = 0.0f,
					m22 = 1f,
					m23 = 0.0f,
					m30 = 0.0f,
					m31 = 0.0f,
					m32 = 0.0f,
					m33 = 1f
				};
			}
		}

		public double m00;
		public double m10;
		public double m20;
		public double m30;
		public double m01;
		public double m11;
		public double m21;
		public double m31;
		public double m02;
		public double m12;
		public double m22;
		public double m32;
		public double m03;
		public double m13;
		public double m23;
		public double m33;

		public double this [int row, int column] {
			get {
				return this[row + column * 4];
			}
			set {
				this[row + column * 4] = value;
			}
		}

		public double this [int index] {
			get {
				switch (index) {
					case 0:
						return m00;
					case 1:
						return m10;
					case 2:
						return m20;
					case 3:
						return m30;
					case 4:
						return m01;
					case 5:
						return m11;
					case 6:
						return m21;
					case 7:
						return m31;
					case 8:
						return m02;
					case 9:
						return m12;
					case 10:
						return m22;
					case 11:
						return m32;
					case 12:
						return m03;
					case 13:
						return m13;
					case 14:
						return m23;
					case 15:
						return m33;
					default:
						throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
			set {
				switch (index) {
					case 0:
						m00 = value;
						break;
					case 1:
						m10 = value;
						break;
					case 2:
						m20 = value;
						break;
					case 3:
						m30 = value;
						break;
					case 4:
						m01 = value;
						break;
					case 5:
						m11 = value;
						break;
					case 6:
						m21 = value;
						break;
					case 7:
						m31 = value;
						break;
					case 8:
						m02 = value;
						break;
					case 9:
						m12 = value;
						break;
					case 10:
						m22 = value;
						break;
					case 11:
						m32 = value;
						break;
					case 12:
						m03 = value;
						break;
					case 13:
						m13 = value;
						break;
					case 14:
						m23 = value;
						break;
					case 15:
						m33 = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}

		public override int GetHashCode () {
			return GetColumn(0).GetHashCode() ^ GetColumn(1).GetHashCode() << 2 ^ GetColumn(2).GetHashCode() >> 2 ^
			       GetColumn(3).GetHashCode() >> 1;
		}

		public override bool Equals (object other) {
			if (!(other is Matrix4x4_d))
				return false;
			Matrix4x4_d matrix4x4d = (Matrix4x4_d) other;
			return GetColumn(0).Equals(matrix4x4d.GetColumn(0)) && GetColumn(1).Equals(matrix4x4d.GetColumn(1)) &&
			       GetColumn(2).Equals(matrix4x4d.GetColumn(2)) && GetColumn(3).Equals(matrix4x4d.GetColumn(3));
		}

		public static Matrix4x4_d operator * (Matrix4x4_d lhs, Matrix4x4_d rhs) {
			return new Matrix4x4_d {
				m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30,
				m01 = (lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31),
				m02 = (lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32),
				m03 = (lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33),
				m10 = (lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30),
				m11 = (lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31),
				m12 = (lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32),
				m13 = (lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33),
				m20 = (lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30),
				m21 = (lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31),
				m22 = (lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32),
				m23 = (lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33),
				m30 = (lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30),
				m31 = (lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31),
				m32 = (lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32),
				m33 = (lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33)
			};
		}

		public static Vector4_d operator * (Matrix4x4_d lhs, Vector4_d v) {
			Vector4_d vector4d;
			vector4d.x = (lhs.m00 * v.x + lhs.m01 * v.y + lhs.m02 * v.z + lhs.m03 * v.w);
			vector4d.y = (lhs.m10 * v.x + lhs.m11 * v.y + lhs.m12 * v.z + lhs.m13 * v.w);
			vector4d.z = (lhs.m20 * v.x + lhs.m21 * v.y + lhs.m22 * v.z + lhs.m23 * v.w);
			vector4d.w = (lhs.m30 * v.x + lhs.m31 * v.y + lhs.m32 * v.z + lhs.m33 * v.w);
			return vector4d;
		}

		public static bool operator == (Matrix4x4_d lhs, Matrix4x4_d rhs) {
			return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) &&
			       lhs.GetColumn(3) == rhs.GetColumn(3);
		}

		public static bool operator != (Matrix4x4_d lhs, Matrix4x4_d rhs) {
			return !(lhs == rhs);
		}

		public Vector4_d GetColumn (int i) {
			return new Vector4_d(this[0, i], this[1, i], this[2, i], this[3, i]);
		}

		public Vector4_d GetRow (int i) {
			return new Vector4_d(this[i, 0], this[i, 1], this[i, 2], this[i, 3]);
		}

		public void SetColumn (int i, Vector4_d v) {
			this[0, i] = v.x;
			this[1, i] = v.y;
			this[2, i] = v.z;
			this[3, i] = v.w;
		}

		public void SetRow (int i, Vector4_d v) {
			this[i, 0] = v.x;
			this[i, 1] = v.y;
			this[i, 2] = v.z;
			this[i, 3] = v.w;
		}

		public Vector3_d MultiplyPoint (Vector3_d v) {
			Vector3_d vector3d;
			vector3d.x = (m00 * v.x + m01 * v.y + m02 * v.z) + m03;
			vector3d.y = (m10 * v.x + m11 * v.y + m12 * v.z) + m13;
			vector3d.z = (m20 * v.x + m21 * v.y + m22 * v.z) + m23;
			double num = 1f / ((m30 * v.x + m31 * v.y + m32 * v.z) + m33);
			vector3d.x *= num;
			vector3d.y *= num;
			vector3d.z *= num;
			return vector3d;
		}

		public Vector3_d MultiplyPoint3x4 (Vector3_d v) {
			Vector3_d vector3d;
			vector3d.x = (m00 * v.x + m01 * v.y + m02 * v.z) + m03;
			vector3d.y = (m10 * v.x + m11 * v.y + m12 * v.z) + m13;
			vector3d.z = (m20 * v.x + m21 * v.y + m22 * v.z) + m23;
			return vector3d;
		}

		public Vector3_d MultiplyVector (Vector3_d v) {
			Vector3_d vector3d;
			vector3d.x = (m00 * v.x + m01 * v.y + m02 * v.z);
			vector3d.y = (m10 * v.x + m11 * v.y + m12 * v.z);
			vector3d.z = (m20 * v.x + m21 * v.y + m22 * v.z);
			return vector3d;
		}

		public static Matrix4x4_d Scale (Vector3_d v) {
			return new Matrix4x4_d {
				m00 = v.x,
				m01 = 0.0f,
				m02 = 0.0f,
				m03 = 0.0f,
				m10 = 0.0f,
				m11 = v.y,
				m12 = 0.0f,
				m13 = 0.0f,
				m20 = 0.0f,
				m21 = 0.0f,
				m22 = v.z,
				m23 = 0.0f,
				m30 = 0.0f,
				m31 = 0.0f,
				m32 = 0.0f,
				m33 = 1f
			};
		}

		public static Matrix4x4_d Translate (Vector3_d v) {
			return new Matrix4x4_d {
				m00 = 1f,
				m01 = 0.0f,
				m02 = 0.0f,
				m03 = v.x,
				m10 = 0.0f,
				m11 = 1f,
				m12 = 0.0f,
				m13 = v.y,
				m20 = 0.0f,
				m21 = 0.0f,
				m22 = 1f,
				m23 = v.z,
				m30 = 0.0f,
				m31 = 0.0f,
				m32 = 0.0f,
				m33 = 1f
			};
		}

		// TODO transpose, invert, determinant, TRS

		public override string ToString () {
			return string.Format(
				"{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n",
				(object) m00,
				(object) m01,
				(object) m02,
				(object) m03,
				(object) m10,
				(object) m11,
				(object) m12,
				(object) m13,
				(object) m20,
				(object) m21,
				(object) m22,
				(object) m23,
				(object) m30,
				(object) m31,
				(object) m32,
				(object) m33);
		}

		public string ToString (string format) {
			return string.Format(
				"{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n",
				(object) m00.ToString(format),
				(object) m01.ToString(format),
				(object) m02.ToString(format),
				(object) m03.ToString(format),
				(object) m10.ToString(format),
				(object) m11.ToString(format),
				(object) m12.ToString(format),
				(object) m13.ToString(format),
				(object) m20.ToString(format),
				(object) m21.ToString(format),
				(object) m22.ToString(format),
				(object) m23.ToString(format),
				(object) m30.ToString(format),
				(object) m31.ToString(format),
				(object) m32.ToString(format),
				(object) m33.ToString(format));
		}
	}
}
