using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TouchButton : MonoBehaviour {
    public event Action OnPress;

    private TouchInput _touchInput;
    private Collider2D _collider;
    private bool _wasTouched = false;

    private void Start() {
        _touchInput = FindObjectOfType<TouchInput>();
        _collider = GetComponent<Collider2D>();
    }

    private bool IsPointOnButton(Vector2 point) => _collider.OverlapPoint(point);

    void Update() {
        if (
            _touchInput.IsActive &&
            IsPointOnButton(_touchInput.TouchPosInGame)
        ) {
            if (!_wasTouched) { OnPress?.Invoke(); }
            _wasTouched = true;
        }
        else { _wasTouched = false; }
    }
}
