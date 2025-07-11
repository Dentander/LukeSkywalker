using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Luke : MonoBehaviour {
  [SerializeField]
  private Valve[] _valves;
  [SerializeField]
  private SO_ValveSequence _soValvesSequence;

  public event Action OnSequenceCompleted;
  public event Action<int> OnLevelIncreased;
  public event Action OnSequenceFailed;

  public int CurrentLevel { get; private set; } = 1;
  public int TotalLevels => _soValvesSequence.ValveSequence.Length;
  private int _currentStep;

  public Valve GetValveByType(ValveType type) {
    foreach (Valve valve in _valves) {
      if (valve.Type == type)
        return valve;
    }
    return null;
  }

  private void Start() {
    GetComponent<ValveHint>()?.Begin();
  }

  [SerializeField]
  private string _menuSceneName = "MenuScene";

  private void HandleValvePressed(Valve valve) {
    if (valve.Type == _soValvesSequence.ValveSequence[_currentStep]) {
      _currentStep++;

      if (_currentStep == CurrentLevel) {
        if (CurrentLevel == TotalLevels) {
          OnSequenceCompleted?.Invoke();
          LoadMenuScene();
        } else {
          CurrentLevel++;
          _currentStep = 0;
          OnLevelIncreased?.Invoke(CurrentLevel);
          GetComponent<ValveHint>()?.Begin();
        }
      }
    } else {
      _currentStep = 0;
      CurrentLevel = 1;
      OnSequenceFailed?.Invoke();
      GetComponent<ValveHint>()?.Begin();
    }
  }
  private void LoadMenuScene() {
    if (!string.IsNullOrEmpty(_menuSceneName)) {
      Debug.Log($"Loading menu scene: {_menuSceneName}");
      SceneManager.LoadScene(_menuSceneName);
    } else {
      Debug.LogWarning("Menu scene name is not set!");
    }
  }

  private void OnEnable() {
    foreach (Valve valve in _valves) {
      valve.OnValvePressed += HandleValvePressed;
    }
  }

  private void OnDisable() {
    foreach (Valve valve in _valves) {
      valve.OnValvePressed -= HandleValvePressed;
    }
  }
}
