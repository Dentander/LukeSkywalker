using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Valve))]
public class ValvePressedEffect : MonoBehaviour {
    private SpriteRenderer _renderer;
    private Valve _valve;

    private void Start() {
        _renderer = GetComponent<SpriteRenderer>();
        _valve = GetComponent<Valve>();
    }

    private void ApplyEffect() {
        _renderer.color = Color.black;
        StartCoroutine(DisableEffect());
    }

    IEnumerator DisableEffect() {
        yield return new WaitForSeconds(0.6f);
        _renderer.color = Color.white;
    }

    private void OnEnable() {
        if (_valve ==  null) {
            _renderer = GetComponent<SpriteRenderer>();
            _valve = GetComponent<Valve>();
        }
        _valve.OnPress += ApplyEffect;
    }

    private void OnDisable() => _valve.OnPress -= ApplyEffect;
}
