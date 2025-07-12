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
  [Header("��������� ����")]
  [SerializeField]
  private Valve[] _valves;
  [SerializeField]
  private SO_ValveSequence _soValvesSequence;
  [SerializeField]
  private string _menuSceneName = "MenuScene";

  [Header("������� ���������")]
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
          Debug.LogError("�� ������� ������� GameMessage!");
          yield break;
        }
      }
    }

    _gameMessage.gameObject.SetActive(true);
    _gameMessage.ShowMessage(
        "�������� �������� ������ �������, ����� ������� ��. ��������� � ��������");

    yield return new WaitForSeconds(_gameMessage.TotalAnimationDuration);
    GetComponent<ValveHint>()?.Begin();
  }

  private GameMessage CreateGameMessage() {
    Canvas canvas = FindObjectOfType<Canvas>();
    if (canvas == null) {
      GameObject canvasGO = new GameObject("Canvas ���������");
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

    // ��������� fallback �������
    if (_fontMessage != null) {
      // ������� ����� ������ fallback �������
      List<TMP_FontAsset> fallbacks = new List<TMP_FontAsset>();

      // ��������� ����������� �����
      TMP_FontAsset defaultFont =
          Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
      if (defaultFont != null) {
        fallbacks.Add(defaultFont);
      }

      // ��������� fallback ������
      _fontMessage.fallbackFontAssetTable = fallbacks;
    }

    gameMessage.SetupMessage(_fontMessage, _backgroundMessage);
    return gameMessage;
  }
  // private GameMessage CreateGameMessage() {
  //   Canvas canvas = FindObjectOfType<Canvas>();
  //   if (canvas == null) {
  //     GameObject canvasGO = new GameObject("Canvas ���������");
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

  //  //// ����� �������� � ������� ���� ��������� ���������
  //  // Debug.Log("����� �������� � �������:");
  //  // FindAllResources();

  //  // TMP_FontAsset font = FindResource<TMP_FontAsset>("Stengazeta-Regular_5");
  //  // Sprite background = FindResource<Sprite>("white-paper-texture-background");

  //  gameMessage.SetupMessage(_fontMessage, _backgroundMessage);  //, _backgroundMessageOKButton);
  //  return gameMessage;
  //}

  private void FindAllResources() {
#if UNITY_EDITOR
    string[] fontGUIDs = AssetDatabase.FindAssets("t:TMP_FontAsset");
    Debug.Log("������� TMP ������:");
    foreach (string guid in fontGUIDs) {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      Debug.Log(path);
    }

    string[] spriteGUIDs = AssetDatabase.FindAssets("t:Sprite");
    Debug.Log("������� �������:");
    foreach (string guid in spriteGUIDs) {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      Debug.Log(path);
    }

    string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture2D");
    Debug.Log("������� ��������:");
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
      Debug.Log($"������ ������: {path}");
      return AssetDatabase.LoadAssetAtPath<T>(path);
    }
#endif

    Debug.LogWarning($"������ {name} �� ������ ����� �������� ���� {typeof(T).Name}");
    return null;
  }

  // private void HandleValvePressed(Valve valve) {
  //   if (_gameMessage != null && _gameMessage.IsShowing) {
  //     // ������������� ��� �������� ��������
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
  //      Debug.LogError("�� ������� ������� GameMessage!");
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
  //    // ������������� ��� �������� ��������
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
  //    _gameMessage.ShowMessage("�������� ������");

  //    GetComponent<ValveHint>()?.Begin();
  //  }
  //}

  private void HandleValvePressed(Valve valve) {
    if (_gameMessage != null && _gameMessage.IsShowing) {
      // ���������� ��� ������� � ��������� ���������
      ResetAllValves();
      return;
    }

    if (_gameMessage == null) {
      _gameMessage = CreateGameMessage();
      if (_gameMessage == null) {
        Debug.LogError("�� ������� ������� GameMessage!");
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
      // ��������� ��� ������� ����� ������� ���������
      ResetAllValvesImmediately();

      _currentStep = 0;
      CurrentLevel = 1;
      OnSequenceFailed?.Invoke();

      _gameMessage.gameObject.SetActive(true);
      _gameMessage.ShowMessage("�������� ������. �������� ������.");

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
        // �������������� ��������
        v.GetComponent<Image>().color = Color.green;
      }
    }
  }

  private IEnumerator CompleteGameRoutine() {
    _gameMessage.gameObject.SetActive(true);
    _gameMessage.ShowMessage(
        "�������� ������! ������ ����� ������� ��� � ������ ����������� ���������!");

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
