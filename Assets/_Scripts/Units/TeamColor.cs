using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColor : MonoBehaviour {

	[SerializeField] private MeshRenderer flatColorParts;
	[SerializeField] private MeshRenderer lightColorParts;

	private static bool initialized;
	private static Material[] flatMaterials;
	private static Material[] lightMaterials;


	public void SetColor () {
		if (!initialized) {
			InitializeMaterials();
		}
	}

	private void InitializeMaterials () {
		int materialCount = 2;

		flatMaterials = new Material[materialCount];
		lightMaterials = new Material[materialCount];

		for (int i = 0; i < materialCount; i++) {
			flatMaterials[i] = Resources.Load<Material>($"Materials/team_flat_{i + 1}");
			lightMaterials[i] = Resources.Load<Material>($"Materials/team_light_{i + 1}");
		}
	}
}