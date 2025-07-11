using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
  public void LoadGameScene() {
    Debug.Log("GameScene");
    SceneManager.LoadScene("GameScene");
  }

  public void QuitGame() => Application.Quit();
}
