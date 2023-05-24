using Ui;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuUiController : MonoBehaviour, UiEventHandler {
    public MenuController menuController;
    public GameController gameController;
    public SceneType sceneType;
    public MenuPage menuPage;
    public GameObject mainMenuPanel;
    public GameObject chooseLevelPanel;
    public LevelButtonsGenerator levelButtonsGenerator;
    public AudioSource soundSource;
    public AudioSource musicSource;
    public AudioClip clickSound;

    // Start is called before the first frame update
    void Start() {
        gameController = menuController.gameController;
        SetMenuPage(MenuPage.MainMenu);
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetMenuPage(MenuPage menuPage) {
        this.menuPage = menuPage;
        SetVisiblePanel();
    }

    public void SetVisiblePanel() {
        mainMenuPanel.SetActive(menuPage == MenuPage.MainMenu);
        chooseLevelPanel.SetActive(menuPage == MenuPage.LevelChoice);
    }

    public void HandleUiEvent(UiEventType uiEventType) {
        switch (uiEventType) {
            case UiEventType.MainMenuPlayButtonClick:
                levelButtonsGenerator.GenerateButtons(1, 12);
                SetMenuPage(MenuPage.LevelChoice);
                break;
            case UiEventType.BackToMainMenuButtonClick:
                SetMenuPage(MenuPage.MainMenu);
                break;
            case UiEventType.ChooseLevelButtonClick:
                gameController.LoadScene(SceneType.Level);
                break;
            case UiEventType.MusicToggleClick:
                gameController.MusicToggle();
                break;
            case UiEventType.SoundToggleClick:
                gameController.SoundToggle();
                break;
            case UiEventType.QuitGameButtonClick:
                Application.Quit();
                break;
        }

        soundSource.PlayOneShot(clickSound);

        Debug.Log(uiEventType);
    }
}
