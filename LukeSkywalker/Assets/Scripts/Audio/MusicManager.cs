using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MusicTrack {
  public string trackName;
  public AudioClip audioClip;
  [Range(0, 1)]
  public float volume = 1f;
}

public class MusicManager : MonoBehaviour {
  public static MusicManager Shared { get; private set; }

  [Header("Music Settings")]
  [SerializeField]
  private MusicTrack[] trackLibrary;
  [SerializeField]
  private int maximumLayers = 3;
  [SerializeField]
  private float globalVolume = 1f;

  private readonly List<AudioSource> activeAudioSources = new List<AudioSource>();
  private string currentMainTrackId;

  private void Awake() {
    if (Shared == null) {
      Shared = this;
      DontDestroyOnLoad(gameObject);
    } else {
      Destroy(gameObject);
    }
  }

  public void PlayMainTrack(string trackId) {
    MusicTrack track = FindTrack(trackId);
    if (track == null)
      return;

    StopMainTrack();

    AudioSource newSource = CreateAudioSource(track);
    newSource.loop = true;
    newSource.Play();

    activeAudioSources.Add(newSource);
    currentMainTrackId = trackId;
  }

  public void PlayOverlayTrack(string trackId, bool shouldLoop = false) {
    if (activeAudioSources.Count >= maximumLayers) {
      Debug.LogWarning("Maximum audio layers reached!");
      return;
    }

    MusicTrack track = FindTrack(trackId);
    if (track == null)
      return;

    AudioSource newSource = CreateAudioSource(track);
    newSource.loop = shouldLoop;
    newSource.Play();

    activeAudioSources.Add(newSource);

    if (!shouldLoop) {
      StartCoroutine(RemoveSourceWhenFinished(newSource));
    }
  }

  public void StopMainTrack() {
    if (string.IsNullOrEmpty(currentMainTrackId))
      return;

    AudioSource sourceToRemove =
        activeAudioSources.FirstOrDefault(s => s.clip.name == currentMainTrackId);

    if (sourceToRemove != null) {
      RemoveAudioSource(sourceToRemove);
    }
    currentMainTrackId = null;
  }

  public void StopOverlayTrack(string trackId) {
    List<AudioSource> sourcesToRemove =
        activeAudioSources.Where(s => s.clip.name == trackId && s.clip.name != currentMainTrackId)
            .ToList();

    foreach (AudioSource source in sourcesToRemove) {
      RemoveAudioSource(source);
    }
  }

  public void StopAllTracks() {
    foreach (AudioSource source in activeAudioSources.ToArray()) {
      RemoveAudioSource(source);
    }
    currentMainTrackId = null;
  }

  public void SetGlobalVolume(float volume) {
    globalVolume = Mathf.Clamp01(volume);
    foreach (AudioSource source in activeAudioSources) {
      source.volume = GetTrackVolume(source.clip.name) * globalVolume;
    }
  }

  private MusicTrack FindTrack(string trackId) {
    MusicTrack track = trackLibrary.FirstOrDefault(t => t.trackName == trackId);
    if (track == null) {
      Debug.LogWarning($"Track not found: {trackId}");
    }
    return track;
  }

  private AudioSource CreateAudioSource(MusicTrack track) {
    AudioSource newSource = gameObject.AddComponent<AudioSource>();
    newSource.clip = track.audioClip;
    newSource.volume = track.volume * globalVolume;
    return newSource;
  }

  private void RemoveAudioSource(AudioSource source) {
    if (source == null)
      return;

    source.Stop();
    activeAudioSources.Remove(source);
    Destroy(source);
  }

  private float GetTrackVolume(string trackId) {
    MusicTrack track = FindTrack(trackId);
    return track?.volume ?? 1f;
  }

  private System.Collections.IEnumerator RemoveSourceWhenFinished(AudioSource source) {
    yield return new WaitForSeconds(source.clip.length);
    RemoveAudioSource(source);
  }
}
