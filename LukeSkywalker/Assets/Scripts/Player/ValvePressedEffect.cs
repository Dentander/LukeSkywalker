using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ValvePressedEffect : MonoBehaviour {
  [Header("Color Settings")]
  [SerializeField]
  private Color _defaultColor = Color.green;
  [SerializeField]
  private Color _pressedColor = Color.red;
  [SerializeField]
  private float _colorTransitionSpeed = 3f;

  [Header("References")]
  [SerializeField]
  private Valve _valve;

  private Image _image;
  private Coroutine _colorTransitionCoroutine;

  private void Awake() {
    _image = GetComponent<Image>();
    if (_valve == null)
      _valve = GetComponent<Valve>();

    _image.color = _defaultColor;  // Устанавливаем начальный цвет
  }

  private void OnEnable() {
    if (_valve != null)
      _valve.OnValvePressed += HandleValvePressed;
  }

  private void OnDisable() {
    if (_valve != null)
      _valve.OnValvePressed -= HandleValvePressed;
  }

  private void HandleValvePressed(Valve valve) {
    if (_colorTransitionCoroutine != null)
      StopCoroutine(_colorTransitionCoroutine);

    _colorTransitionCoroutine = StartCoroutine(TransitionColor());
  }

  // private IEnumerator TransitionColor() {
  //   // Переход к красному цвету
  //   float t = 0f;
  //   while (t < 3f) {
  //     _image.color = Color.Lerp(_defaultColor, _pressedColor, t);
  //     t += Time.deltaTime * _colorTransitionSpeed;
  //     yield return null;
  //   }

  //  // Возврат к зеленому цвету
  //  t = 0f;
  //  while (t < 3f) {
  //    _image.color = Color.Lerp(_pressedColor, _defaultColor, t);
  //    t += Time.deltaTime * _colorTransitionSpeed;
  //    yield return null;
  //  }

  //  _image.color = _defaultColor;
  //}

  private bool _isInterrupted = false;

  public void InterruptEffect() {
    _isInterrupted = true;
    if (_colorTransitionCoroutine != null) {
      StopCoroutine(_colorTransitionCoroutine);
      _colorTransitionCoroutine = null;
    }
    _image.color = _defaultColor;
  }

  private IEnumerator TransitionColor() {
    _isInterrupted = false;

    // Быстрый переход к красному (уменьшаем время)
    float t = 0f;
    while (t < 3f && !_isInterrupted) {
      _image.color = Color.Lerp(_defaultColor, _pressedColor, t);
      t += Time.deltaTime * _colorTransitionSpeed;
      yield return null;
    }

    if (_isInterrupted) {
      _image.color = _defaultColor;
      yield break;
    }

    // Быстрый возврат к зеленому
    t = 0f;
    while (t < 3f && !_isInterrupted) {
      _image.color = Color.Lerp(_pressedColor, _defaultColor, t);
      t += Time.deltaTime * _colorTransitionSpeed;
      yield return null;
    }

    if (!_isInterrupted) {
      _image.color = _defaultColor;
    }
  }

  // private IEnumerator TransitionColor() {
  //   // Получаем ссылку на Luke (можно кэшировать в Awake)
  //   Luke luke = FindObjectOfType<Luke>();

  //  // Переход к красному цвету
  //  float t = 0f;
  //  while (t < 3f) {
  //    if (luke != null && luke.CurrentLevel == 1) {
  //      // Если уровень сброшен, прерываем анимацию
  //      _image.color = _defaultColor;
  //      yield break;
  //    }

  //    _image.color = Color.Lerp(_defaultColor, _pressedColor, t);
  //    t += Time.deltaTime * _colorTransitionSpeed;
  //    yield return null;
  //  }

  //  // Возврат к зеленому цвету
  //  t = 0f;
  //  while (t < 3f) {
  //    if (luke != null && luke.CurrentLevel == 1) {
  //      // Если уровень сброшен, прерываем анимацию
  //      _image.color = _defaultColor;
  //      yield break;
  //    }

  //    _image.color = Color.Lerp(_pressedColor, _defaultColor, t);
  //    t += Time.deltaTime * _colorTransitionSpeed;
  //    yield return null;
  //  }

  //  _image.color = _defaultColor;
  //}

  // Для внешнего вызова
  public void ApplyEffect(Valve valve) {
    HandleValvePressed(valve);
  }
}
