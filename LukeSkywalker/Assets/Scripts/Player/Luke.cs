using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Luke : MonoBehaviour {
  [Header("Game Settings")]
  [SerializeField]
  private Valve[] _valves;
  [SerializeField]
  private SO_ValveSequence _soValvesSequence;
  [SerializeField]
  private string _menuSceneName = "MenuScene";

  [Header("Message System")]
  [SerializeField]
  private GameMessage _gameMessage;

  public event Action OnSequenceCompleted;
  public event Action<int> OnLevelIncreased;
  public event Action OnSequenceFailed;

  public int CurrentLevel { get; private set; } = 1;
  public int TotalLevels => _soValvesSequence.ValveSequence.Length;
  private int _currentStep;

  private void Start() {
    StartCoroutine(StartGameRoutine());
  }

  // private IEnumerator StartGameRoutine() {
  //   // Показываем стартовое сообщение
  //   //_gameMessage.ShowMessage("Повторяй!");
  //   if (_gameMessage != null && _gameMessage.gameObject.activeInHierarchy) {
  //     _gameMessage.ShowMessage("Повторяй");
  //   } else {
  //     Debug.LogWarning("GameMessage не доступен!");
  //   }

  //  // Ждем завершения анимации перед началом подсказок
  //  yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration * 0.5f);

  //  GetComponent<ValveHint>()?.Begin();
  //}
  // private IEnumerator StartGameRoutine() {
  //  // Ждем завершения инициализации
  //  yield return null;

  //  // Получаем или создаем GameMessage
  //  if (_gameMessage == null) {
  //    _gameMessage = FindObjectOfType<GameMessage>(true);

  //    if (_gameMessage == null) {
  //      // Создаем новый экземпляр через Instantiate
  //      GameObject messagePrefab = Resources.Load<GameObject>("Prefabs/GameMessage");
  //      if (messagePrefab != null) {
  //        _gameMessage = Instantiate(messagePrefab).GetComponent<GameMessage>();
  //        _gameMessage.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
  //      } else {
  //        Debug.LogError("GameMessage prefab not found in Resources!");
  //        yield break;
  //      }
  //    }
  //  }

  //  // Активируем и показываем сообщение
  //  _gameMessage.gameObject.SetActive(true);
  //  _gameMessage.ShowMessage("Повторяй!");

  //  yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration * 0.5f);
  //  GetComponent<ValveHint>()?.Begin();
  //}

  // private IEnumerator StartGameRoutine() {
  //   yield return null;

  //  if (_gameMessage == null) {
  //    _gameMessage = FindObjectOfType<GameMessage>(true);
  //    if (_gameMessage == null) {
  //      CreateDefaultGameMessage();
  //    }
  //  }

  //  // Не пытаемся менять parent для существующего объекта
  //  _gameMessage.gameObject.SetActive(true);
  //  _gameMessage.ShowMessage("Повторяй!");

  //  yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration * 0.5f);
  //  GetComponent<ValveHint>()?.Begin();
  //}

  private void CreateDefaultGameMessage() {
    // Создаем новый объект (не из префаба)
    GameObject messageGO = new GameObject("DynamicGameMessage");
    messageGO.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

    // Настройка RectTransform
    RectTransform rt = messageGO.AddComponent<RectTransform>();
    rt.anchoredPosition = Vector2.zero;
    rt.sizeDelta = new Vector2(400, 100);

    // Добавляем компоненты
    _gameMessage = messageGO.AddComponent<GameMessage>();
    _gameMessage.SetComponents(messageGO.AddComponent<TextMeshProUGUI>(),
                               messageGO.AddComponent<CanvasGroup>());

    // Настройка текста
    TextMeshProUGUI text = _gameMessage.GetComponent<TextMeshProUGUI>();
    text.text = "Повторяй!";
    text.fontSize = 72;
    text.alignment = TextAlignmentOptions.Center;
    text.color = Color.white;
  }

  private IEnumerator StartGameRoutine() {
    yield return null;  // Ждем завершения инициализации

    if (_gameMessage == null) {
      _gameMessage = FindObjectOfType<GameMessage>();
      if (_gameMessage == null) {
        _gameMessage = CreateGameMessage();
        if (_gameMessage == null) {
          Debug.LogError("Failed to create GameMessage!");
          yield break;
        }
      }
    }

    _gameMessage.ShowMessage("Повторяй!");
    yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration * 0.5f);
    GetComponent<ValveHint>()?.Begin();
  }

  private GameMessage CreateGameMessage() {
    // Создаем новый объект
    GameObject messageGO = new GameObject("GameMessage");

    // Устанавливаем высокий порядок отрисовки
    Canvas canvas = FindObjectOfType<Canvas>();
    if (canvas == null) {
      GameObject canvasGO = new GameObject("MessageCanvas");
      canvas = canvasGO.AddComponent<Canvas>();
      canvasGO.AddComponent<CanvasScaler>();
      canvasGO.AddComponent<GraphicRaycaster>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      canvas.sortingOrder = 999;  // Максимальный приоритет
    }

    // Добавляем обязательные компоненты
    var text = messageGO.AddComponent<TextMeshProUGUI>();
    var canvasGroup = messageGO.AddComponent<CanvasGroup>();

    // Настраиваем RectTransform
    var rt = messageGO.GetComponent<RectTransform>();
    rt.SetParent(FindObjectOfType<Canvas>().transform, false);
    rt.anchoredPosition = Vector2.zero;
    rt.sizeDelta = new Vector2(500, 200);

    // Настраиваем текст
    text.text = "Повторяй!";
    text.fontSize = 72;
    text.alignment = TextAlignmentOptions.Center;
    text.color = Color.white;
    text.fontStyle = FontStyles.Bold;

    // Добавляем и возвращаем GameMessage
    return messageGO.AddComponent<GameMessage>();
  }

  private void HandleValvePressed(Valve valve) {
    if (_gameMessage == null) {
      // CreateGameMessage();
      CreateDefaultGameMessage();
      Debug.LogWarning("GameMessage был создан в runtime");
    }

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
      _currentStep = 0;
      CurrentLevel = 1;
      OnSequenceFailed?.Invoke();
      //_gameMessage.ShowMessage("Допущена ошибка");
      if (_gameMessage != null && _gameMessage.gameObject.activeInHierarchy) {
        _gameMessage.ShowMessage("Допущена ошибка");
      } else {
        Debug.LogWarning("GameMessage не доступен!");
      }
      GetComponent<ValveHint>()?.Begin();
    }
  }

  private IEnumerator CompleteGameRoutine() {
    //_gameMessage.ShowMessage("У тебя получилось!");
    if (_gameMessage != null && _gameMessage.gameObject.activeInHierarchy) {
      _gameMessage.ShowMessage("У тебя получилось!");
    } else {
      Debug.LogWarning("GameMessage не доступен!");
    }
    yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration);
    SceneManager.LoadScene(_menuSceneName);
  }

  public Valve GetValveByType(ValveType type) {
    foreach (Valve valve in _valves) {
      if (valve.Type == type)
        return valve;
    }
    return null;
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
