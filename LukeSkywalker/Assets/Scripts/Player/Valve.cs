using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Valve : MonoBehaviour {
  [field:SerializeField]
  public ValveType Type { get; private set; }
  public event Action<Valve> OnValvePressed;

  private Button _button;

  private void Awake() {
    _button = GetComponent<Button>();
    _button.onClick.AddListener(OnButtonClick);
  }

  private void OnButtonClick() {
    OnValvePressed?.Invoke(this);
  }

  private void OnDestroy() {
    if (_button != null)
      _button.onClick.RemoveListener(OnButtonClick);
  }
}
