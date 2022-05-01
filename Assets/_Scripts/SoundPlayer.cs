using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

	public AudioClip titleMusic;

	public static SoundPlayer instance;

	private static AudioSource[] sources;
	private static AudioSource musicSource;
	private static Dictionary<string, AudioClip[]> audioClips;


	private void Awake () {
		if (instance == null || instance == this) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);

			return;
		}

		LoadSounds();

		AudioSource sourcePrefab = Resources.Load<AudioSource>("Prefabs/AudioSource");
		sources = new AudioSource[8];

		for (int i = 0; i < 8; i++) {
			sources[i] = Instantiate<AudioSource>(sourcePrefab);
			sources[i].transform.parent = instance.transform;
		}

		musicSource = Instantiate<AudioSource>(sourcePrefab);
		musicSource.transform.parent = instance.transform;
		musicSource.loop = true;
	}

	public static void PlayTitleMusic () {
		if (musicSource.clip == instance.titleMusic && musicSource.isPlaying) {
			return;
		}

		musicSource.clip = instance.titleMusic;
		musicSource.volume = 0.6f;
		musicSource.Play();
	}

	public static void StopTitleMusic () {
		musicSource.Stop();
	}

	// Play a sound clip directly if in local mode, otherwise if online send a ClientRpc to play the sound for both players
	public static void PlaySound (Sound sound, float volume, bool varyPitch) {
		if (Game.online) {
			if (Game.isHost) {
				Game.GetLocalPlayer().PlaySoundClientRpc(sound, volume, varyPitch);
			}
			else {
				Game.GetLocalPlayer().PlaySoundServerRpc(sound, volume, varyPitch);
			}
		}
		else {
			PlaySoundLocal(sound, volume, varyPitch);
		}
	}

	// Play a sound clip on the local client only, regardless of online or host status
	public static void PlaySoundLocal(Sound sound, float volume, bool varyPitch) {
		if (instance == null) {
			return;
		}
		if (!audioClips.ContainsKey($"{sound}")) {
			return;
		}

		AudioClip[] clips = audioClips[$"{sound}"];
		int index = Random.Range(0, clips.Length);
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

		source.clip = clips[index];
		source.volume = volume;
		
		if (varyPitch) {
			source.pitch = Random.Range(0.75f, 1.25f);
		}
		else {
			source.pitch = 1;
		}

		source.Play();
	}

	private void LoadSounds () {
		audioClips = new Dictionary<string, AudioClip[]>();

		foreach (string soundName in System.Enum.GetNames(typeof(Sound))) {
			AudioClip[] clips = Resources.LoadAll<AudioClip>($"Sounds/{soundName}/");
			audioClips.Add(soundName, clips);
		}
	}
}


public enum Sound {
	ArrowDamage = 0,
	ArrowFling = 1,
	AxeStrike = 2,
	Boat = 3,
	Death = 4,
	GameStart = 5,
	GameEnd = 6,
	Ladder = 7,
	MenuMove = 8,
	MenuClick = 9,
	PikeStrike = 10,
	SquadWipe = 11,
	SwordStrike = 12,
}













