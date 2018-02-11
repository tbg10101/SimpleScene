// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.
// Converted to Unity 64-bit by Tristan Bellman-Greenwood

using UnityEngine;

namespace SimpleScene {
	public abstract class SSObject {
		public delegate void PositionOrSizeChangedHandler(SSObject sender);
		public event PositionOrSizeChangedHandler OnPositionOrSizeChanged;

		private Vector3d Pos = Vector3d.zero;
		public Vector3d _pos {
			get {
				return Pos;
			}
			set {
				if (Pos.Equals(value)) {
					return;
				}

				Pos = value;
				NotifyPositionOrSizeChanged();
			}
		}

		private double Extents = 1.0;
		public double _extents {
			get {
				return Extents;
			}
			set {
				if (Extents.Equals(value)) {
					return;
				}

				Extents = value;
				NotifyPositionOrSizeChanged();
			}
		}

		private void NotifyPositionOrSizeChanged () {
			if (OnPositionOrSizeChanged != null) {
				OnPositionOrSizeChanged(this);
			}
		}
	}
}
