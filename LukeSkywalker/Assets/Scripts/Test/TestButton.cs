using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour
{
    private TouchButton _touchButton;

    void Start()
    {
        _touchButton = GetComponent<TouchButton>();
        _touchButton.OnPress += Test;
    }

    void Test() => Debug.Log("UIUI");
}
