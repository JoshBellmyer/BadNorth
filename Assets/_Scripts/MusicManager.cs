using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [Range(1, 2)]
    [SerializeField] float volumeScale = 1.5f;
    [SerializeField] AudioClip mainTheme;
    [SerializeField] List<UnitTheme> themes;
    AudioSource mainThemePlayer;
    Dictionary<UnitType, AudioSource> players;
    Dictionary<UnitType, int> unitCounts;

    private void Start()
    {
        mainThemePlayer = gameObject.AddComponent<AudioSource>();
        mainThemePlayer.clip = mainTheme;
        mainThemePlayer.loop = true;

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

    public void Play()
    {
        mainThemePlayer.Play();
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
        players[unitType].volume = -Mathf.Pow(volumeScale, -unitCounts[unitType]) + 1;
        Debug.Log(players[unitType].volume);
    }

    public void RemoveTheme(UnitType unitType)
    {
        unitCounts[unitType]--;
        Debug.Assert(unitCounts[unitType] >= 0);
        players[unitType].volume = -Mathf.Pow(volumeScale, -unitCounts[unitType]) + 1;

    }
}

[System.Serializable]
public class UnitTheme
{
    public UnitType type;
    public AudioClip clip;
}