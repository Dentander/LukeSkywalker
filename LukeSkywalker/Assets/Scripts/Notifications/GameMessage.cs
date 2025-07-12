using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
  private int _fontSize = 350;
  [SerializeField]
  private float _fadeInDuration = 0.5f;

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

    if (customFont != null) {
      _messageText.font = customFont;
    }

    // Настройка RectTransform
    RectTransform rt = GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0.1f, 0.2f);
    rt.anchorMax = new Vector2(0.9f, 0.8f);
    rt.offsetMin = Vector2.zero;
    rt.offsetMax = Vector2.zero;

    CreateOkButton();
  }

  private void CreateOkButton() {
    if (_okButton != null)
      return;

    // Создаем кнопку
    _okButton = new GameObject("OK Button");
    _okButton.transform.SetParent(transform, false);

    // Настройка RectTransform (приподнята + уже)
    RectTransform buttonRt = _okButton.AddComponent<RectTransform>();
    buttonRt.anchorMin = new Vector2(0.4f, 0.1f);  // Выше чем было (0.05 -> 0.1)
    buttonRt.anchorMax = new Vector2(0.6f, 0.2f);  // Уже (40% -> 20% ширины)
    buttonRt.offsetMin = Vector2.zero;
    buttonRt.offsetMax = Vector2.zero;

    // Создаем спрайт с закругленными углами
    Image buttonImage = _okButton.AddComponent<Image>();
    buttonImage.color = _buttonColor;

    // Простейший способ скругления - через текстуру
    Texture2D roundedTexture = CreateRoundedTexture(200, 100, 20, _buttonColor);
    buttonImage.sprite =
        Sprite.Create(roundedTexture, new Rect(0, 0, 200, 100), Vector2.one * 0.5f);

    // Настройка кнопки (исправленный вариант)
    _buttonComponent = _okButton.AddComponent<Button>();
    _buttonComponent.onClick.AddListener(() => HideMessage());  // Фикс: явный вызов

    // Текст кнопки
    GameObject textObj = new GameObject("Text");
    textObj.transform.SetParent(_okButton.transform, false);

    TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
    buttonText.text = "OK";
    buttonText.color = Color.white;
    buttonText.fontSize = 42;  // Оптимальный размер
    buttonText.alignment = TextAlignmentOptions.Center;
    buttonText.fontStyle = FontStyles.Bold;

    // Растягиваем текст
    RectTransform textRt = textObj.GetComponent<RectTransform>();
    textRt.anchorMin = Vector2.zero;
    textRt.anchorMax = Vector2.one;
    textRt.offsetMin = Vector2.zero;
    textRt.offsetMax = Vector2.zero;
  }

  // Создает текстуру с закругленными углами
  private Texture2D CreateRoundedTexture(int width, int height, int radius, Color color) {
    Texture2D texture = new Texture2D(width, height);
    Color transparent = new Color(0, 0, 0, 0);

    for (int y = 0; y < height; y++) {
      for (int x = 0; x < width; x++) {
        bool isCorner = (x < radius && y < radius) ||                         // Левый нижний
                        (x < radius && y > height - radius - 1) ||            // Левый верхний
                        (x > width - radius - 1 && y < radius) ||             // Правый нижний
                        (x > width - radius - 1 && y > height - radius - 1);  // Правый верхний

        if (isCorner) {
          // Проверяем попадание в окружность угла
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
    gameObject.SetActive(true);
    IsShowing = true;
    StartCoroutine(FadeIn());
  }

  private IEnumerator FadeIn() {
    _canvasGroup.alpha = 0f;
    Time.timeScale = 0f;  // Пауза игры

    float timer = 0f;
    while (timer < _fadeInDuration) {
      _canvasGroup.alpha = timer / _fadeInDuration;
      timer += Time.unscaledDeltaTime;
      yield return null;
    }

    _canvasGroup.alpha = 1f;
  }

  public void HideMessage() {
    Time.timeScale = 1f;  // Возобновить игру
    IsShowing = false;
    gameObject.SetActive(false);
  }
}
