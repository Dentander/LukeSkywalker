using System.Collections;
using UnityEngine;

public class ValveHint : MonoBehaviour {
    [SerializeField] private SO_ValveSequence _soValvesSequence;
    private Luke _luke;

    private void Start() {
        _luke = GetComponent<Luke>();
        Begin();
    }

    public void Begin() {
        StartCoroutine(IBegin());
    }

    public IEnumerator IBegin() {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < _luke.PassedValves; ++i) {

            Valve valve = _luke.GetValveByType(_soValvesSequence.ValveSequence[i]);
            Debug.Log(valve);
            valve.gameObject.GetComponent<ValvePressedEffect>().ApplyEffect();
            yield return new WaitForSeconds(1f);
            Debug.Log("adadad    " + i);    
        }
    }
}
