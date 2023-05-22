using System;
using UnityEngine;

namespace UI {
    public class UiController : MonoBehaviour {
        public void Start() {

        }

        public void Update() {

        }

        public void HandleUiEvent(UiEventType uiEventType) {
            switch (uiEventType) {
                case UiEventType.HintButtonClick:
                    break;
                case UiEventType.MusicToggleClick:
                    break;
                case UiEventType.QuitLevelButtonClick:
                    break;
                case UiEventType.RestartButtonClick:
                    break;
                case UiEventType.SoundToggleClick:
                    break;
                case UiEventType.NextLevelButtonClick:
                    break;
                case UiEventType.QuitGameButtonClick:
                    break;
            }

            Debug.Log(uiEventType);
        }
    }
}
