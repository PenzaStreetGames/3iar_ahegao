using System;
using Ui;
using UnityEngine;

namespace UI {
    public class LevelUiController : MonoBehaviour, UiEventHandler {
        public TextFieldController scoreField;
        public TextFieldController turnsField;
        public TextFieldController destroyedTilesField;
        public GameObject WinPanel;
        public GameObject LosePanel;
        public LevelController levelController;
        public GameController gameController;
        public TextFieldController winPanelTextField;
        public TextFieldController losePanelLabelField;
        public TextFieldController losePanelTextField;
        public AudioSource soundSource;
        public AudioClip clickSound;

        public void Start() {
            gameController = levelController.gameController;
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
                    gameController.StartLevel(gameController.levelNumber);
                    break;
                case UiEventType.SoundToggleClick:
                    gameController.SoundToggle();
                    break;
                case UiEventType.NextLevelButtonClick:
                    gameController.levelNumber += 1;
                    gameController.StartLevel(gameController.levelNumber);
                    break;
                case UiEventType.QuitGameButtonClick:
                    break;
            }

            soundSource.PlayOneShot(clickSound);

            Debug.Log(uiEventType);
        }

        public void ShowWinPanel(int score) {
            WinPanel.SetActive(true);
            winPanelTextField.SetValue($"Набрано {score} очков");
        }

        public void ShowLosePanel(string reason) {
            LosePanel.SetActive(true);
            losePanelLabelField.SetValue(reason);
        }
    }
}
