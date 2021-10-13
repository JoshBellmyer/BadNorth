using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

	[Header("Settings")]
	[SerializeField] private float speed;
	[SerializeField] private float islandDistance;

	[Header("References")]
	[SerializeField] private Transform[] mountPoints;

	private GameObject cam;
	private bool moving;
	private bool following;
	private Group<Unit> mountedGroup;
	private Rigidbody rb;


	private void Start () {
		rb = GetComponent<Rigidbody>();
	}

	private void Update () {
		if (moving) {
			MoveForward();

			return;
		}
		else if (cam != null && following) {
			FollowCamera();
		}
	}


	public void SetPlayer (PlayerController player) {
		transform.SetParent(player.transform);
		cam = player.GetComponent<CameraController>().camera.gameObject;
		player.Boat = this;
		following = true;
	}

	public void SetSail () {
		moving = true;
	}

	public void MountUnits (Group<Unit> unitGroup) {
		unitGroup.CanMove = false;
		unitGroup.CanAttack = false;

		// TODO: add units to boat

		mountedGroup = unitGroup;
	}

	public void DismountUnits () {
		moving = false;
		following = false;

		if (mountedGroup == null) {
			return;
		}

		// TODO: drop units off at the nearest walkable space

		mountedGroup = null;
	}

	private void MoveForward () {
		Vector3 movement = transform.forward * speed * Time.deltaTime;

		rb.MovePosition(transform.position + movement);
	}

	private void FollowCamera () {
		Vector3 camPos = new Vector3(cam.transform.position.x, 0, cam.transform.position.z);
		Vector3 direction = Vector3.Normalize(camPos);

		transform.forward = direction * -1;
		transform.position = direction * islandDistance;
	}

	private void OnTriggerEnter (Collider other) {
		if (other.tag.Equals("Terrain") && moving) {
			DismountUnits();
		}
	}
}















