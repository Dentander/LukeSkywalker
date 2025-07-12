using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Luke : MonoBehaviour {
  [Header("Настройки игры")]
  [SerializeField]
  private Valve[] _valves;
  [SerializeField]
  private SO_ValveSequence _soValvesSequence;
  [SerializeField]
  private string _menuSceneName = "MenuScene";

  [Header("Система сообщений")]
  [SerializeField]
  private GameMessage _gameMessagePrefab;
  [SerializeField]
  private TMP_FontAsset _fontMessage;
  [SerializeField]
  private Sprite _backgroundMessage;

  public event Action OnSequenceCompleted;
  public event Action<int> OnLevelIncreased;
  public event Action OnSequenceFailed;

  public int CurrentLevel { get; private set; } = 1;
  public int TotalLevels => _soValvesSequence.ValveSequence.Length;

  private GameMessage _gameMessage;
  private int _currentStep;

  private void Start() {
    MusicManager.Shared.PlayMainTrack("BackgroundMusic");
    StartCoroutine(GameStartRoutine());
  }

  private IEnumerator GameStartRoutine() {
    yield return null;

    EnsureGameMessageExists();
    _gameMessage.gameObject.SetActive(true);
    _gameMessage.ShowMessage(
        "Выровняй давление внутри капсулы, чтобы открыть её. Запоминай и повторяй");

    yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration);
    GetComponent<ValveHint>()?.Begin();
  }

  private void EnsureGameMessageExists() {
    if (_gameMessage != null)
      return;

    _gameMessage = FindObjectOfType<GameMessage>(true) ??
                   Instantiate(_gameMessagePrefab, EnsureCanvas().transform, false);

    if (_fontMessage != null && _backgroundMessage != null) {
      Debug.LogWarning("Everything is OK.");
      _gameMessage.SetupMessage(_fontMessage, _backgroundMessage);
    } else if (_fontMessage == null) {
      Debug.LogWarning("Font is missing, skipping SetupMessage.");
    } else if (_backgroundMessage == null) {
      Debug.LogWarning("Background sprite is missing, skipping SetupMessage.");
    }
  }

  private Canvas EnsureCanvas() {
    Canvas canvas = FindObjectOfType<Canvas>();
    if (canvas != null)
      return canvas;

    GameObject canvasGO = new GameObject("Canvas сообщений");
    canvas = canvasGO.AddComponent<Canvas>();
    canvasGO.AddComponent<CanvasScaler>();
    canvasGO.AddComponent<GraphicRaycaster>();
    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    canvas.sortingOrder = 999;
    return canvas;
  }

  private void HandleValvePressed(Valve valve) {
    if (_gameMessage != null && _gameMessage.IsShowing) {
      ResetAllValves();
      return;
    }

    EnsureGameMessageExists();

    if (valve.Type == _soValvesSequence.ValveSequence[_currentStep]) {
      _currentStep++;

      if (_currentStep == CurrentLevel) {
        if (CurrentLevel == TotalLevels) {
          OnSequenceCompleted?.Invoke();
          StartCoroutine(CompleteGameRoutine());
        } else {
          CurrentLevel++;
          _currentStep = 0;
          OnLevelIncreased?.Invoke(CurrentLevel);
          GetComponent<ValveHint>()?.Begin();
        }
      }
    } else {
      FailSequence();
    }
  }

  private void FailSequence() {
    ResetAllValvesImmediately();
    MusicManager.Shared.PlayOverlayTrack("Again", true);
    _currentStep = 0;
    CurrentLevel = 1;
    OnSequenceFailed?.Invoke();

    _gameMessage.gameObject.SetActive(true);
    _gameMessage.ShowMessage("Попробуй заново. Неверный клапан.");
    MusicManager.Shared.StopOverlayTrack("Again");
    GetComponent<ValveHint>()?.Begin();
  }

  private void ResetAllValves() {
    foreach (var valve in _valves) {
      valve.GetComponent<ValvePressedEffect>()?.InterruptEffect();
      valve.GetComponent<Image>().color = Color.green;
    }
  }

  private void ResetAllValvesImmediately() {
    foreach (var valve in _valves) {
      valve.GetComponent<ValvePressedEffect>()?.InterruptEffect();
      valve.GetComponent<Image>().color = Color.green;
    }
  }

  private IEnumerator CompleteGameRoutine() {
    _gameMessage.gameObject.SetActive(true);
    _gameMessage.ShowMessage(
        "Отличная работа! Теперь можно открыть люк и помочь космонавтам выбраться!");
    while (_gameMessage.gameObject.activeSelf) yield return null;
    SceneManager.LoadScene(_menuSceneName);
  }

  public Valve GetValveByType(ValveType type) {
    foreach (var valve in _valves) {
      if (valve.Type == type)
        return valve;
    }
    return null;
  }

  private void OnEnable() {
    foreach (var valve in _valves) {
      valve.OnValvePressed += HandleValvePressed;
    }
  }

  private void OnDisable() {
    foreach (var valve in _valves) {
      valve.OnValvePressed -= HandleValvePressed;
    }
  }
}
