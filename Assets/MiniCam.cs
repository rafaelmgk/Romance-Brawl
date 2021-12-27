using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MiniCam : MonoBehaviour {
	[SerializeField] Transform miniCamPosition;
	public RawImage image;
	public Camera miniCam;
	public RenderTexture rt;
	private Quaternion my_rotation;


	Vector3 offset;
	public GameObject[] respawns;
	private void Awake() {
		offset = transform.position - miniCamPosition.position;
	}
	private void Start() {
		my_rotation = this.transform.rotation;
		CreateRenderTexture();
	}
	void Update() {
		respawns = GameObject.FindGameObjectsWithTag("Player");
		miniCam.targetTexture = rt;
		image.texture = rt;
		this.transform.rotation = my_rotation;
	}


	private void LateUpdate() {
		transform.position = miniCamPosition.position + offset;
	}
	private void CreateRenderTexture() {
		rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
		rt.Create();
	}
}