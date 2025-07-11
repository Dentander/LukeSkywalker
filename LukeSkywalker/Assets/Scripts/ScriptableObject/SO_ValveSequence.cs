using UnityEngine;

[CreateAssetMenu(fileName = "ValveSequence", menuName = "Valve/Sequence")]
public class SO_ValveSequence : ScriptableObject {
    [SerializeField] public ValveType[] ValveSequence;
}
