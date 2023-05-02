using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {
    public UiController UiController;
    public UiEventType UiEventType;

    public int buttonState; //0 - unpressed, 1 - pressed
    public Sprite[] switchSprites;
    public Image switchImage;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendUiMessage() {
        UiController.HandleUiEvent(UiEventType);
    }

    public void ChangeImageOnClick() {
        switch (UiEventType) {
            case UiEventType.MusicToggleClick:
            case UiEventType.SoundToggleClick:
                buttonState = 1 - buttonState;
                switchImage.sprite = switchSprites[buttonState];
                break;
        }
    }
}
