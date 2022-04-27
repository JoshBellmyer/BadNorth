using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [Range(1, 2)]
    [SerializeField] float volumeScale = 1.5f;
    [SerializeField] float fadeTime = 1f;
    [SerializeField] AudioClip mainTheme;
    [SerializeField] AudioClip specialTheme;
    [SerializeField] List<UnitTheme> themes;
    AudioSource mainThemePlayer;
    AudioSource specialThemePlayer;
    bool specialThemePlaying;
    Dictionary<UnitType, AudioSource> players;
    Dictionary<UnitType, int> unitCounts;

    private void Start()
    {
        mainThemePlayer = gameObject.AddComponent<AudioSource>();
        mainThemePlayer.clip = mainTheme;
        mainThemePlayer.loop = true;

        specialThemePlayer = gameObject.AddComponent<AudioSource>();
        specialThemePlayer.clip = specialTheme;
        specialThemePlayer.loop = true;
        specialThemePlayer.volume = 0;

        players = new Dictionary<UnitType, AudioSource>();
        unitCounts = new Dictionary<UnitType, int>();
        foreach (var theme in themes)
        {
            var player = gameObject.AddComponent<AudioSource>();
            player.clip = theme.clip;
            player.volume = 0;
            player.loop = true;
            players.Add(theme.type, player);
            unitCounts.Add(theme.type, 0);
        }

        Play();
    }

    private void Update()
    {
        foreach (var count in unitCounts)
        {
            if (count.Value <= 0)
            {
                if(specialThemePlaying)
                {
                    StartCoroutine(Fade(specialThemePlayer, 0));
                    specialThemePlaying = false;
                    Debug.Log("Stopped");
                }
                Debug.Log(count.Key);
                return;
            }
        }
        if (!specialThemePlaying)
        {
            StartCoroutine(Fade(specialThemePlayer, 1));
            specialThemePlaying = true;
        }
    }

    private IEnumerator Fade(AudioSource source, float targetVolume)
    {
        float originalVolume = source.volume;
        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(originalVolume, targetVolume, i / fadeTime);
            Debug.Log(source.volume);
            yield return null;
        }
    }

    public void Play()
    {
        mainThemePlayer.Play();
        specialThemePlayer.Play();
        foreach (var player in players.Values)
        {
            player.Play();
        }
    }

    public void Pause()
    {
        mainThemePlayer.Pause();
        foreach (var player in players.Values)
        {
            player.Pause();
        }
    }

    public void Stop()
    {
        mainThemePlayer.Stop();
        foreach (var player in players.Values)
        {
            player.Stop();
        }
    }

    public void AddTheme(UnitType unitType)
    {
        unitCounts[unitType]++;
        float targetVolume = -Mathf.Pow(volumeScale, -unitCounts[unitType]) + 1;
        StartCoroutine(Fade(players[unitType], targetVolume));
    }

    public void RemoveTheme(UnitType unitType)
    {
        unitCounts[unitType]--;
        Debug.Assert(unitCounts[unitType] >= 0);
        float targetVolume = -Mathf.Pow(volumeScale, -unitCounts[unitType]) + 1;
        StartCoroutine(Fade(players[unitType], targetVolume));

    }
}

[System.Serializable]
public class UnitTheme
{
    public UnitType type;
    public AudioClip clip;
}