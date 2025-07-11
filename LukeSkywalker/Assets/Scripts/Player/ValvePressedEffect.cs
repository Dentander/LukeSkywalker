using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Valve))]
public class ValvePressedEffect : MonoBehaviour {
  private Valve _valve;

  [SerializeField]
  private Sprite _pressedSprite;
  [SerializeField]
  private Sprite _defaultSprite;

  private void Start() {
    _valve = GetComponent<Valve>();
  }

  public void ApplyEffect() {
    GetComponent<Image>().sprite = _pressedSprite;
    StartCoroutine(DisableEffect());
  }

  private IEnumerator DisableEffect() {
    yield return new WaitForSeconds(0.6f);
    GetComponent<Image>().sprite = _defaultSprite;
  }

  private void OnEnable() {
    if (_valve == null) {
      _valve = GetComponent<Valve>();
    }
    _valve.OnPress += ApplyEffect;
  }

  private void OnDisable() => _valve.OnPress -= ApplyEffect;
}
