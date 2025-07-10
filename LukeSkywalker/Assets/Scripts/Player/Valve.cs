using System;
using UnityEngine;

public class Valve : TouchButton {
    [SerializeField] public ValveType Type { get; private set; }

    public event Action<Valve> OnValvePressed;

    private void Invoke() => OnValvePressed?.Invoke(this);

    private void OnEnable() => OnPress += Invoke;

    private void OnDisable() => OnPress -= Invoke;
}
