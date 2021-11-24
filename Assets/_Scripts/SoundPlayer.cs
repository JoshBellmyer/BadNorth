using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

	public static SoundPlayer instance;

	private static AudioSource[] sources;
	private static Dictionary<string, AudioClip> audioClips;


	private void Awake () {
		if (instance == null || instance == this) {
			instance = this;
		}
		else {
			Destroy(gameObject);
		}

		LoadSounds();

		AudioSource sourcePrefab = Resources.Load<AudioSource>("Prefabs/AudioSource");
		sources = new AudioSource[8];

		for (int i = 0; i < 8; i++) {
			sources[i] = Instantiate<AudioSource>(sourcePrefab);
			sources[i].transform.parent = instance.transform;
		}
	}

	public static void PlaySound (string soundName, float volume) {
		if (instance == null) {
			return;
		}
		if (!audioClips.ContainsKey(soundName)) {
			return;
		}

		AudioSource source = null;

		for (int i = 0; i < sources.Length; i++) {
			if (!sources[i].isPlaying) {
				source = sources[i];

				break;
			}
		}

		if (source == null) {
			source = sources[Random.Range(0, sources.Length)];
		}

		source.clip = audioClips[soundName];
		source.volume = volume;
		source.pitch = Random.Range(0.75f, 1.25f);
		source.Play();
	}

	private void LoadSounds () {
		AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/");
		audioClips = new Dictionary<string, AudioClip>();

		foreach (AudioClip ac in clips) {
			audioClips.Add(ac.name, ac);
		}
	}
}