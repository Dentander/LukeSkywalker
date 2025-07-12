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

    _image.color = _defaultColor;  // ������������� ��������� ����
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
  //   // ������� � �������� �����
  //   float t = 0f;
  //   while (t < 3f) {
  //     _image.color = Color.Lerp(_defaultColor, _pressedColor, t);
  //     t += Time.deltaTime * _colorTransitionSpeed;
  //     yield return null;
  //   }

  //  // ������� � �������� �����
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

    // ������� ������� � �������� (��������� �����)
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

    // ������� ������� � ��������
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
  //   // �������� ������ �� Luke (����� ���������� � Awake)
  //   Luke luke = FindObjectOfType<Luke>();

  //  // ������� � �������� �����
  //  float t = 0f;
  //  while (t < 3f) {
  //    if (luke != null && luke.CurrentLevel == 1) {
  //      // ���� ������� �������, ��������� ��������
  //      _image.color = _defaultColor;
  //      yield break;
  //    }

  //    _image.color = Color.Lerp(_defaultColor, _pressedColor, t);
  //    t += Time.deltaTime * _colorTransitionSpeed;
  //    yield return null;
  //  }

  //  // ������� � �������� �����
  //  t = 0f;
  //  while (t < 3f) {
  //    if (luke != null && luke.CurrentLevel == 1) {
  //      // ���� ������� �������, ��������� ��������
  //      _image.color = _defaultColor;
  //      yield break;
  //    }

  //    _image.color = Color.Lerp(_pressedColor, _defaultColor, t);
  //    t += Time.deltaTime * _colorTransitionSpeed;
  //    yield return null;
  //  }

  //  _image.color = _defaultColor;
  //}

  // ��� �������� ������
  public void ApplyEffect(Valve valve) {
    HandleValvePressed(valve);
  }
}
