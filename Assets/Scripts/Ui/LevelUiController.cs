using System;
using Level;
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
                    HideLosePanel();
                    HideWinPanel();
                    levelController.RestartLevel();
                    break;
                case UiEventType.SoundToggleClick:
                    gameController.SoundToggle();
                    break;
                case UiEventType.NextLevelButtonClick:
                    gameController.IncreaseLevelNumber();
                    HideLosePanel();
                    HideWinPanel();
                    levelController.RestartLevel();
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

        public void ShowLosePanel(LevelProgressStage stage) {
            LosePanel.SetActive(true);
            (var label, var text) = ("", "");
            switch (stage) {
                case LevelProgressStage.NoTurnsLeftLose:
                    label = "Закончились ходы";
                    text = "Ходы закончились. Не расстраивайтесь! Попробуйте ещё раз";
                    break;
                case LevelProgressStage.NoCombinationsLeftLose:
                    label = "Нет комбинаций";
                    text = "На поле не осталось ходов. Это чистая случайность. Попробуйте снова";
                    break;
                default:
                    Debug.LogError($"Unexpected stage for lose handling {stage}");
                    break;
            }
            losePanelLabelField.SetValue(label);
            losePanelTextField.SetValue(text);
        }

        public void HideWinPanel() {
            WinPanel.SetActive(false);
        }

        public void HideLosePanel() {
            LosePanel.SetActive(false);
        }
    }
}
