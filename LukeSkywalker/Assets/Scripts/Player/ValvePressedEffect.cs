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

  private IEnumerator TransitionColor() {
    // Переход к красному цвету
    float t = 0f;
    while (t < 1f) {
      _image.color = Color.Lerp(_defaultColor, _pressedColor, t);
      t += Time.deltaTime * _colorTransitionSpeed;
      yield return null;
    }

    // Возврат к зеленому цвету
    t = 0f;
    while (t < 1f) {
      _image.color = Color.Lerp(_pressedColor, _defaultColor, t);
      t += Time.deltaTime * _colorTransitionSpeed;
      yield return null;
    }

    _image.color = _defaultColor;
  }

  // Для внешнего вызова (например из ValveHint)
  public void ApplyEffect(Valve valve) {
    HandleValvePressed(valve);
  }
}
