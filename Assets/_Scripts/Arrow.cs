using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

	public string team;
	public bool vectorB;
	public ArcherUnit launcher;
	

	private void Update () {
		if (Game.online && !Game.isHost) {
			return;
		}
		
		if (transform.position.y < -1) {
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter (Collider other) {
		if (Game.online && !Game.isHost) {
			return;
		}
		if (other.isTrigger) {
			return;
		}

		Unit unit = other.GetComponent<Unit>();

		if (unit != null) {
			if (unit.Team != team) {
				// Debug.Log(other.gameObject);
				unit.GetComponent<DamageHelper>().TakeDamage(DamageType.Piercing, GetComponent<Rigidbody>().velocity);

				Destroy(gameObject);
			}

			return;
		}

		launcher.UseVector(!vectorB);

		Destroy(gameObject);
	}

	public void Setup (string team, ArcherUnit launcher, bool vectorB) {
		this.team = team;
		this.launcher = launcher;
		this.vectorB = vectorB;
	}
}