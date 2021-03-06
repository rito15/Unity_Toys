using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_BoolAsButton : MonoBehaviour
{
    public bool _flag;
    public int value = 0;

    private void OnValidate()
    {
        if (_flag)
        {
            Method();
            _flag = false;
        }
    }

    private void Method()
    {
        Debug.Log("Method");
        value++;
    }
}
