using UnityEngine;

public class TouchInput : MonoBehaviour {
    [SerializeField] private KeyCode _key = KeyCode.Space;

    private Camera _camera;

    private void Start() {
        _camera = FindObjectOfType<Camera>();
    }

    public bool IsActive => Input.touchCount > 0 || Input.GetKey(_key);

    public Vector3 TouchPosInGame {
        get {
            if (!IsActive) { return Vector3.zero; }

            Vector3 posOnScreen = (Input.touchCount > 0) ? 
                                      Input.GetTouch(0).position : 
                                      Input.mousePosition;

            Vector2 res = _camera.ScreenToWorldPoint(posOnScreen);
            return new Vector3(res.x, res.y, 0);
        }
    }
}
