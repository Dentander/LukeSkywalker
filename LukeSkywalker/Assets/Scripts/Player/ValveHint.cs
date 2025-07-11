using System.Collections;
using UnityEngine;

public class ValveHint : MonoBehaviour {
  [SerializeField]
  private SO_ValveSequence _soValvesSequence;
  private Luke _luke;
  private Coroutine _hintCoroutine;

  private void Awake() {
    _luke = GetComponent<Luke>();
  }

  public void Begin() {
    if (_hintCoroutine != null)
      StopCoroutine(_hintCoroutine);

    _hintCoroutine = StartCoroutine(ShowHints());
  }

  private IEnumerator ShowHints() {
    yield return new WaitForSeconds(1f);

    for (int i = 0; i < _luke.CurrentLevel; i++) {
      Valve valve = _luke.GetValveByType(_soValvesSequence.ValveSequence[i]);
      if (valve != null) {
        valve.GetComponent<ValvePressedEffect>()?.ApplyEffect(valve);
        yield return new WaitForSeconds(1f);
      }
    }
  }
}
