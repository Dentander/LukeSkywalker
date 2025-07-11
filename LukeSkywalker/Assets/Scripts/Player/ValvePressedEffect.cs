using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ValvePressedEffect : MonoBehaviour {
  [SerializeField]
  private Sprite _pressedSprite;
  [SerializeField]
  private Sprite _defaultSprite;

  private Valve _valve;
  private Image _image;

  private void Start() {
    _valve = GetComponent<Valve>();
    _image = GetComponent<Image>();
    _valve.OnValvePressed += ApplyEffect;
  }

  public void ApplyEffect(Valve valve) {
    if (valve != _valve)
      return;

    _image.sprite = _pressedSprite;
    StartCoroutine(DisableEffect());
  }

  private IEnumerator DisableEffect() {
    yield return new WaitForSeconds(0.6f);
    _image.sprite = _defaultSprite;
  }

  private void OnDestroy() {
    if (_valve != null)
      _valve.OnValvePressed -= ApplyEffect;
  }
}
