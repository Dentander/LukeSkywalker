using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
    private TouchButton _touchButton;

    private void GetComponents() {
        _touchButton = GetComponent<TouchButton>();
    }

    private void Start() {
        GetComponents();
    }

    private void LoadGameScene() {
        SceneManager.LoadScene(1);
    }

    private void OnEnable() {
        if (_touchButton == null) {
            GetComponents();
        }

        _touchButton.OnPress += LoadGameScene;
    }

    private void OnDisable() {
        _touchButton.OnPress -= LoadGameScene;        
    }
}
