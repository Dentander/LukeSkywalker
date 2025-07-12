using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class GameMessage : MonoBehaviour {
  [Header("Настройки дизайна")]
  [SerializeField]
  private Color _textColor = new Color(0.863f, 0.475f, 0.224f);  // dd7939
  [SerializeField]
  private Color _buttonColor = new Color(0.2f, 0.7f, 0.2f);
  [SerializeField]
  private Sprite _buttonOKBackground;
  [SerializeField]
  private int _fontSize = 150;
  [SerializeField]
  private float _fadeInDuration = 0.5f;
  [SerializeField]
  private float _textPadding = 40f;  // Увеличенные отступы

  [SerializeField]
  GameObject messagePrefab;

  private TextMeshProUGUI _messageText;
  private CanvasGroup _canvasGroup;
  private Image _background;
  private GameObject _okButton;
  private Button _buttonComponent;

  public float TotalAnimationDuration => _fadeInDuration;
  public bool IsShowing { get; private set; }

  private void Awake() {
    _messageText = GetComponent<TextMeshProUGUI>();
    _canvasGroup = GetComponent<CanvasGroup>();
    _background = GetComponent<Image>();

    _canvasGroup.alpha = 0f;
    gameObject.SetActive(false);
  }

  public void SetupMessage(TMP_FontAsset customFont, Sprite backgroundSprite) {
    // Настройка фона
    _background.sprite = backgroundSprite;
    _background.type = Image.Type.Sliced;
    _background.color = Color.white;
    _background.raycastTarget = true;

    // Настройка текста
    _messageText.color = _textColor;
    _messageText.fontSize = _fontSize;
    _messageText.alignment = TextAlignmentOptions.Center;
    _messageText.fontStyle = FontStyles.Bold;
    _messageText.enableWordWrapping = true;
    _messageText.overflowMode = TextOverflowModes.Ellipsis;
    _messageText.margin = new Vector4(_textPadding, _textPadding, _textPadding, _textPadding);

    if (customFont != null) {
      _messageText.font = customFont;

      // Добавляем fallback для специальных символов
      if (_messageText.font.fallbackFontAssetTable == null ||
          _messageText.font.fallbackFontAssetTable.Count == 0) {
        TMP_FontAsset fallbackFont =
            Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (fallbackFont != null) {
          _messageText.font.fallbackFontAssetTable = new List<TMP_FontAsset> { fallbackFont };
        }
      }

      // Принудительно обновляем символы
      _messageText.ForceMeshUpdate();
    }

    // Уменьшенные размеры окна (было 0.1-0.9 по X и 0.2-0.8 по Y)
    RectTransform rt = GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0.15f, 0.25f);  // Уменьшил ширину и высоту
    rt.anchorMax = new Vector2(0.85f, 0.75f);
    rt.offsetMin = Vector2.zero;
    rt.offsetMax = Vector2.zero;

    CreateOkButton();
  }

  private void CreateOkButton() {
    if (_okButton != null)
      return;

    _okButton = new GameObject("OK Button");
    _okButton.transform.SetParent(transform, false);

    RectTransform buttonRt = _okButton.AddComponent<RectTransform>();
    buttonRt.anchorMin = new Vector2(0.4f, 0.1f);
    buttonRt.anchorMax = new Vector2(0.6f, 0.2f);
    buttonRt.offsetMin = Vector2.zero;
    buttonRt.offsetMax = Vector2.zero;

    Image buttonImage = _okButton.AddComponent<Image>();
    buttonImage.color = _buttonColor;

    Texture2D roundedTexture = CreateRoundedTexture(200, 100, 20, _buttonColor);
    buttonImage.sprite =
        Sprite.Create(roundedTexture, new Rect(0, 0, 200, 100), Vector2.one * 0.5f);

    _buttonComponent = _okButton.AddComponent<Button>();
    _buttonComponent.onClick.AddListener(() => HideMessage());

    GameObject textObj = new GameObject("Text");
    textObj.transform.SetParent(_okButton.transform, false);

    TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
    buttonText.text = "OK";
    buttonText.color = Color.white;
    buttonText.fontSize = 42;
    buttonText.alignment = TextAlignmentOptions.Center;
    buttonText.fontStyle = FontStyles.Bold;

    RectTransform textRt = textObj.GetComponent<RectTransform>();
    textRt.anchorMin = Vector2.zero;
    textRt.anchorMax = Vector2.one;
    textRt.offsetMin = Vector2.zero;
    textRt.offsetMax = Vector2.zero;
  }

  private Texture2D CreateRoundedTexture(int width, int height, int radius, Color color) {
    Texture2D texture = new Texture2D(width, height);
    Color transparent = new Color(0, 0, 0, 0);

    for (int y = 0; y < height; y++) {
      for (int x = 0; x < width; x++) {
        bool isCorner = (x < radius && y < radius) || (x < radius && y > height - radius - 1) ||
                        (x > width - radius - 1 && y < radius) ||
                        (x > width - radius - 1 && y > height - radius - 1);

        if (isCorner) {
          int dx = Mathf.Min(x, width - x - 1);
          int dy = Mathf.Min(y, height - y - 1);
          if (Mathf.Sqrt(dx * dx + dy * dy) > radius)
            texture.SetPixel(x, y, transparent);
          else
            texture.SetPixel(x, y, color);
        } else {
          texture.SetPixel(x, y, color);
        }
      }
    }

    texture.Apply();
    return texture;
  }

  public void ShowMessage(string message) {
    if (_messageText == null)
      return;

    _messageText.text = message;

    // Принудительное обновление текста
    _messageText.ForceMeshUpdate();

    gameObject.SetActive(true);
    IsShowing = true;
    StartCoroutine(FadeIn());
  }

  private IEnumerator FadeIn() {
    _canvasGroup.alpha = 0f;
    Time.timeScale = 0f;

    float timer = 0f;
    while (timer < _fadeInDuration) {
      _canvasGroup.alpha = timer / _fadeInDuration;
      timer += Time.unscaledDeltaTime;
      yield return null;
    }

    _canvasGroup.alpha = 1f;
  }

  public void HideMessage() {
    Time.timeScale = 1f;
    IsShowing = false;
    gameObject.SetActive(false);
  }
}
