using System;
using UnityEngine;

namespace UI {
    public class UiController : MonoBehaviour {

        public GameController gameController;
        public void Start() {

        }

        public void Update() {

        }

        public void HandleUiEvent(UiEventType uiEventType) {
            switch (uiEventType) {
                case UiEventType.HintButtonClick:
                    break;
                case UiEventType.MusicToggleClick:
                    gameController.MusicToggle();
                    break;
                case UiEventType.QuitLevelButtonClick:
                    gameController.QuitLevel();
                    break;
                case UiEventType.RestartButtonClick:
                    gameController.StartLevel();
                    break;
                case UiEventType.SoundToggleClick:
                    gameController.SoundToggle();
                    break;
                case UiEventType.NextLevelButtonClick:
                    gameController.StartLevel();
                    break;
                case UiEventType.QuitGameButtonClick:
                    break;
            }

            Debug.Log(uiEventType);
        }
    }
}
