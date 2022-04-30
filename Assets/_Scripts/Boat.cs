using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

	[Header("Settings")]
	[SerializeField] private float speed;
	[SerializeField] private float islandDistance;

	[Header("References")]
	[SerializeField] private Transform[] mountPoints;
	[SerializeField] private GameObject laser;
	[SerializeField] private MeshRenderer laserMesh;
	[SerializeField] private GameObject selector;
	[SerializeField] private NetworkBoat networkBoat;

	[Header("Materials")]
	[SerializeField] private Material red;
	[SerializeField] private Material green;

	[SerializeField] private GameObject cam;
	private bool moving;
	private bool following;
	private Group mountedGroup;
	private Rigidbody rb;
	private List<Unit> mountedUnits = new List<Unit>();

	private bool canSail;
	private bool sailed;
	private Vector3 dismountPos;
	[SerializeField] private Player player;
	private int _playerId;

	private bool setColor = false;
	private bool destroyed = false;

	public int PlayerId {
        get {
            if (Game.online) {
                return networkBoat.playerId.Value;
            }
            else {
                return _playerId;
            }
        }
        set { 
            if (Game.online) {
                _playerId = value;
                networkBoat.SetPlayerIdServerRpc(value);
            }
            else {
                _playerId = value; 
            }
        }
    }


	private void Start () {
		rb = GetComponent<Rigidbody>();
		canSail = false;

		if (!Game.online) {
			GetComponent<TeamColor>().SetColor(PlayerId);
		}
	}

	private void Update () {
		if (Game.instance.isPaused) {
			return;
		}

		// Set color for online game
		if (Game.online && !setColor) {
			if (PlayerId > 0) {
				GetComponent<TeamColor>().SetColor(PlayerId);
				setColor = true;
			}
		}

		if (moving) {
			MoveForward();

			return;
		}
		else if (cam != null && following) {
			FollowCamera();
			UpdateLaser();
		}

		if (!following && sailed) {
			Sink();
		}
	}


	private void UpdateLaser () {
		RaycastHit hit;
		bool hitTerrain = Physics.Raycast(transform.position + new Vector3(0, 0.7f, 0) + transform.forward * 1f, transform.forward, out hit, LayerMask.GetMask("Terrain"));

		if (hitTerrain) {
			laser.transform.localScale = new Vector3(1, 1, hit.distance);
			selector.transform.position = GridUtils.GetTopFromSide(hit.point, hit.normal);

			// Collider[] cols = Physics.OverlapSphere(selector.transform.position + new Vector3(0, 0.75f, 0), 0.35f, LayerMask.GetMask("Terrain"));
			bool terrainAbove = GridUtils.CheckForTerrainAbove(selector.transform.position);

			if (!terrainAbove && selector.transform.position.y <= 1f) {
				canSail = true;
				dismountPos = selector.transform.position;
				selector.SetActive(true);
				laserMesh.sharedMaterial = green;
			}
			else {
				canSail = false;
				selector.SetActive(false);
				laserMesh.sharedMaterial = red;
			}
		}
		else {
			canSail = false;
			selector.SetActive(false);
			laserMesh.sharedMaterial = red;
		}
	}

	public void SetPlayer (Player player) {
		// ransform.SetParent(player.transform);
		cam = player.camera.gameObject;
		player.Boat = this;
		following = true;
		sailed = false;
		this.player = player.GetComponent<Player>();

		PlayerId = this.player.playerId;
		selector.transform.SetParent(null);
		selector.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer($"Player {this.player.playerId}");
		laserMesh.gameObject.layer = LayerMask.NameToLayer($"Player {this.player.playerId}");
		laser.SetActive(true);

		player.GetComponent<CameraController>().ZoomOut();
	}

	public bool SetSail () {
		if (!canSail || sailed) {
			return false;
		}

		laser.SetActive(false);
		selector.SetActive(false);
		moving = true;
		sailed = true;

		return true;
	}

	public void CancelDeploy()
	{
		laser.SetActive(false);
		selector.SetActive(false);

		if (player != null) {
			player.Boat = null;
		}

		DestroyMountedUnits();
	}

	public void MountUnits (List<Unit> unitList) {
		int index = 0;

		foreach (Unit u in unitList) {
			// u.transform.position = mountPoints[index].position;
			// u.transform.eulerAngles = mountPoints[index].transform.eulerAngles;
			// u.transform.SetParent(transform);
			Game.SetPosition(u.gameObject, mountPoints[index].position);
			Game.SetRotation(u.gameObject, mountPoints[index].transform.eulerAngles);
			Game.SetParent(u.gameObject, gameObject);
			mountedUnits.Add(u);

			index++;
		}

		mountedGroup = unitList[0].Group;
		mountedGroup.SetAgentEnabled(false);
		mountedGroup.SetInBoat(true);
	}

	public void DismountUnits () {
		moving = false;
		following = false;

		foreach (Unit u in mountedUnits) {
			if (u == null) {
				return;
			}

			// u.transform.SetParent(null);
			Game.SetParent(u.gameObject, null);
			TeamManager.instance.Add(u.Team, u);
		}

		mountedGroup.TeleportTo(dismountPos);
		mountedGroup.SetAgentEnabled(true);
		mountedGroup.SetInBoat(false);
		
		mountedGroup = null;
		mountedUnits.Clear();

		transform.RotateAround(transform.position, transform.right, -10);
		// Game.SetRotation(gameObject, transform.eulerAngles);
		// Game.SetPosition(gameObject, new Vector3(0, 0.2f, 0));
		transform.position += new Vector3(0, 0.2f, 0);

		SoundPlayer.PlaySound(Sound.Boat, 1, false);
	}

	private void DestroyMountedUnits () {
		foreach (Unit u in mountedUnits) {
			TeamManager.instance.Remove(u.Team, u);
			Game.DestroyObject(u.gameObject);
		}
	}

	private void MoveForward () {
		Vector3 movement = transform.forward * speed * Time.deltaTime;

		transform.position += movement;
		// Game.SetPosition(gameObject, transform.position + movement);
	}

	private void Sink () {
		float sinkSpeed = 0.6f;
		transform.position += new Vector3(0, -sinkSpeed * Time.deltaTime, 0);
		// Game.SetPosition(gameObject, transform.position + new Vector3(0, -sinkSpeed * Time.deltaTime, 0));

		if (transform.position.y < -2.5f && !destroyed) {
			player.Boat = null;
			Game.DestroyObject(gameObject);
			destroyed = true;
		}
	}

	private void FollowCamera () {
		Vector3 camPos = new Vector3(cam.transform.position.x, 0, cam.transform.position.z);
		Vector3 direction = Vector3.Normalize(camPos);

		transform.forward = direction * -1;
		// Game.SetRotation(gameObject, transform.eulerAngles);
		// Game.SetPosition(gameObject, direction * islandDistance);
		transform.position = direction * islandDistance;
	}

	private void OnTriggerStay (Collider other) {
		if (other.tag.Equals("Terrain") && moving) {
			if (Vector3.Distance(dismountPos, transform.position) > 3) {
				return;
			}

			DismountUnits();
		}
	}
}















