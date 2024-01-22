using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LoadCode : MonoBehaviour
{
    void Awake()
    {
        TMP_InputField code = GetComponent<TMP_InputField>();
        code.text = DataManager.GetCode();
    }
}
