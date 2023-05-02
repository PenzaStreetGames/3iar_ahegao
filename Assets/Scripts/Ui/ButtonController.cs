using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class ButtonController : MonoBehaviour {
    public UiController UiController;
    public UiEventType UiEventType;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendUiMessage() {
        UiController.HandleUiEvent(UiEventType);
    }
}
