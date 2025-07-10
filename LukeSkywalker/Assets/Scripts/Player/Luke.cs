using System;
using UnityEngine;

public class Luke : MonoBehaviour {
    [SerializeField] private Valve[] _valves;
    [SerializeField] private SO_ValveSequence _soValvesSequence;
    
    public event Action OnWin;
    public event Action OnValidValvePressed;
    public event Action OnInvalidValvePressed;
    public int PassedValves { get; private set; } = 0;
    public int NextValve { get; private set; } = 0;

    public Valve GetValveByType(ValveType type) {
        foreach (Valve valve in _valves) {
            if (valve.Type == type) { return valve; }
        }
        return null;
    }

    private void HandleValvePressed(Valve valve) {
        if (valve.Type == _soValvesSequence.ValveSequence[NextValve]) {
            ++NextValve;
            OnValidValvePressed?.Invoke();
        }
        else {
            NextValve = 0;
            OnInvalidValvePressed?.Invoke();
        }
        if (NextValve == _soValvesSequence.ValveSequence.Length) { OnWin?.Invoke(); }
    }

    private void OnEnable() {
        foreach (Valve valve in _valves) {
            valve.OnValvePressed += HandleValvePressed;
        }
    }

    private void OnDisable() {
        foreach (Valve valve in _valves) {
            valve.OnValvePressed -= HandleValvePressed;
        }
    }
}
