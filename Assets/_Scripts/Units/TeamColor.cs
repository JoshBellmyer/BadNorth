using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColor : MonoBehaviour {

	[SerializeField] private MaterialGroup[] flatColorParts;
	[SerializeField] private MaterialGroup[] lightColorParts;

	private static bool initialized;
	private static Material[] flatMaterials;
	private static Material[] lightMaterials;


	public void SetColor (int colorId) {
		if (!initialized) {
			InitializeMaterials();
		}

		foreach (MaterialGroup mg in flatColorParts) {
			Material[] materials = mg.meshRenderer.materials;
			materials[mg.materialIndex] = flatMaterials[colorId];
			mg.meshRenderer.materials = materials;
		}
		foreach (MaterialGroup mg in lightColorParts) {
			Material[] materials = mg.meshRenderer.materials;
			materials[mg.materialIndex] = lightMaterials[colorId];
			mg.meshRenderer.materials = materials;
		}
	}

	private void InitializeMaterials () {
		int materialCount = 3;

		flatMaterials = new Material[materialCount];
		lightMaterials = new Material[materialCount];

		for (int i = 0; i < materialCount; i++) {
			flatMaterials[i] = Resources.Load<Material>($"Materials/team_flat_{i}");
			lightMaterials[i] = Resources.Load<Material>($"Materials/team_light_{i}");
		}
	}
}


[System.Serializable]
public class MaterialGroup {

	public MeshRenderer meshRenderer;
	public int materialIndex;
}