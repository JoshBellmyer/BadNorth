using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner {

	private static List<GameObject> particles;

	public static void LoadParticles () {
		particles = new List<GameObject>();

		int index = 1;
		GameObject tempParticle = Resources.Load<GameObject>($"Particles/Particle {index}");

		while (tempParticle != null) {
			particles.Add(tempParticle);
			index++;

			tempParticle = Resources.Load<GameObject>($"Particles/Particle {index}");
		}
	}

	public static GameObject SpawnParticle (Vector3 pos, int particleIndex, float despawn) {
		if (particleIndex >= particles.Count) {
			return null;
		}

		GameObject newParticle = GameObject.Instantiate<GameObject>(particles[particleIndex]);
		newParticle.transform.position = pos;

		if (despawn > 0) {
			GameObject.Destroy(newParticle, despawn);
		}

		return newParticle;
	}
}