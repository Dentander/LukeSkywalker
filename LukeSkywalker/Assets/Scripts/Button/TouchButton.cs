using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class TouchButton : MonoBehaviour {
  public event Action OnPress;

  private TouchInput _touchInput;
  private Collider2D _collider;
  private bool _wasTouched;

  private void Start() {
    _touchInput = FindObjectOfType<TouchInput>();
    _collider = GetComponent<Collider2D>();
  }

  private bool IsPointOnButton(Vector2 point) {
    return _collider.OverlapPoint(point);
  }

  private void Update() {
    if (_touchInput == null)
      return;

    if (_touchInput.IsActive && IsPointOnButton(_touchInput.TouchPosInGame)) {
      if (!_wasTouched) {
        HandlePress();
        OnPress?.Invoke();
      }
      _wasTouched = true;
    } else {
      _wasTouched = false;
    }
  }

  protected abstract void HandlePress();
}
