using System.Collections;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(CanvasGroup))]
public class GameMessage : MonoBehaviour {
  [Header("Animation Settings")]
  [SerializeField]
  private float _pulseIntensity = 0.1f;
  [SerializeField]
  private float _pulseSpeed = 20f;
  [SerializeField]
  private int _pulseCount = 2;

  private TextMeshProUGUI _messageText;
  private CanvasGroup _canvasGroup;
  private Vector3 _originalScale;
  private Coroutine _animationCoroutine;

  public float TotalAnimationDuration => 0.5f + (0.3f * _pulseCount) + 1f;

  private void Awake() {
    // Получаем компоненты автоматически
    _messageText = GetComponent<TextMeshProUGUI>();
    _canvasGroup = GetComponent<CanvasGroup>();

    if (_messageText == null || _canvasGroup == null) {
      Debug.LogError("Required components not found!", this);
      enabled = false;
      return;
    }

    _originalScale = transform.localScale;
    ResetState();
  }

  private void ResetState() {
    _canvasGroup.alpha = 0f;
    transform.localScale = _originalScale * 0.8f;
    gameObject.SetActive(true);
  }

  public void ShowMessage(string message) {
    if (!isActiveAndEnabled)
      gameObject.SetActive(true);

    if (_animationCoroutine != null)
      StopCoroutine(_animationCoroutine);

    _messageText.text = message;
    _messageText.color = Color.red;

    _animationCoroutine = StartCoroutine(AnimateMessage());
  }

  private IEnumerator AnimateMessage() {
    ResetState();
    // Активируем объект (на случай если был деактивирован)
    gameObject.SetActive(true);

    // Быстрое появление
    float timer = 0f;
    while (timer < 0.5f) {
      _canvasGroup.alpha = timer * 2f;
      transform.localScale = _originalScale * (0.8f + timer * 0.8f);
      timer += Time.deltaTime;
      yield return null;
    }

    // Пульсация
    for (int i = 0; i < _pulseCount; i++) {
      timer = 0f;
      while (timer < 0.3f) {
        float pulse = Mathf.Sin(timer * _pulseSpeed) * _pulseIntensity;
        transform.localScale = _originalScale * (1.2f + pulse);
        timer += Time.deltaTime;
        yield return null;
      }
    }

    // Плавное исчезновение
    timer = 0f;
    while (timer < 1f) {
      _canvasGroup.alpha = 1f - timer;
      transform.localScale = _originalScale * (1f + timer * 0.2f);
      timer += Time.deltaTime;
      yield return null;
    }

    _canvasGroup.alpha = 0f;
    transform.localScale = _originalScale * 0.8f;
  }

  public void SetComponents(TextMeshProUGUI text, CanvasGroup canvasGroup) {
    _messageText = text;
    _canvasGroup = canvasGroup;
    _originalScale = transform.localScale;

    // Принудительная активация
    gameObject.SetActive(true);
    _canvasGroup.alpha = 0f;
    transform.localScale = _originalScale * 0.8f;
  }
}
