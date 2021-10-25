using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite3D : MonoBehaviour {

	[HideInInspector] public new Camera camera;

	private void Update () {
		if (camera == null) {
			return;
		}

		transform.forward = -camera.transform.forward;
	}
}