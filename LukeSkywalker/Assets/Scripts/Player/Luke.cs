#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

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
  private GameMessage _gameMessage;
  [SerializeField]
  private GameMessage _gameMessagePrefab;
  private GameMessage _gameMessageInstance;
  [SerializeField]
  private TMP_FontAsset _fontMessage;
  [SerializeField]
  private Sprite _backgroundMessage;
  [SerializeField]
  private Sprite _backgroundMessageOKButton;

  public event Action OnSequenceCompleted;
  public event Action<int> OnLevelIncreased;
  public event Action OnSequenceFailed;

  public int CurrentLevel { get; private set; } = 1;
  public int TotalLevels => _soValvesSequence.ValveSequence.Length;
  private int _currentStep;

  private void Start() {
    StartCoroutine(StartGameRoutine());
  }

  private IEnumerator StartGameRoutine() {
    yield return null;

    if (_gameMessage == null) {
      _gameMessage = FindObjectOfType<GameMessage>(true);
      if (_gameMessage == null) {
        _gameMessage = CreateGameMessage();
        if (_gameMessage == null) {
          Debug.LogError("Не удалось создать GameMessage!");
          yield break;
        }
      }
    }

    _gameMessage.gameObject.SetActive(true);
    _gameMessage.ShowMessage(
        "Выровняй давление внутри капсулы, чтобы открыть ее. Запоминай и повторяй");

    yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration);
    GetComponent<ValveHint>()?.Begin();
  }

  private GameMessage CreateGameMessage() {
    Canvas canvas = FindObjectOfType<Canvas>();
    if (canvas == null) {
      GameObject canvasGO = new GameObject("Canvas сообщений");
      canvas = canvasGO.AddComponent<Canvas>();
      canvasGO.AddComponent<CanvasScaler>();
      canvasGO.AddComponent<GraphicRaycaster>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      canvas.sortingOrder = 999;
    }

    GameObject messageGO = new GameObject("GameMessage");
    messageGO.transform.SetParent(canvas.transform, false);

    GameMessage gameMessage = messageGO.AddComponent<GameMessage>();
    RectTransform rt = messageGO.GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0.2f, 0.3f);
    rt.anchorMax = new Vector2(0.8f, 0.7f);
    rt.offsetMin = Vector2.zero;
    rt.offsetMax = Vector2.zero;

    // Настройка fallback шрифтов
    if (_fontMessage != null) {
      // Создаем новый список fallback шрифтов
      List<TMP_FontAsset> fallbacks = new List<TMP_FontAsset>();

      // Добавляем стандартный шрифт
      TMP_FontAsset defaultFont =
          Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
      if (defaultFont != null) {
        fallbacks.Add(defaultFont);
      }

      // Применяем fallback шрифты
      _fontMessage.fallbackFontAssetTable = fallbacks;
    }

    gameMessage.SetupMessage(_fontMessage, _backgroundMessage);
    return gameMessage;
  }
  // private GameMessage CreateGameMessage() {
  //   Canvas canvas = FindObjectOfType<Canvas>();
  //   if (canvas == null) {
  //     GameObject canvasGO = new GameObject("Canvas сообщений");
  //     canvas = canvasGO.AddComponent<Canvas>();
  //     canvasGO.AddComponent<CanvasScaler>();
  //     canvasGO.AddComponent<GraphicRaycaster>();
  //     canvas.renderMode = RenderMode.ScreenSpaceOverlay;
  //     canvas.sortingOrder = 999;
  //   }

  //  GameObject messageGO = new GameObject("GameMessage");
  //  messageGO.transform.SetParent(canvas.transform, false);

  //  GameMessage gameMessage = messageGO.AddComponent<GameMessage>();
  //  RectTransform rt = messageGO.GetComponent<RectTransform>();
  //  rt.anchoredPosition = Vector2.zero;
  //  rt.sizeDelta = new Vector2(800, 500);

  //  //// Поиск ресурсов с выводом всех возможных вариантов
  //  // Debug.Log("Поиск ресурсов в проекте:");
  //  // FindAllResources();

  //  // TMP_FontAsset font = FindResource<TMP_FontAsset>("Stengazeta-Regular_5");
  //  // Sprite background = FindResource<Sprite>("white-paper-texture-background");

  //  gameMessage.SetupMessage(_fontMessage, _backgroundMessage);  //, _backgroundMessageOKButton);
  //  return gameMessage;
  //}

  private void FindAllResources() {
#if UNITY_EDITOR
    string[] fontGUIDs = AssetDatabase.FindAssets("t:TMP_FontAsset");
    Debug.Log("Найдены TMP шрифты:");
    foreach (string guid in fontGUIDs) {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      Debug.Log(path);
    }

    string[] spriteGUIDs = AssetDatabase.FindAssets("t:Sprite");
    Debug.Log("Найдены спрайты:");
    foreach (string guid in spriteGUIDs) {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      Debug.Log(path);
    }

    string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture2D");
    Debug.Log("Найдены текстуры:");
    foreach (string guid in textureGUIDs) {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      Debug.Log(path);
    }
#endif
  }

  private T FindResource<T>(string name)
      where T : UnityEngine.Object {
#if UNITY_EDITOR
    string[] guids = AssetDatabase.FindAssets(name + " t:" + typeof(T).Name);
    if (guids.Length > 0) {
      string path = AssetDatabase.GUIDToAssetPath(guids[0]);
      Debug.Log($"Найден ресурс: {path}");
      return AssetDatabase.LoadAssetAtPath<T>(path);
    }
#endif

    Debug.LogWarning($"Ресурс {name} не найден среди объектов типа {typeof(T).Name}");
    return null;
  }

  // private void HandleValvePressed(Valve valve) {
  //   if (_gameMessage != null && _gameMessage.IsShowing) {
  //     // Останавливаем все анимации клапанов
  //     // foreach (Valve v in _valves) {
  //     //  var effect = v.GetComponent<ValvePressedEffect>();
  //     //  if (effect != null) {
  //     //    effect.StopAllCoroutines();
  //     //    v.GetComponent<Image>().color = Color.green;
  //     //  }
  //     //}
  //     foreach (Valve v in _valves) {
  //       var effect = v.GetComponent<ValvePressedEffect>();
  //       if (effect != null) {
  //         effect.InterruptEffect();
  //       }
  //     }

  //    return;
  //  }

  //  if (_gameMessage == null) {
  //    _gameMessage = CreateGameMessage();
  //    if (_gameMessage == null) {
  //      Debug.LogError("Не удалось создать GameMessage!");
  //      return;
  //    }
  //  }

  //  if (valve.Type == _soValvesSequence.ValveSequence[_currentStep]) {
  //    _currentStep++;

  //    if (_currentStep == CurrentLevel) {
  //      if (CurrentLevel == TotalLevels) {
  //        OnSequenceCompleted?.Invoke();
  //        StartCoroutine(CompleteGameRoutine());
  //      } else {
  //        CurrentLevel++;
  //        _currentStep = 0;
  //        OnLevelIncreased?.Invoke(CurrentLevel);
  //        GetComponent<ValveHint>()?.Begin();
  //      }
  //    }
  //  } else {
  //    // Останавливаем все анимации клапанов
  //    // foreach (Valve v in _valves) {
  //    //  var effect = v.GetComponent<ValvePressedEffect>();
  //    //  if (effect != null) {
  //    //    effect.StopAllCoroutines();
  //    //    v.GetComponent<Image>().color = Color.green;
  //    //  }
  //    //}
  //    foreach (Valve v in _valves) {
  //      var effect = v.GetComponent<ValvePressedEffect>();
  //      if (effect != null) {
  //        effect.InterruptEffect();
  //      }
  //    }

  //    _currentStep = 0;
  //    CurrentLevel = 1;
  //    OnSequenceFailed?.Invoke();

  //    _gameMessage.gameObject.SetActive(true);
  //    _gameMessage.ShowMessage("Допущена ошибка");

  //    GetComponent<ValveHint>()?.Begin();
  //  }
  //}

  private void HandleValvePressed(Valve valve) {
    if (_gameMessage != null && _gameMessage.IsShowing) {
      // Сбрасываем все клапаны в дефолтное состояние
      ResetAllValves();
      return;
    }

    if (_gameMessage == null) {
      _gameMessage = CreateGameMessage();
      if (_gameMessage == null) {
        Debug.LogError("Не удалось создать GameMessage!");
        return;
      }
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
      // Прерываем ВСЕ эффекты перед показом сообщения
      ResetAllValvesImmediately();

      _currentStep = 0;
      CurrentLevel = 1;
      OnSequenceFailed?.Invoke();

      _gameMessage.gameObject.SetActive(true);
      _gameMessage.ShowMessage("Попробуй заново. Неверный клапан.");

      GetComponent<ValveHint>()?.Begin();
    }
  }

  private void ResetAllValves() {
    foreach (Valve v in _valves) {
      var effect = v.GetComponent<ValvePressedEffect>();
      if (effect != null) {
        effect.InterruptEffect();
      }
      v.GetComponent<Image>().color = Color.green;
    }
  }

  private void ResetAllValvesImmediately() {
    foreach (Valve v in _valves) {
      var effect = v.GetComponent<ValvePressedEffect>();
      if (effect != null) {
        effect.InterruptEffect();
        // Дополнительная гарантия
        v.GetComponent<Image>().color = Color.green;
      }
    }
  }

  private IEnumerator CompleteGameRoutine() {
    _gameMessage.gameObject.SetActive(true);
    _gameMessage.ShowMessage(
        "Отличная работа! Теперь можно открыть люк и помочь космонавтам выбраться!");

    while (_gameMessage.gameObject.activeSelf) {
      yield return null;
    }

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
