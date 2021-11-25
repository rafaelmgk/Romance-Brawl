using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldData : MonoBehaviour {
	public struct WorldBounds {
		public float leftBound;
		public float rightBound;
		public float upBound;
		public float downBound;

		public WorldBounds(float left, float right, float up, float down) {
			leftBound = left;
			rightBound = right;
			upBound = up;
			downBound = down;
		}
	}

	public struct CameraLimits {
		public float leftLimit;
		public float rightLimit;
		public float upLimit;
		public float downLimit;

		public CameraLimits(float left, float right, float up, float down) {
			leftLimit = left;
			rightLimit = right;
			upLimit = up;
			downLimit = down;
		}
	}

	[Header("World Bounds")]
	[SerializeField] private float _leftBound;
	[SerializeField] private float _rightBound;
	[SerializeField] private float _upBound;
	[SerializeField] private float _downBound;


	[Header("Camera Limits")]
	[SerializeField] private float _leftLimit;
	[SerializeField] private float _rightLimit;
	[SerializeField] private float _upLimit;
	[SerializeField] private float _downLimit;

	/// <summary>
	/// This function creates and returns a WorldBounds using in-class data.
	/// </summary>
	/// <returns></returns>
	public WorldBounds GenerateWorldBounds() {
		WorldBounds worldBounds = new WorldBounds(_leftBound, _rightBound, _upBound, _downBound);
		return worldBounds;
	}

	/// <summary>
	/// This function creates and returns a CameraLimits using in-class data.
	/// </summary>
	/// <returns></returns>
	public CameraLimits GenerateCameraLimits() {
		CameraLimits cameraLimits = new CameraLimits(_leftLimit, _rightLimit, _upLimit, _downLimit);
		return cameraLimits;
	}
}
