using UnityEngine;
using UnityEngine.UI;

public class PressureSystem : MonoBehaviour {
  [SerializeField]
  private Slider _leftPressureSlider;
  [SerializeField]
  private Slider _rightPressureSlider;
  [SerializeField]
  private float _transitionSpeed = 1f;

  private Luke _luke;
  private float _targetLeftValue;
  private float _targetRightValue;

  private void Awake() {
    _luke = GetComponent<Luke>();
    if (_luke == null) {
      Debug.LogError("Luke component not found on the same GameObject!");
      enabled = false;  // Отключаем скрипт, если Luke не найден
      return;
    }

    SetupInitialValues();
  }

  private void SetupInitialValues() {
    _leftPressureSlider.value = 0f;
    _rightPressureSlider.value = 1f;
    _targetLeftValue = 0f;
    _targetRightValue = 1f;
  }

  private void OnEnable() {
    if (_luke == null)
      return;

    _luke.OnLevelIncreased += UpdatePressureOnLevelUp;
    _luke.OnSequenceFailed += ResetPressure;
    _luke.OnSequenceCompleted += CompletePressure;
  }

  private void OnDisable() {
    if (_luke == null)
      return;

    _luke.OnLevelIncreased -= UpdatePressureOnLevelUp;
    _luke.OnSequenceFailed -= ResetPressure;
    _luke.OnSequenceCompleted -= CompletePressure;
  }

  private void Update() {
    _leftPressureSlider.value = Mathf.MoveTowards(_leftPressureSlider.value, _targetLeftValue,
                                                  _transitionSpeed * Time.deltaTime);

    _rightPressureSlider.value = Mathf.MoveTowards(_rightPressureSlider.value, _targetRightValue,
                                                   _transitionSpeed * Time.deltaTime);
  }

  private void UpdatePressureOnLevelUp(int newLevel) {
    float totalLevels = _luke.TotalLevels;
    float progress = (float)newLevel / totalLevels;

    // Изменено: левый слайдер идет от 0 до 0.5, правый от 1 до 0.5
    _targetLeftValue = progress * 0.5f;
    _targetRightValue = 1f - progress * 0.5f;
  }

  private void ResetPressure() {
    _targetLeftValue = 0f;
    _targetRightValue = 1f;
  }

  private void CompletePressure() {
    // Изменено: конечные значения теперь 0.5 для обоих слайдеров
    _targetLeftValue = 0.5f;
    _targetRightValue = 0.5f;
  }
}
