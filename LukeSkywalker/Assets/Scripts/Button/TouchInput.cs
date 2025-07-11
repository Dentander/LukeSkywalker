using UnityEngine;

public class TouchInput : MonoBehaviour {
  [SerializeField]
  private KeyCode _interactionKey = KeyCode.Space;
  private Camera _mainCamera;

  public bool IsActive => Input.touchCount > 0 ||
                          Input.GetKey(_interactionKey) || Input.GetMouseButton(0);

  public Vector2 TouchPosInGame {
    get {
      if (!IsActive)
        return Vector2.zero;

      Vector3 screenPos = Input.touchCount > 0?(Vector3)Input.GetTouch(0).position
          : Input.mousePosition;

      return _mainCamera.ScreenToWorldPoint(screenPos);
    }
  }

  private void Start() {
    _mainCamera = Camera.main;
  }
}
