using UnityEngine;

public class AudioManager : MonoBehaviour {
  public static AudioManager Instance;

  [SerializeField]
  private AudioClip backgroundMusic;
  private AudioSource musicSource;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(gameObject);

      // Создаем источник для музыки
      musicSource = gameObject.AddComponent<AudioSource>();
      musicSource.clip = backgroundMusic;
      musicSource.loop = true;
      musicSource.playOnAwake = true;
      musicSource.volume = 0.5f;  // Настройте громкость по желанию

      PlayBackgroundMusic();
    } else {
      Destroy(gameObject);
    }
  }

  public void PlayBackgroundMusic() {
    if (!musicSource.isPlaying) {
      musicSource.Play();
    }
  }

  public void StopBackgroundMusic() {
    musicSource.Stop();
  }

  public void SetMusicVolume(float volume) {
    musicSource.volume = volume;
  }

  // Метод для воспроизведения звуковых эффектов
  public void PlaySound(AudioClip clip, float volume = 1f) {
    AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
  }
}
