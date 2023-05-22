using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TextFieldController : MonoBehaviour {

    public TMP_Text Text;
    public string Value;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetValue(string value) {
        Text.text = value;
        Value = value;
    }

    public string GetValue() {
        return Value;
    }
}
